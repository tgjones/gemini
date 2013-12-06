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
                    ActivateItem((IDocument) value);
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

        public const string StateFile = @".\ApplicationState.bin";

        public static bool HasPersistedState
        {
            get { return File.Exists(StateFile); }
        }

		public ShellViewModel()
		{
		    ((IActivate) this).Activate();

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

            private DummyTool() {}
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

	        var shellView = (IShellView) view;
	        if (!HasPersistedState)
	        {
	            foreach (var defaultDocument in _modules.SelectMany(x => x.DefaultDocuments))
	                OpenDocument(defaultDocument);
	            foreach (var defaultTool in _modules.SelectMany(x => x.DefaultTools))
	                ShowTool((ITool) IoC.GetInstance(defaultTool, null));
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
	                    {
	                        continue;
	                    }

	                    ExportAttribute exportAttribute =
	                        item.GetType()
	                            .GetCustomAttributes(typeof (ExportAttribute), false)
	                            .Cast<ExportAttribute>()
	                            .FirstOrDefault();

	                    string typeName = null;

	                    if (exportAttribute != null && exportAttribute.ContractType != null)
	                    {
	                        typeName = exportAttribute.ContractType.AssemblyQualifiedName;
	                    }

	                    if (string.IsNullOrEmpty(typeName))
	                    {
	                        continue;
	                    }

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
            catch
            {
                if (stream != null)
                {
                    stream.Close();
                }
            }
        }
	}
}