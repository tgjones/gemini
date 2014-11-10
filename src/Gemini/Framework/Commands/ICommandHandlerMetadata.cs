using System;

namespace Gemini.Framework.Commands
{
    public interface ICommandHandlerMetadata
    {
        Type CommandDefinitionType { get; }
    }
}