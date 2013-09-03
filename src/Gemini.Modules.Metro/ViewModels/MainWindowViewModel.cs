using System.ComponentModel.Composition;
using Gemini.Framework.Services;

namespace Gemini.Modules.Metro.ViewModels
{
    [Export(typeof(IMainWindow))]
    public class MainWindowViewModel : Gemini.Modules.MainWindow.ViewModels.MainWindowViewModel
    {
        
    }
}