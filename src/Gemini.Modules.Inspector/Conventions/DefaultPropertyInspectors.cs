using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using Gemini.Modules.Inspector.Inspectors;

namespace Gemini.Modules.Inspector.Conventions
{
    public static class DefaultPropertyInspectors
    {
        private static readonly List<PropertyEditorBuilder> _inspectorBuilders;

        public static List<PropertyEditorBuilder> InspectorBuilders
        {
            get { return _inspectorBuilders; }
        }

        static DefaultPropertyInspectors()
        {
            _inspectorBuilders = new List<PropertyEditorBuilder>
            {
                new StandardPropertyEditorBuilder<bool, CheckBoxEditorViewModel>(),
                new StandardPropertyEditorBuilder<Color, ColorEditorViewModel>(),
                new StandardPropertyEditorBuilder<int, TextBoxEditorViewModel>(),
                new StandardPropertyEditorBuilder<Point3D, Point3DEditorViewModel>(),
                new StandardPropertyEditorBuilder<string, TextBoxEditorViewModel>(),

                new RangePropertyEditorBuilder(),
                new EnumPropertyEditorBuilder(),
            };
        }

        public static IEditor CreateEditor(PropertyDescriptor propertyDescriptor)
        {
            foreach (var inspectorBuilder in _inspectorBuilders)
            {
                if (inspectorBuilder.IsApplicable(propertyDescriptor))
                    return inspectorBuilder.BuildEditor(propertyDescriptor);
            }
            return null;
        }
    }
}