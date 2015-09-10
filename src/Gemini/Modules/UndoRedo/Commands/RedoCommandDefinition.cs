using System;
using System.ComponentModel.Composition;
using System.Windows.Input;
using Gemini.Framework.Commands;
using Gemini.Properties;

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
            get { return Resources.EditRedoCommandText; }
        }

        public override string ToolTip
        {
            get { return Resources.EditRedoCommandToolTip; }
        }

        public override Uri IconSource
        {
            get { return new Uri("pack://application:,,,/Gemini;component/Resources/Icons/Redo.png"); }
        }

        [Export]
        public static CommandKeyboardShortcut KeyGesture = new CommandKeyboardShortcut<RedoCommandDefinition>(new KeyGesture(Key.Y, ModifierKeys.Control));
    }
}