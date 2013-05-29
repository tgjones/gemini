using Caliburn.Micro;
using Gemini.Framework;

namespace Gemini.Modules.ErrorList
{
    public interface IErrorList : ITool
    {
        IObservableCollection<ErrorListItem> Items { get; } 
    }
}