using System.Windows.Input;
using Caliburn.Micro;

namespace Gemini.Framework
{
	public class Document : Screen
	{
		private ICommand _closeCommand;
		public ICommand CloseCommand
		{
			get
			{
				return _closeCommand ?? (_closeCommand = new RelayCommand(p => TryClose(), p =>
				{
					bool canClose = false;
					CanClose(r => canClose = r);
					return canClose;
				}));
			}
		}
	}
}