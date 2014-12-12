using System;
using System.Windows.Input;
using Gemini.Framework.Commands;

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
            get { return "_Open"; }
        }

        public override string ToolTip
        {
            get { return "Open"; }
        }

        public override Uri IconSource
        {
            get { return new Uri("pack://application:,,,/Gemini;component/Resources/Icons/Open.png"); }
        }

        public override KeyGesture KeyGesture
        {
            get { return new KeyGesture(Key.O, ModifierKeys.Control); }
        }
    }
}