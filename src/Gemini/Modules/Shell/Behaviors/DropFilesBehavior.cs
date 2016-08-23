using Caliburn.Micro;
using Gemini.Framework;
using Gemini.Framework.Commands;
using Gemini.Framework.Services;
using Gemini.Modules.Shell.Commands;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Gemini.Modules.Shell.Behaviors
{
    /// <summary>
    /// Attached behaviour to implement the drop event
    /// source: http://social.msdn.microsoft.com/Forums/de-DE/wpf/thread/21bed380-c485-44fb-8741-f9245524d0ae
    ///         http://stackoverflow.com/questions/1034374/drag-and-drop-in-mvvm-with-scatterview
    /// </summary>
    public class DropFilesBehavior : DependencyObject
    {
        public static readonly DependencyProperty AllowOpenProperty = DependencyProperty.RegisterAttached(
            "AllowOpen",
            typeof(bool),
            typeof(DropFilesBehavior),
            new PropertyMetadata(false, OnAllowOpenChanged));

        public static bool GetAllowOpen(DependencyObject source)
        {
            return (bool)source.GetValue(AllowOpenProperty);
        }

        public static void SetAllowOpen(DependencyObject source, bool value)
        {
            source.SetValue(AllowOpenProperty, value);
        }

        private static void OnAllowOpenChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            UIElement uiElement = d as UIElement;
            uiElement.Drop -= UIElement_Drop;

            if ((bool)e.NewValue)
            {
                uiElement.Drop += UIElement_Drop;
            }
        }

        private static void UIElement_Drop(object sender, DragEventArgs e)
        {
            UIElement uiElement = sender as UIElement;

            // Sanity check just in case this was somehow send by something else
            if (uiElement == null)
                return;

            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] droppedFilePaths = e.Data.GetData(DataFormats.FileDrop, true) as string[];
                Task.Run(() => OpenDocument(droppedFilePaths));
            }
        }

        public static async Task OpenDocument(string[] droppedFilePaths)
        {
            var shell = IoC.Get<IShell>();
            var providers = IoC.GetAllInstances(typeof(IEditorProvider)).Cast<IEditorProvider>();

            foreach (var newPath in droppedFilePaths)
            {
                // Check if file type is supprted
                if (providers.FirstOrDefault(p => p.Handles(newPath)) == null)
                    continue;

                // Check if the document is already open
                bool foundInShell = false;
                foreach (var document in shell.Documents.OfType<PersistedDocument>().Where(d => !d.IsNew))
                {
                    if (string.IsNullOrEmpty(document.FilePath))
                        continue;

                    var docPath = Path.GetFullPath(document.FilePath);
                    if (string.Equals(newPath, docPath, System.StringComparison.OrdinalIgnoreCase))
                    {
                        shell.OpenDocument(document);
                        foundInShell = true;
                        break;
                    }
                }

                if (!foundInShell)
                {
                    shell.OpenDocument(await OpenFileCommandHandler.GetEditor(newPath));

                    // Add the file to the recent documents list
                    //shell.RecentFiles.Update(newPath);
                }
            }
        }

    }
}
