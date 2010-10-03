using Gemini.Framework;

namespace Gemini.Demo.Modules.Home.ViewModels
{
	public class HomeViewModel : Screen
    {
        public override string DisplayName
        {
            get { return "Home"; }
        }

        public override bool Equals(object obj)
        {
            var other = obj as HomeViewModel;
            return other != null;
        }
    }
}