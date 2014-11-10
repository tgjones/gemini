namespace Gemini.Framework.Commands
{
    public interface ICommandUiItem
    {
        CommandDefinition CommandDefinition { get; }
        void Update(CommandHandler commandHandler);
    }
}