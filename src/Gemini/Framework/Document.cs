using System.Windows.Input;

namespace Gemini.Framework
{
	public abstract class Document : LayoutItemBase, IDocument
	{
		private ICommand _closeCommand;
		public ICommand CloseCommand
		{
			get
			{
				return _closeCommand ?? (_closeCommand = new RelayCommand(p => TryClose(null), p => true));
			}
		}

	}
}