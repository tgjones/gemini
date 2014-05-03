using System.Windows.Controls;
using Gemini.Modules.Inspector.Controls;
using Gemini.Modules.Inspector.MonoGame.Converters;

namespace Gemini.Modules.Inspector.MonoGame.Inspectors
{
    /// <summary>
    /// Interaction logic for XnaColorEditorView.xaml
    /// </summary>
    public partial class XnaColorEditorView : UserControl
    {
        public XnaColorEditorView()
        {
            InitializeComponent();
        }

        private void OnScreenColorPickerColorHovered(object sender, ColorEventArgs e)
        {
            ((XnaColorEditorViewModel) DataContext).Value = XnaColorToColorConverter.Convert(e.Color);
        }

        private void OnScreenColorPickerColorPicked(object sender, ColorEventArgs e)
        {
            ((XnaColorEditorViewModel) DataContext).Value = XnaColorToColorConverter.Convert(e.Color);
        }
    }
}
