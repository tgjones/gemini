using System.Threading.Tasks;
using Caliburn.Micro;
using Gemini.Demo.Modules.Home.ViewModels;
using Gemini.Framework.Commands;
using Gemini.Framework.Results;

namespace Gemini.Demo.Modules.Home.Commands
{
    [CommandHandler]
    public class ViewHomeCommandHandler : CommandHandlerBase<ViewHomeCommandDefinition>
    {
        public override async Task Run(Command command)
        {
            await Show.Document<HomeViewModel>().ExecuteAsync();
        }
    }
}
