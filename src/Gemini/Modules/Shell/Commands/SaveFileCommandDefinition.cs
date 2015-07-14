using System;
using System.Windows.Input;
using Gemini.Framework.Commands;

namespace Gemini.Modules.Shell.Commands
{
    [CommandDefinition]
    public class SaveFileCommandDefinition : CommandDefinition
    {
        public const string CommandName = "File.SaveFile";

        public override string Name
        {
            get { return CommandName; }
        }

        public override string Text
        {
            get { return "_Save"; }
        }

        public override string ToolTip
        {
            get { return "Save"; }
        }

        public override Uri IconSource
        {
            get { return new Uri("pack://application:,,,/Gemini;component/Resources/Icons/Save.png"); }
        }

        public override KeyGesture KeyGesture
        {
            get { return new KeyGesture(Key.S, ModifierKeys.Control); }
        }
    }
}