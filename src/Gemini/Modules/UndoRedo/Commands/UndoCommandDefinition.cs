using System;
using System.Windows.Input;
using Gemini.Framework.Commands;

namespace Gemini.Modules.UndoRedo.Commands
{
    [CommandDefinition]
    public class UndoCommandDefinition : CommandDefinition
    {
        public const string CommandName = "Edit.Undo";

        public override string Name
        {
            get { return CommandName; }
        }

        public override string Text
        {
            get { return "_Undo"; }
        }

        public override string ToolTip
        {
            get { return "Undo"; }
        }

        public override Uri IconSource
        {
            get { return new Uri("pack://application:,,,/Gemini;component/Resources/Icons/Undo.png"); }
        }

        public override KeyGesture KeyGesture
        {
            get { return new KeyGesture(Key.Z, ModifierKeys.Control); }
        }
    }
}