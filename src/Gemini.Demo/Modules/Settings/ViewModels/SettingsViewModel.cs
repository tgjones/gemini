using System.ComponentModel;
using System.ComponentModel.Composition;
using Gemini.Framework;


namespace Gemini.Demo.Modules.Settings.ViewModels
{
	[Export(typeof(SettingsViewModel))]
	public class SettingsViewModel : Window
	{

	}
}