﻿/*
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

namespace Gemini.Framework.Themes
{
    [Export(typeof(ITheme))]
    public class LightTheme : ITheme
    {
        public virtual string Name
        {
            get { return Properties.Resources.ThemeLightName; }
        }

        public virtual IEnumerable<Uri> ApplicationResources
        {
            get
            {
                yield return new Uri("pack://application:,,,/Xceed.Wpf.AvalonDock.Themes.VS2013;component/LightTheme.xaml");
                yield return new Uri("pack://application:,,,/Gemini;component/Themes/VS2013/LightTheme.xaml");
            }
        }

        public virtual IEnumerable<Uri> MainWindowResources
        {
            get { yield break; }
        }
    }
}