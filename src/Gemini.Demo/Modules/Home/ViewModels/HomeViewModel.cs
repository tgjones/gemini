using System.ComponentModel.Composition;
using Caliburn.Micro;

namespace Gemini.Demo.Modules.Home.ViewModels
{
	[Export(typeof(HomeViewModel))]
	public class HomeViewModel : Screen
	{
		public override string DisplayName
		{
			get { return "Home"; }
		}
	}
}