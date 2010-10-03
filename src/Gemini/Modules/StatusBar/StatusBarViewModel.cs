using Caliburn.Core;
using Gemini.Framework.Services;

namespace Gemini.Modules.StatusBar
{
	public class StatusBarViewModel : PropertyChangedBase, IStatusBar
	{
		private string _message;

		public string Message
		{
			get { return _message; }
			set
			{
				_message = value;
				NotifyOfPropertyChange("Message");
			}
		}
	}
}