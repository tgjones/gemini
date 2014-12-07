using System.Windows;
using System.Windows.Controls;
using Gemini.Modules.Inspector.Inspectors;

namespace Gemini.Modules.Inspector.Controls
{
    public class InspectorItemTemplateSelector : DataTemplateSelector
    {
        public DataTemplate LabelledTemplate { get; set; }
        public DataTemplate DefaultTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item is ILabelledInspector)
                return LabelledTemplate;
            return DefaultTemplate;
        }
    }
}