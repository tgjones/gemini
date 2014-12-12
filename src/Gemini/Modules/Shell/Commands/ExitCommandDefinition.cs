using System.Windows.Input;
using Gemini.Framework.Commands;

namespace Gemini.Modules.Shell.Commands
{
    [CommandDefinition]
    public class ExitCommandDefinition : CommandDefinition
    {
        public const string CommandName = "File.Exit";

        public override string Name
        {
            get { return CommandName; }
        }

        public override string Text
        {
            get { return "E_xit"; }
        }

        public override string ToolTip
        {
            get { return "Exit"; }
        }

        public override KeyGesture KeyGesture
        {
            get { return new KeyGesture(Key.F4, ModifierKeys.Alt); }
        }
    }
}