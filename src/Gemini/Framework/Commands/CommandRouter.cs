using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows;

namespace Gemini.Framework.Commands
{
    [Export(typeof(ICommandRouter))]
    public class CommandRouter : ICommandRouter
    {
        private readonly Dictionary<Type, CommandHandlerWrapper> _globalCommandHandlerWrappers;

        [ImportingConstructor]
        public CommandRouter(
            [ImportMany(typeof(ICommandHandler))] 
            ICommandHandler[] globalCommandHandlers)
        {
            _globalCommandHandlerWrappers = BuildCommandHandlerWrappers(globalCommandHandlers);
        }

        private static Dictionary<Type, CommandHandlerWrapper> BuildCommandHandlerWrappers(ICommandHandler[] commandHandlers)
        {
            // Command handlers are either ICommandHandler<T> or ICommandListHandler<T>.
            // We need to extract T, and use it as the key in our dictionary.

            var commandHandlerType = typeof(ICommandHandler<>);
            var commandListHandlerType = typeof(ICommandListHandler<>);

            var result = new Dictionary<Type, CommandHandlerWrapper>();

            foreach (var commandHandler in commandHandlers)
            {
                Type commandHandlerInterfaceType, commandDefinitionType;

                if (ImplementsCommandHandlerType(commandHandler.GetType(), commandHandlerType,
                    out commandHandlerInterfaceType, out commandDefinitionType))
                {
                    result[commandDefinitionType] = CommandHandlerWrapper.FromCommandHandler(
                        commandHandlerInterfaceType, commandHandler);
                    continue;
                }

                if (ImplementsCommandHandlerType(commandHandler.GetType(), commandListHandlerType,
                    out commandHandlerInterfaceType, out commandDefinitionType))
                {
                    result[commandDefinitionType] = CommandHandlerWrapper.FromCommandListHandler(
                        commandHandlerInterfaceType, commandHandler);
                    continue;
                }
            }

            return result;
        }

        private static bool ImplementsCommandHandlerType(
            Type type, Type genericInterfaceType, 
            out Type commandHandlerInterfaceType,
            out Type commandDefinitionType)
        {
            while (type != null)
            {
                var foundInterface = type.GetInterfaces().FirstOrDefault(x => 
                    x.IsGenericType && x.GetGenericTypeDefinition() == genericInterfaceType);

                if (foundInterface != null)
                {
                    commandHandlerInterfaceType = foundInterface;
                    commandDefinitionType = foundInterface.GetGenericArguments().First();
                    return true;
                }

                type = type.BaseType;
            }

            commandHandlerInterfaceType = null;
            commandDefinitionType = null;
            return false;
        }

        public CommandHandlerWrapper GetCommandHandler(CommandDefinitionBase commandDefinition, IInputElement target)
        {
            // TODO: We could look at the currently focused element, and iterate up through
            // the tree, giving each DataContext a chance to handle for the command.
            // For now, there are only global handlers.
            CommandHandlerWrapper commandHandler;
            if (!_globalCommandHandlerWrappers.TryGetValue(commandDefinition.GetType(), out commandHandler))
                return null;

            return commandHandler;
        }
    }
}