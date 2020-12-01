using System;
using Caliburn.Micro;

namespace Gemini.Framework.Commands
{
    public class Command : PropertyChangedBase
    {
        private readonly CommandDefinitionBase _commandDefinition;
        private bool _visible = true;
        private bool _enabled = true;
        private bool _checked;
        private string _text;
        private string _toolTip;
        private Uri _iconSource;

        public CommandDefinitionBase CommandDefinition
        {
            get { return _commandDefinition; }       
        }

        public bool Visible
        {
            get { return _visible; }
            set { Set(ref _visible, value); }
        }

        public bool Enabled
        {
            get { return _enabled; }
            set { Set(ref _enabled, value); }
        }

        public bool Checked
        {
            get { return _checked; }
            set { Set(ref _checked, value); }
        }

        public string Text
        {
            get { return _text; }
            set { Set(ref _text, value); }
        }

        public string ToolTip
        {
            get { return _toolTip; }
            set { Set(ref _toolTip, value); }
        }

        public Uri IconSource
        {
            get { return _iconSource; }
            set { Set(ref _iconSource, value); }
        }

        public object Tag { get; set; }

        public Command(CommandDefinitionBase commandDefinition)
        {
            _commandDefinition = commandDefinition;
            Text = commandDefinition.Text;
            ToolTip = commandDefinition.ToolTip;
            IconSource = commandDefinition.IconSource;
        }
    }
}