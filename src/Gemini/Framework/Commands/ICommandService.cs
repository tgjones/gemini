using System;

namespace Gemini.Framework.Commands
{
    public interface ICommandService
    {
        CommandDefinitionBase GetCommandDefinition(Type commandDefinitionType);
        Command GetCommand(CommandDefinitionBase commandDefinition);
        TargetableCommand GetTargetableCommand(Command command);
    }
}