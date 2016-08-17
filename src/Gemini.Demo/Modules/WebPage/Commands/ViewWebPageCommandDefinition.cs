using Gemini.Framework.Commands;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Gemini.Demo.Modules.WebPage.Commands
{
    [CommandDefinition]
    public class ViewWebPageCommandDefinition : CommandDefinition
    {
        public const string CommandName = "View.WebBrowser";

        public override string Name
        {
            get { return CommandName; }
        }

        public override string Text
        {
            get { return "Web Browser"; }
        }

        public override string ToolTip
        {
            get { return "Web Browser"; }
        }

        public override Uri IconSource
        {
            get
            {
                { return new Uri("pack://application:,,,/Gemini.Demo;component/Resources/WebBrowser.png"); }
            }
        }

        [Export]
        public static CommandKeyboardShortcut KeyGesture = new CommandKeyboardShortcut<ViewWebPageCommandDefinition>(new KeyGesture(Key.W, ModifierKeys.Control));
    }
}
