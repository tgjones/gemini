using System;
using System.IO;
using System.Windows.Input;
using System.Windows.Media;
using Caliburn.Micro;

namespace Gemini.Framework
{
    public interface ILayoutItem : IScreen
    {
        Guid Id { get; }
        string ContentId { get; }
        ICommand CloseCommand { get; }
        ImageSource IconSource { get; }
        bool IsSelected { get; set; }
        bool ShouldReopenOnStart { get; }
        void LoadState(BinaryReader reader);
        void SaveState(BinaryWriter writer);
    }
}