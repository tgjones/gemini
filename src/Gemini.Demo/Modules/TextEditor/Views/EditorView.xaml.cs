using System.Windows.Controls;
using System.Windows.Input;

namespace Gemini.Demo.Modules.TextEditor.Views
{
	public partial class EditorView : UserControl
	{
		public EditorView()
		{
			InitializeComponent();
            Loaded += (sender, e) => MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
		}
	}
}
