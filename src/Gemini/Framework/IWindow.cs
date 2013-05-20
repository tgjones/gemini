using System.Windows.Input;
using Caliburn.Micro;

namespace Gemini.Framework
{
    public interface IWindow : IActivate, IDeactivate, INotifyPropertyChangedEx
    {
        bool IsVisible { get; set; }
    }
}