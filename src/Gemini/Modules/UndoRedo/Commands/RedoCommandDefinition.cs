using System;
using System.Windows.Input;
using Gemini.Framework.Commands;

namespace Gemini.Modules.UndoRedo.Commands
{
    [CommandDefinition]
    public class RedoCommandDefinition : CommandDefinition
    {
        public const string CommandName = "Edit.Redo";

        public override string Name
        {
            get { return CommandName; }
        }

        public override string Text
        {
            get { return "_Redo"; }
        }

        public override string ToolTip
        {
            get { return "Redo"; }
        }

        public override Uri IconSource
        {
            get { return new Uri("pack://application:,,,/Gemini;component/Resources/Icons/Redo.png"); }
        }

        public override KeyGesture KeyGesture
        {
            get { return new KeyGesture(Key.Y, ModifierKeys.Control); }
        }
    }
}