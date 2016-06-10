using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace Gemini.Framework.Controls
{
    /// <summary>
    /// Slider that exposes the thumb drag started / completed events directly
    /// to allow Caliburn Micro to attach to the messages.
    /// </summary>
    public class SliderEx : Slider
    {
        [Category("Behavior")]
        public event DragStartedEventHandler ThumbDragStarted;

        [Category("Behavior")]
        public event DragCompletedEventHandler ThumbDragCompleted;

        protected override void OnThumbDragStarted(DragStartedEventArgs e)
        {
            if (ThumbDragStarted != null)
                ThumbDragStarted(this, e);

            base.OnThumbDragStarted(e);
        }

        protected override void OnThumbDragCompleted(DragCompletedEventArgs e)
        {
            if (ThumbDragCompleted != null)
                ThumbDragCompleted(this, e);

            base.OnThumbDragCompleted(e);
        }
    }
}
