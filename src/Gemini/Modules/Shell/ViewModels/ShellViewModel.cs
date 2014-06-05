﻿using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Windows;
using Caliburn.Micro;
using Gemini.Framework;
using Gemini.Framework.Services;
using Gemini.Modules.MainMenu;
using Gemini.Modules.Shell.Views;
using Gemini.Modules.StatusBar;
using Gemini.Modules.ToolBars;

namespace Gemini.Modules.Shell.ViewModels
{
    [Export(typeof(IShell))]
    public class ShellViewModel : Conductor<IDocument>.Collection.OneActive, IShell
    {
        public event EventHandler ActiveDocumentChanging;
        public event EventHandler ActiveDocumentChanged;
        public event EventHandler CurrentThemeChanged;

        [ImportMany(typeof(IModule))]
        private IEnumerable<IModule> _modules;

        private bool _closing;

        private ResourceDictionary _currentTheme;
        public ResourceDictionary CurrentTheme
        {
            get { return _currentTheme; }
            set
            {
                if (_currentTheme == value)
                    return;

                if (_currentTheme != null)
                    Application.Current.Resources.MergedDictionaries.Remove(_currentTheme);

                _currentTheme = value;

                if (_currentTheme != null)
                    Application.Current.Resources.MergedDictionaries.Add(_currentTheme);

                if (CurrentThemeChanged != null)
                    CurrentThemeChanged(this, EventArgs.Empty);
            }
        }

        [Import]
        private IMenu _mainMenu;
        public IMenu MainMenu
        {
            get { return _mainMenu; }
        }

        [Import]
        private IToolBars _toolBars;
        public IToolBars ToolBars
        {
            get { return _toolBars; }
        }

        [Import]
        private IStatusBar _statusBar;
        public IStatusBar StatusBar
        {
            get { return _statusBar; }
        }

        private ILayoutItem _currentActiveItem;
        public ILayoutItem CurrentActiveItem
        {
            get { return _currentActiveItem; }
            set
            {
                _currentActiveItem = value;
                if (value is IDocument)
                    ActivateItem((IDocument)value);
                NotifyOfPropertyChange(() => CurrentActiveItem);
            }
        }

        private readonly BindableCollection<ITool> _tools;

        public IObservableCollection<ITool> Tools
        {
            get { return _tools; }
        }

        public IObservableCollection<IDocument> Documents
        {
            get { return Items; }
        }

        private string _stateFile= Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ApplicationState.bin");
        private bool _stateFileLoaded;

        /// <summary>
        /// The location where ApplicationState.bin will be saved to and loaded from.
        /// The default location is the executable file directory (AppDomain.CurrentDomain.BaseDirectory + "ApplicationState.bin).
        /// </summary>
        public string StateFile
        {
            get { return _stateFile; }
            set
            {
                if(_stateFileLoaded)
                    throw new InvalidOperationException(
                        "StateFile can not be set after PostInitialize() has been called on all IModules.");
                _stateFile = value;
            }
        }

        public bool HasPersistedState
        {
            get { return File.Exists(StateFile); }
        }

        public ShellViewModel()
        {
            ((IActivate)this).Activate();

            _tools = new BindableCollection<ITool>();

            if (!HasPersistedState)
            {
                // This workaround is necessary until https://avalondock.codeplex.com/workitem/15577
                // is applied, or the bug is fixed in another way.
                _tools.Add(new DummyTool(PaneLocation.Left));
                _tools.Add(new DummyTool(PaneLocation.Right));
                _tools.Add(new DummyTool(PaneLocation.Bottom));
            }
        }

        [Export(typeof(DummyTool))]
        private class DummyTool : Tool
        {
            private readonly PaneLocation _preferredLocation;

            public override PaneLocation PreferredLocation
            {
                get { return _preferredLocation; }
            }

            public DummyTool(PaneLocation preferredLocation)
            {
                _preferredLocation = preferredLocation;
                IsVisible = false;
            }

            private DummyTool() { }
        }

