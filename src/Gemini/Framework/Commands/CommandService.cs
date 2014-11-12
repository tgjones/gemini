using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;

namespace Gemini.Framework.Commands
{
    [Export(typeof(ICommandService))]
    public class CommandService : ICommandService
    {
        private readonly Dictionary<Type, CommandDefinition> _commandDefinitionsLookup;
        private readonly Dictionary<CommandDefinition, Command> _commands;
        private readonly Dictionary<Command, TargetableCommand> _targetableCommands;
            
        [ImportMany]
        private CommandDefinition[] _commandDefinitions;

        public CommandService()
        {
            _commandDefinitionsLookup = new Dictionary<Type, CommandDefinition>();
            _commands = new Dictionary<CommandDefinition, Command>();
            _targetableCommands = new Dictionary<Command, TargetableCommand>();
        }

        public CommandDefinition GetCommandDefinition(Type commandDefinitionType)
        {
            CommandDefinition commandDefinition;
            if (!_commandDefinitionsLookup.TryGetValue(commandDefinitionType, out commandDefinition))
                commandDefinition = _commandDefinitionsLookup[commandDefinitionType] =
                    _commandDefinitions.First(x => x.GetType() == commandDefinitionType);
            return commandDefinition;
        }

        public Command GetCommand(CommandDefinition commandDefinition)
        {
            Command command;
            if (!_commands.TryGetValue(commandDefinition, out command))
                command = _commands[commandDefinition] = new Command(commandDefinition);
            return command;
        }

        public TargetableCommand GetTargetableCommand(Command command)
        {
            TargetableCommand targetableCommand;
            if (!_targetableCommands.TryGetValue(command, out targetableCommand))
                targetableCommand = _targetableCommands[command] = new TargetableCommand(command);
            return targetableCommand;
        }
    }
}