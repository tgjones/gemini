using System;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows;

namespace Gemini.Framework.Commands
{
    [Export(typeof(ICommandRouter))]
    public class CommandRouter : ICommandRouter
    {
        private readonly Lazy<CommandHandler, ICommandHandlerMetadata>[] _commandHandlers;

        [ImportingConstructor]
        public CommandRouter(
            [ImportMany(typeof(CommandHandler))] 
            Lazy<CommandHandler, ICommandHandlerMetadata>[] commandHandlers)
        {
            _commandHandlers = commandHandlers;
        }

        public CommandHandler GetCommandHandler(CommandDefinition commandDefinition, IInputElement target)
        {
            // TODO: We could look at the currently focused element, and iterate up through
            // the tree, giving each DataContext a chance to handle for the command.
            // For now, there are only global handlers.
            var globalHandler = _commandHandlers.FirstOrDefault(x => x.Metadata.CommandDefinitionType == commandDefinition.GetType());
            if (globalHandler == null)
                return null;

            return globalHandler.Value;
        }
    }
}