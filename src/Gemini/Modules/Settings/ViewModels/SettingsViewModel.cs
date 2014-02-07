using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows.Input;
using Caliburn.Micro;
using Gemini.Framework;

namespace Gemini.Modules.Settings.ViewModels
{
    [Export(typeof (SettingsViewModel))]
    public class SettingsViewModel : WindowBase
    {
        private IEnumerable<ISettingsEditor> _settingsEditors;

        public SettingsViewModel()
        {
            CancelCommand = new RelayCommand(o => TryClose(false));
            OkCommand = new RelayCommand(SaveChanges);

            DisplayName = "Options";
        }

        public List<SettingsPageViewModel> Pages { get; private set; }
        public ICommand CancelCommand { get; private set; }
        public ICommand OkCommand { get; private set; }

        protected override void OnInitialize()
        {
            base.OnInitialize();

            var pages = new List<SettingsPageViewModel>();
            _settingsEditors = IoC.GetAll<ISettingsEditor>();

            foreach (ISettingsEditor settingsEditor in _settingsEditors)
            {
                List<SettingsPageViewModel> parentCollection = GetParentCollection(settingsEditor, pages);

                SettingsPageViewModel existingPage =
                    parentCollection.FirstOrDefault(m => m.Name == settingsEditor.SettingsPageName);

                if (existingPage != null && existingPage.Editor == null)
                {
                    existingPage.Editor = settingsEditor;
                }
                else
                {
                    parentCollection.Add(new SettingsPageViewModel
                    {
                        Editor = settingsEditor,
                        Name = settingsEditor.SettingsPageName
                    });
                }
            }

            Pages = pages;
        }

        private List<SettingsPageViewModel> GetParentCollection(ISettingsEditor settingsEditor,
            List<SettingsPageViewModel> pages)
        {
            if (string.IsNullOrEmpty(settingsEditor.SettingsPagePath))
            {
                return pages;
            }

            string[] path = settingsEditor.SettingsPagePath.Split(new[] {'\\'}, StringSplitOptions.RemoveEmptyEntries);

            foreach (string pathElement in path)
            {
                SettingsPageViewModel page = pages.FirstOrDefault(s => s.Name == pathElement);

                if (page == null)
                {
                    page = new SettingsPageViewModel {Name = pathElement};
                    pages.Add(page);
                }

                pages = page.Children;
            }

            return pages;
        }

        private void SaveChanges(object obj)
        {
            foreach (ISettingsEditor settingsEditor in _settingsEditors)
            {
                settingsEditor.ApplyChanges();
            }

            TryClose(true);
        }
    }
}