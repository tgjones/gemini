using System.Windows;

namespace Gemini.Framework.Commands
{
    public interface ICommandRouter
    {
        CommandHandler GetCommandHandler(CommandDefinitionBase commandDefinition, IInputElement target);
    }
}