using System.Threading.Tasks;
using Caliburn.Micro;
using Gemini.Framework.Commands;
using Gemini.Framework.Results;

namespace Gemini.Modules.Toolbox.Commands
{
    [CommandHandler]
    public class ViewToolboxCommandHandler : CommandHandlerBase<ViewToolboxCommandDefinition>
    {
        public override async Task Run(Command command)
        {
            await Show.Tool<IToolbox>().ExecuteAsync();
        }
    }
}
