using Gemini.Framework.Commands;
using Gemini.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gemini.Modules.Shell.Commands
{
    [CommandDefinition]
    public class OpenRecentCommandListDefinition : CommandListDefinition
    {
        public const string CommandName = "File.OpenRecent";

        public override string Name
        {
            get { return CommandName; }
        }
    }
}
