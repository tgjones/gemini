/*
 * Original source code from the Wide framework:
 * https://github.com/chandramouleswaran/Wide
 * 
 * Used in Gemini with kind permission of the author.
 *
 * Original licence follows:
 *
 * Copyright (c) 2013 Chandramouleswaran Ravichandran
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
 */

using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows;
using Gemini.Framework.Services;

namespace Gemini.Framework.Themes
{
    [Export(typeof(IThemeManager))]
    public class ThemeManager : IThemeManager
    {
        public event EventHandler CurrentThemeChanged;

        private readonly SettingsPropertyChangedEventManager<Properties.Settings> _settingsEventManager =
            new SettingsPropertyChangedEventManager<Properties.Settings>(Properties.Settings.Default);

        private ResourceDictionary _applicationResourceDictionary;

        public List<ITheme> Themes
        {
            get; private set;
        }

        public ITheme CurrentTheme { get; private set; }

        [ImportingConstructor]
        public ThemeManager([ImportMany] ITheme[] themes)
        {
            Themes = new List<ITheme>(themes);
            _settingsEventManager.AddListener(s => s.ThemeName, value => SetCurrentTheme(value));
        }

        public bool SetCurrentTheme(string name)
        {
            var theme = Themes.FirstOrDefault(x => x.GetType().Name == name);
            if (theme == null)
                return false;

            var mainWindow = Application.Current.MainWindow;
            if (mainWindow == null)
                return false;

            if (CurrentTheme == theme)
            {
                return true; // Nothing to do, avoid full repaint of mainwindow
            }

            var applicationResources = CreateApplicationResources(theme);
            var windowResources = CreateUriResources(theme.MainWindowResources);
            var windowResourceDictionary =
                mainWindow.Resources.MergedDictionaries[0];

            bool applicationDictionaryCreated = false;
            if (_applicationResourceDictionary == null)
            {
                _applicationResourceDictionary = new ResourceDictionary();
                Application.Current.Resources.MergedDictionaries.Add(_applicationResourceDictionary);
                applicationDictionaryCreated = true;
            }

            var previousApplicationResources =
                _applicationResourceDictionary.MergedDictionaries.ToArray();
            var previousWindowResources =
                windowResourceDictionary.MergedDictionaries.ToArray();

            try
            {
                ReplaceMergedDictionaries(
                    _applicationResourceDictionary,
                    applicationResources);
                ReplaceMergedDictionaries(
                    windowResourceDictionary,
                    windowResources);
            }
            catch
            {
                ReplaceMergedDictionaries(
                    _applicationResourceDictionary,
                    previousApplicationResources);
                ReplaceMergedDictionaries(
                    windowResourceDictionary,
                    previousWindowResources);
                if (applicationDictionaryCreated)
                {
                    Application.Current.Resources.MergedDictionaries.Remove(
                        _applicationResourceDictionary);
                    _applicationResourceDictionary = null;
                }
                throw;
            }

            CurrentTheme = theme;
            RaiseCurrentThemeChanged(EventArgs.Empty);

            return true;
        }

        private static ResourceDictionary[] CreateApplicationResources(ITheme theme)
        {
            var resources = new List<ResourceDictionary>();
            var avalonDockResources =
                BuiltInThemeResources.CreateAvalonDockDictionary(theme);
            if (avalonDockResources != null)
                resources.Add(avalonDockResources);
            resources.AddRange(CreateUriResources(theme.ApplicationResources));
            return resources.ToArray();
        }

        private static ResourceDictionary[] CreateUriResources(
            IEnumerable<Uri> resourceUris)
        {
            return resourceUris
                .Select(uri => new ResourceDictionary { Source = uri })
                .ToArray();
        }

        private static void ReplaceMergedDictionaries(
            ResourceDictionary owner,
            IEnumerable<ResourceDictionary> resources)
        {
            owner.BeginInit();
            try
            {
                owner.MergedDictionaries.Clear();
                foreach (var resource in resources)
                    owner.MergedDictionaries.Add(resource);
            }
            finally
            {
                owner.EndInit();
            }
        }

        private void RaiseCurrentThemeChanged(EventArgs args)
        {
            var handler = CurrentThemeChanged;
            if (handler != null)
                handler(this, args);
        }
    }
}
