using System.Windows;
using System.Windows.Media;

namespace Gemini.Framework.Services
{
    public interface IMainWindow
    {
        WindowState WindowState { get; set; }
        double Width { get; set; }
        double Height { get; set; }

        string Title { get; set; }
        ImageSource Icon { get; set; } 

        IShell Shell { get; }
    }
}