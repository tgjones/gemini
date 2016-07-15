﻿using Gemini.Framework.Commands;
using Gemini.Properties;

namespace Gemini.Modules.Shell.Commands
{
    [CommandDefinition]
    public class RecentFilesCommandDefinition : CommandDefinition
    {
        public const string CommandName = "File.RecentFiles";

        public override string Name
        {
            get { return CommandName; }
        }

        public override string Text
        {
            get { return Resources.FileOpenRecentCommandText; }
        }

        public override string ToolTip
        {
            get { return Resources.FileOpenRecentCommandToolTip; }
        }
    }
}
