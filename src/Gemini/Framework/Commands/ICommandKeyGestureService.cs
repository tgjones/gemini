using System.Windows;
using System.Windows.Input;

namespace Gemini.Framework.Commands
{
    public interface ICommandKeyGestureService
    {
        void BindKeyGestures(UIElement uiElement);
        KeyGesture GetPrimaryKeyGesture(CommandDefinitionBase commandDefinition);
    }
}