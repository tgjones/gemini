using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;
using System.Windows.Controls;
using System.Windows.Input;

namespace Gemini.Framework.Controls
{
    /// <summary>
    /// Button that automatically repeats when the user keeps pressing it.
    /// </summary>
    public class RepeatingButton : Button, IDisposable
    {
        /// <summary>
        /// How many times the button has repeated so far.
        /// </summary>
        public uint RepeatCount
        {
            get;
            private set;
        }

        /// <summary>
        /// A mapping of repetitions to intervals in milliseconds.
        /// When the number of repetitions in the dictionaries key is reached,
        /// the repetition timers speed is set to the value. This is used to
        /// make the repetitions go faster after a certain ammount of
        /// repetitions.
        /// </summary>
        public Dictionary<uint, uint> RepeatSpeed
        {
            get;
            set;
        }

        private Timer _loopTimer;

        public RepeatingButton()
        {
            RepeatSpeed = new Dictionary<uint, uint> {
                { 0, 300 },
                { 1, 250 },
                { 5, 100 },
                { 15, 10 },
                { 50, 5 }
            };

            PreviewMouseDown += RepeatingButton_MouseDown;
            PreviewMouseUp += RepeatingButton_MouseUp;

            _loopTimer = new Timer();
            _loopTimer.AutoReset = true;
            _loopTimer.Elapsed += _loopTimer_Elapsed;
        }

        private void RepeatingButton_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton != MouseButton.Left)
                return;

            e.Handled = true;

            OnClick();

            RepeatCount = 1;
            _loopTimer.Interval = RepeatSpeed.First().Value;
            _loopTimer.Enabled = true;
        }

        private void RepeatingButton_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton != MouseButton.Left)
                return;

            e.Handled = true;

            RepeatCount = 0;
            _loopTimer.Enabled = false;
        }

        private void _loopTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            RepeatCount++;

            if (RepeatSpeed.ContainsKey(RepeatCount))
                _loopTimer.Interval = RepeatSpeed[RepeatCount];

            Execute.OnUIThread(() => OnClick());
        }

        #region IDisposable Support

        private bool _disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    if (_loopTimer != null)
                    {
                        _loopTimer.Dispose();
                        _loopTimer = null;
                    }
                }

                _disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }

        #endregion
    }
}
