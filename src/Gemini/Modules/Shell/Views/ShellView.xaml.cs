using System.Windows;
using System.Windows.Controls;

namespace Gemini.Modules.Shell.Views
{
	/// <summary>
	/// Interaction logic for ShellView.xaml
	/// </summary>
	public partial class ShellView : Window, IShellView
	{
	    ToolBarTray IShellView.ToolBarTray
	    {
	        get { return ToolBarTray; }
	    }

	    public ShellView()
		{
			InitializeComponent();
		}
	}
}
