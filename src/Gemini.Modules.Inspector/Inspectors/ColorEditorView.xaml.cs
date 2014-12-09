using System;
using System.Windows.Controls;
using System.Windows.Media;
using Gemini.Modules.Inspector.Controls;

namespace Gemini.Modules.Inspector.Inspectors
{
    public partial class ColorEditorView : UserControl
    {
        private Color _originalColor;

        public ColorEditorView()
        {
            InitializeComponent();
        }

        private void OnScreenColorPickerPickingStarted(object sender, EventArgs e)
        {
            _originalColor = ((ColorEditorViewModel) DataContext).Value;
        }

        private void OnScreenColorPickerPickingCancelled(object sender, EventArgs e)
        {
            ((ColorEditorViewModel) DataContext).Value = _originalColor;
        }

        private void OnScreenColorPickerColorHovered(object sender, ColorEventArgs e)
        {
            ((ColorEditorViewModel) DataContext).Value = e.Color;
        }

        private void OnScreenColorPickerColorPicked(object sender, ColorEventArgs e)
        {
            ((ColorEditorViewModel) DataContext).Value = e.Color;
        }
    }
}