        protected override void OnViewLoaded(object view)
        {
            foreach (var module in _modules)
                module.PreInitialize();
            foreach (var module in _modules)
                module.Initialize();

            // If after initialization no theme was loaded, load the default one
            if (_currentTheme == null)
                CurrentTheme = new ResourceDictionary
                {
                    Source = new Uri("/Gemini;component/Themes/VS2010/Theme.xaml", UriKind.Relative)
                };

            var shellView = (IShellView)view;
            if (!HasPersistedState)
            {
                foreach (var defaultDocument in _modules.SelectMany(x => x.DefaultDocuments))
                    OpenDocument(defaultDocument);
                foreach (var defaultTool in _modules.SelectMany(x => x.DefaultTools))
                    ShowTool((ITool)IoC.GetInstance(defaultTool, null));
            }
            else
            {
                LoadState(StateFile, shellView);
            }

            foreach (var module in _modules)
                module.PostInitialize();

            base.OnViewLoaded(view);
        }

        public void ShowTool(ITool model)
        {
            if (Tools.Contains(model))
                model.IsVisible = true;
            else
                Tools.Add(model);
            model.IsSelected = true;
            CurrentActiveItem = model;
        }

        public void OpenDocument(IDocument model)
        {
            ActivateItem(model);
        }

        public void CloseDocument(IDocument document)
        {
            DeactivateItem(document, true);
        }

        public override void ActivateItem(IDocument item)
        {
            if (_closing)
                return;

            var handler = ActiveDocumentChanging;
            if (handler != null)
                handler(this, EventArgs.Empty);

            base.ActivateItem(item);
        }

        protected override void OnActivationProcessed(IDocument item, bool success)
        {
            var handler = ActiveDocumentChanged;
            if (handler != null)
                handler(this, EventArgs.Empty);

            if (CurrentActiveItem != item)
                CurrentActiveItem = item;

            base.OnActivationProcessed(item, success);
        }

        protected override void OnDeactivate(bool close)
        {
            // Workaround for a complex bug that occurs when
            // (a) the window has multiple documents open, and
            // (b) the last document is NOT active
            // 
            // The issue manifests itself with a crash in
            // the call to base.ActivateItem(item), above,
            // saying that the collection can't be changed
            // in a CollectionChanged event handler.
            // 
            // The issue occurs because:
            // - Caliburn.Micro sees the window is closing, and calls Items.Clear()
            // - AvalonDock handles the CollectionChanged event, and calls Remove()
            //   on each of the open documents.
            // - If removing a document causes another to become active, then AvalonDock
            //   sets a new ActiveContent.
            // - We have a WPF binding from Caliburn.Micro's ActiveItem to AvalonDock's
            //   ActiveContent property, so ActiveItem gets updated.
            // - The document no longer exists in Items, beacuse that collection was cleared,
            //   but Caliburn.Micro helpfully adds it again - which causes the crash.
            //
            // My workaround is to use the following _closing variable, and ignore activation
            // requests that occur when _closing is true.
            _closing = true;

            SaveState(StateFile);

            base.OnDeactivate(close);
        }

        public void Close()
        {
            Application.Current.MainWindow.Close();
        }

