using System.ComponentModel.Composition;
using System.Windows;
using System.Windows.Input;

namespace Gemini.Framework.Commands
{
    [Export(typeof(ICommandKeyGestureService))]
    public class CommandKeyGestureService : ICommandKeyGestureService
    {
        private readonly CommandDefinitionBase[] _commandDefinitions;
        private readonly ICommandService _commandService;

        [ImportingConstructor]
        public CommandKeyGestureService(
            [ImportMany] CommandDefinitionBase[] commandDefinitions,
            ICommandService commandService)
        {
            _commandDefinitions = commandDefinitions;
            _commandService = commandService;
        }

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