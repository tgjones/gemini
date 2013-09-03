using System;
using System.ComponentModel.Composition;
using System.Windows;
using Gemini.Framework;

namespace Gemini.Modules.Metro
{
	[Export(typeof(IModule))]
	public class Module : ModuleBase
	{
		public override void Initialize()
		{
            Shell.CurrentTheme = new ResourceDictionary
            {
                Source = new Uri("/Gemini;component/Themes/Metro/Theme.xaml", UriKind.Relative)
            };
		}
	}
}