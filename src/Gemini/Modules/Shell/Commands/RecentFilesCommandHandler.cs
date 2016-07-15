using Gemini.Framework.Commands;
using Gemini.Framework.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gemini.Modules.Shell.Commands
{
    [CommandHandler]
    public class RecentFilesCommandHandler : CommandHandlerBase<RecentFilesCommandDefinition>
    {
        private readonly IShell _shell;

        [ImportingConstructor]
        public RecentFilesCommandHandler(IShell shell)
        {
            _shell = shell;
        }

        public override void Update(Command command)
        {
            command.Enabled = (_shell.RecentFiles.Items.Count > 0);
        }

        public override Task Run(Command command)
        {
            return null;
        }
    }
}
