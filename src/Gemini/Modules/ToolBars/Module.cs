using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Windows;
using Gemini.Framework;

namespace Gemini.Modules.ToolBars
{
    [Export(typeof(IModule))]
    public class Module : ModuleBase
    {
        public override IEnumerable<ResourceDictionary> GlobalResourceDictionaries
        {
            get
            {
                yield return new ResourceDictionary
                {
                    Source = new Uri("/Gemini;component/Modules/ToolBars/Resources/Styles.xaml", UriKind.Relative)
                };
            }
        }
    }
}