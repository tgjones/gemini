using Gemini.Framework.Commands;
using Gemini.Properties;
using System;

namespace Gemini.Modules.Shell.Commands
{
    [CommandDefinition]
    public class SaveAllFilesCommandDefinition : CommandDefinition
    {
        public const string CommandName = "File.SaveAllFiles";

        public override string Name
        {
            get { return CommandName; }
        }

        public override string Text
        {
            get { return Resources.FileSaveAllCommandText; }
        }

        public override string ToolTip
        {
            get { return Resources.FileSaveAllCommandToolTip; }
        }

        public override Uri IconSource
        {
            get { return new Uri("pack://application:,,,/Gemini;component/Resources/Icons/Saveall.png"); }
        }


        // TODO: there is a bug in case of multiple modifiers
        //[Export]
        //public static CommandKeyboardShortcut KeyGesture = new CommandKeyboardShortcut<SaveFileCommandDefinition>(new KeyGesture(Key.S, ModifierKeys.Control | ModifierKeys.Shift));
    }
}
