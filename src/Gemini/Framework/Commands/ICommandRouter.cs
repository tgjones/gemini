using System.Windows;

namespace Gemini.Framework.Commands
{
    public interface ICommandRouter
    {
        CommandHandler GetCommandHandler(CommandDefinition commandDefinition, IInputElement target);
    }
}