using System;
using System.ComponentModel.Composition;

namespace Gemini.Framework.Commands
{
    [MetadataAttribute]
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class CommandHandlerAttribute : ExportAttribute, ICommandHandlerMetadata
    {
        public CommandHandlerAttribute(Type commandDefinitionType)
            : base(typeof(CommandHandler))
        {
            CommandDefinitionType = commandDefinitionType;
        }

        public Type CommandDefinitionType { get; set; }
    }
}