        private void SaveState(string fileName)
        {
            FileStream stream = null;

            try
            {
                stream = new FileStream(fileName, FileMode.Create, FileAccess.Write);

                using (var writer = new BinaryWriter(stream))
                {
                    stream = null;

                    IEnumerable<ILayoutItem> itemStates = Documents.Concat(Tools.Cast<ILayoutItem>());

                    int itemCount = 0;
                    // reserve some space for items count, it'll be updated later
                    writer.Write(itemCount);

                    foreach (ILayoutItem item in itemStates)
                    {
                        if (!item.ShouldReopenOnStart)
                            continue;

                        var itemType = item.GetType();
                        List<ExportAttribute> exportAttributes = itemType
                                .GetCustomAttributes(typeof(ExportAttribute), false)
                                .Cast<ExportAttribute>().ToList();
                        var geminiExport = exportAttributes.OfType<GeminiExportAttribute>().FirstOrDefault();
                        var itemTypeName = itemType.AssemblyQualifiedName;

                        // throw exceptions here, instead of failing silently. These are design time errors.
                        if (exportAttributes.Count == 0)
                            throw new InvalidOperationException(string.Format(
                                "A ViewModel that participates in LayoutItem.ShouldReopenOnStart must be decorated with an ExportAttribute, infringing type is {0}", itemType));
                        if (exportAttributes.Count > 1 && geminiExport == null)
                            throw new InvalidOperationException(string.Format(
                                "Ambiguity between multiple MEF exports on {0}. Mark one Mef export as a GeminiExport.", itemType));
                        if (string.IsNullOrEmpty(itemTypeName))
                            throw new Exception(string.Format(
                                "Could not retrieve the assembly qualified type name for {0}, most likely because the type is generic.", itemType));
                        // TODO: it is possible to save generic types. It requires that every generic parameter is saved, along with its position in the generic tree... A lot of work.

                        // find the type name of the export
                        var mainExportAttribute = (geminiExport == null || geminiExport.ContractName == null || geminiExport.ContractType == null)
                            ? exportAttributes.First()
                            : geminiExport;
                        string typeName;
                        if (mainExportAttribute.ContractName != null)
                            typeName = mainExportAttribute.ContractName;
                        else if (mainExportAttribute.ContractType != null)
                            typeName = mainExportAttribute.ContractType.AssemblyQualifiedName;
                        else typeName = itemTypeName;

                        // ReSharper disable once AssignNullToNotNullAttribute - checked earlier in the method
                        writer.Write(typeName);
                        writer.Write(item.ContentId);

                        // Here's the tricky part. Because some items might fail to save their state, or they might be removed (a plug-in assembly deleted and etc.)
                        // we need to save the item's state size to be able to skip the data during deserialization.
                        // Save surrent stream position. We'll need it later.
                        long stateSizePosition = writer.BaseStream.Position;

                        // Reserve some space for item state size
                        writer.Write(0L);

                        long stateSize;

                        try
                        {
                            long stateStartPosition = writer.BaseStream.Position;
                            item.SaveState(writer);
                            stateSize = writer.BaseStream.Position - stateStartPosition;
                        }
                        catch
                        {
                            stateSize = 0;
                        }

                        // Go back to the position before item's state and write the actual value.
                        writer.BaseStream.Seek(stateSizePosition, SeekOrigin.Begin);
                        writer.Write(stateSize);

                        if (stateSize > 0)
                        {
                            // Got to the end of the stream
                            writer.BaseStream.Seek(0, SeekOrigin.End);
                        }

                        itemCount++;
                    }

                    writer.BaseStream.Seek(0, SeekOrigin.Begin);
                    writer.Write(itemCount);
                    writer.BaseStream.Seek(0, SeekOrigin.End);

                    var shellView = Views.Values.Single() as IShellView;
                    if (shellView != null)
                        shellView.SaveLayout(writer.BaseStream);
                }
            }
            catch
            {
                if (stream != null)
                {
                    stream.Close();
                }
            }
        }

        private void LoadState(string fileName, IShellView shellView)
        {
            _stateFileLoaded = true;
            var layoutItems = new Dictionary<string, ILayoutItem>();

            if (!File.Exists(fileName))
            {
                return;
            }

            FileStream stream = null;

            try
            {
                stream = new FileStream(fileName, FileMode.Open, FileAccess.Read);

                using (var reader = new BinaryReader(stream))
                {
                    stream = null;

                    int count = reader.ReadInt32();

                    for (int i = 0; i < count; i++)
                    {
                        string typeName = reader.ReadString();
                        string contentId = reader.ReadString();
                        long stateEndPosition = reader.ReadInt64();
                        stateEndPosition += reader.BaseStream.Position;

                        var contentType = Type.GetType(typeName);
                        bool skipStateData = true;

                        if (contentType != null)
                        {
                            var contentInstance = IoC.GetInstance(contentType, null) as ILayoutItem;

                            if (contentInstance != null)
                            {
                                layoutItems.Add(contentId, contentInstance);

                                try
                                {
                                    contentInstance.LoadState(reader);
                                    skipStateData = false;
                                }
                                catch
                                {
                                    skipStateData = true;
                                }
                            }
                        }

                        // Skip state data block if we couldn't read it.
                        if (skipStateData)
                        {
                            reader.BaseStream.Seek(stateEndPosition, SeekOrigin.Begin);
                        }
                    }

                    shellView.LoadLayout(reader.BaseStream, ShowTool, OpenDocument, layoutItems);
                }
            }
            finally
            {
                if (stream != null)
                {
                    stream.Dispose();
                }
            }
        }
    }
}