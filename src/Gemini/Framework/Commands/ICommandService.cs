using System;

namespace Gemini.Framework.Commands
{
    public interface ICommandService
    {
        CommandDefinition GetCommandDefinition(Type commandDefinitionType);
        Command GetCommand(CommandDefinition commandDefinition);
        TargetableCommand GetTargetableCommand(Command command);
    }
}