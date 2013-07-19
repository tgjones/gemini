using System.Windows;
using System.Windows.Media;

namespace Gemini.Framework.Services
{
    public interface IMainWindow
    {
        WindowState WindowState { get; set; }
        string Title { get; set; }
        ImageSource Icon { get; set; } 

        IShell Shell { get; }
    }
}