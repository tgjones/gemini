using System.ComponentModel.Composition;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interactivity;
using Gemini.Framework.Services;

namespace Gemini.Framework.Commands
{
    [Export(typeof(ICommandKeyGestureService))]
    public class CommandKeyGestureService : ICommandKeyGestureService
    {
        [ImportMany]
        private CommandDefinition[] _commandDefinitions;

        [Import]
        private ICommandService _commandService;

        public void BindKeyGestures(UIElement uiElement)
        {
            foreach (var commandDefinition in _commandDefinitions)
                if (commandDefinition.KeyGesture != null)
                    uiElement.InputBindings.Add(new InputBinding(
                        _commandService.GetTargetableCommand(_commandService.GetCommand(commandDefinition)),
                        commandDefinition.KeyGesture));
        }
    }
}