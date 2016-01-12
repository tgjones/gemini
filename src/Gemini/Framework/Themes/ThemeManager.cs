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

        private readonly ITheme[] _themes;

        public IEnumerable<ITheme> Themes
        {
            get { return _themes; }
        }

        public ITheme CurrentTheme { get; private set; }

        [ImportingConstructor]
        public ThemeManager([ImportMany] ITheme[] themes)
        {
            _themes = themes;
            _settingsEventManager.AddListener(s => s.ThemeName, value => SetCurrentTheme(value));
        }

        public bool SetCurrentTheme(string name)
        {
            var theme = _themes.FirstOrDefault(x => x.Name == name);
            if (theme == null)
                return false;

            var mainWindow = Application.Current.MainWindow;
            if (mainWindow == null)
                return false;

            CurrentTheme = theme;

            if (_applicationResourceDictionary == null)
            {
                _applicationResourceDictionary = new ResourceDictionary();
                Application.Current.Resources.MergedDictionaries.Add(_applicationResourceDictionary);
            }
            _applicationResourceDictionary.BeginInit();
            _applicationResourceDictionary.MergedDictionaries.Clear();

            var windowResourceDictionary = mainWindow.Resources.MergedDictionaries[0];
            windowResourceDictionary.BeginInit();
            windowResourceDictionary.MergedDictionaries.Clear();

            foreach (var uri in theme.ApplicationResources)
                _applicationResourceDictionary.MergedDictionaries.Add(new ResourceDictionary
                {
                    Source = uri
                });

            foreach (var uri in theme.MainWindowResources)
                windowResourceDictionary.MergedDictionaries.Add(new ResourceDictionary
                {
                    Source = uri
                });

            _applicationResourceDictionary.EndInit();
            windowResourceDictionary.EndInit();

            RaiseCurrentThemeChanged(EventArgs.Empty);

            return true;
        }

        private void RaiseCurrentThemeChanged(EventArgs args)
        {
            var handler = CurrentThemeChanged;
            if (handler != null)
                handler(this, args);
        }
    }
}