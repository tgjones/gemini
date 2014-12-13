using System.Windows;

namespace Gemini.Framework.Commands
{
    public interface ICommandRouter
    {
        CommandHandlerWrapper GetCommandHandler(CommandDefinitionBase commandDefinition, IInputElement target);
    }
}