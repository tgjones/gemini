using System;
using System.ComponentModel.Composition;
using System.Windows.Input;
using Gemini.Framework.Commands;
using Gemini.Properties;

namespace Gemini.Modules.Shell.Commands
{
    [CommandDefinition]
    public class OpenFileCommandDefinition : CommandDefinition
    {
        public const string CommandName = "File.OpenFile";

        public override string Name
        {
            get { return CommandName; }
        }

        public override string Text
        {
            get { return Resources.FileOpenCommandText; }
        }

        public override string ToolTip
        {
            get { return Resources.FileOpenCommandToolTip; }
        }

        public override Uri IconSource
        {
            get { return new Uri("pack://application:,,,/Gemini;component/Resources/Icons/Open.png"); }
        }

        [Export]
        public static CommandKeyboardShortcut KeyGesture = new CommandKeyboardShortcut<OpenFileCommandDefinition>(new KeyGesture(Key.O, ModifierKeys.Control));
    }
}