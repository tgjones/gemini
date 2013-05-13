using System.Windows.Controls;
using Gemini.Modules.Inspector.Controls;

namespace Gemini.Modules.Inspector.Inspectors
{
    public partial class ColorEditorView : UserControl
    {
        public ColorEditorView()
        {
            InitializeComponent();
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
