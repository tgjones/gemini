using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Caliburn.Micro;
using Gemini.Framework;
using Gemini.Properties;

namespace Gemini.Modules.Settings.ViewModels
{
    [Export(typeof (SettingsViewModel))]
    [PartCreationPolicy (CreationPolicy.NonShared)]
    public class SettingsViewModel : WindowBase
    {
        private IEnumerable<ISettingsEditorAsync> _settingsEditors;
        private SettingsPageViewModel _selectedPage;

        public SettingsViewModel()
        {
            DisplayName = Resources.SettingsDisplayName;
        }

        public List<SettingsPageViewModel> Pages { get; internal set; }

        public SettingsPageViewModel SelectedPage
        {
            get { return _selectedPage; }
            set
            {
                _selectedPage = value;
                NotifyOfPropertyChange(() => SelectedPage);
            }
        }

        protected override async Task OnInitializeAsync(CancellationToken cancellationToken)
        {
            await base.OnInitializeAsync(cancellationToken);

            var pages = new List<SettingsPageViewModel>();

            _settingsEditors = IoC.GetAll<ISettingsEditorAsync>().Concat(IoC.GetAll<ISettingsEditor>().Select(e => new SettingsEditorWrapper(e)));

            foreach (var settingsEditor in _settingsEditors)
            {
                var parentCollection = GetParentCollection(settingsEditor, pages);

                var page = parentCollection.FirstOrDefault(m => m.Name == settingsEditor.SettingsPageName);

                if (page == null)
                {
                    page = new SettingsPageViewModel { Name = settingsEditor.SettingsPageName };
                    parentCollection.Add(page);
                }

                page.Editors.Add(settingsEditor is SettingsEditorWrapper wrapper ? (object)wrapper.ViewModel : (object)settingsEditor);
            }

            Pages = pages;
            SelectedPage = GetFirstLeafPageRecursive(pages);
        }

        private static SettingsPageViewModel GetFirstLeafPageRecursive(List<SettingsPageViewModel> pages)
        {
            if (!pages.Any())
                return null;

            var firstPage = pages.First();
            if (!firstPage.Children.Any())
                return firstPage;

            return GetFirstLeafPageRecursive(firstPage.Children);
        }

        private List<SettingsPageViewModel> GetParentCollection(ISettingsEditorAsync settingsEditor,
            List<SettingsPageViewModel> pages)
        {
            if (string.IsNullOrEmpty(settingsEditor.SettingsPagePath))
            {
                return pages;
            }

            var path = settingsEditor.SettingsPagePath.Split(new[] {'\\'}, StringSplitOptions.RemoveEmptyEntries);

            foreach (var pathElement in path)
            {
                var page = pages.FirstOrDefault(s => s.Name == pathElement);

                if (page == null)
                {
                    page = new SettingsPageViewModel { Name = pathElement };
                    pages.Add(page);
                }

                pages = page.Children;
            }

            return pages;
        }

        public async Task SaveChanges()
        {
            foreach (var settingsEditor in _settingsEditors)
            {
                await settingsEditor.ApplyChangesAsync();
            }

            await TryCloseAsync(true);
        }

        public Task Cancel() => TryCloseAsync(false);
    }
}
