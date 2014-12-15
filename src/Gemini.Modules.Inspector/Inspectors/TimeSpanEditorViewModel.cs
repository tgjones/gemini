using System;

namespace Gemini.Modules.Inspector.Inspectors
{
    public class TimeSpanEditorViewModel : EditorBase<TimeSpan?>, ILabelledInspector
    {
        private TimeSpan _timeInterval;

        /// <summary>
        /// Gets the TimeInterval.
        /// </summary>
        public TimeSpan TimeInterval
        {
            get { return _timeInterval; }
            private set
            {
                if (value == _timeInterval) return;
                _timeInterval = value;
                NotifyOfPropertyChange(() => TimeInterval);
            }
        }

        private string _formatString;

        /// <summary>
        /// Gets the FormatString.
        /// </summary>
        public string FormatString
        {
            get { return _formatString; }
            private set
            {
                if (string.Equals(value, _formatString)) return;
                _formatString = value;
                NotifyOfPropertyChange(() => FormatString);
            }
        }

        public TimeSpanEditorViewModel()
        {
            //These seem like sufficiently nominal settings.
            TimeInterval = new TimeSpan(0, 15, 0);
            FormatString = @"hh:mm:ss tt";
            //TODO: TBD: anything else that could be exposed through the VM?
        }
    }
}
