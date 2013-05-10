using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Gemini.Modules.Inspector.Inspectors;
using Gemini.Modules.Inspector.Util;

namespace Gemini.Modules.Inspector
{
    public class InspectorBuilder<TBuilder>
        where TBuilder : InspectorBuilder<TBuilder>
    {
        private readonly List<IInspector> _inspectors;

        protected List<IInspector> Inspectors
        {
            get { return _inspectors; }
        }

        public InspectorBuilder()
        {
            _inspectors = new List<IInspector>();
        }

        public TBuilder WithCollapsibleGroup(string name, Func<CollapsibleGroupBuilder, CollapsibleGroupBuilder> callback)
        {
            var builder = new CollapsibleGroupBuilder();
            _inspectors.Add(callback(builder).ToCollapsibleGroup(name));
            return (TBuilder) this;
        }

        public TBuilder WithCheckBoxEditor<T, TProperty>(T instance, Expression<Func<T, TProperty>> propertyExpression)
        {
            return WithEditor<T, TProperty, CheckBoxEditorViewModel>(instance, propertyExpression);
        }

        public TBuilder WithColorEditor<T, TProperty>(T instance, Expression<Func<T, TProperty>> propertyExpression)
        {
            return WithEditor<T, TProperty, ColorEditorViewModel>(instance, propertyExpression);
        }

        public TBuilder WithEnumEditor<T, TProperty>(T instance, Expression<Func<T, TProperty>> propertyExpression)
        {
            return WithEditor<T, TProperty, EnumEditorViewModel<TProperty>>(instance, propertyExpression);
        }

        public TBuilder WithPoint3DEditor<T, TProperty>(T instance, Expression<Func<T, TProperty>> propertyExpression)
        {
            return WithEditor<T, TProperty, Point3DEditorViewModel>(instance, propertyExpression);
        }

        public TBuilder WithRangeEditor<T, TProperty>(T instance, Expression<Func<T, TProperty>> propertyExpression, double minimum, double maximum)
        {
            return WithEditor(instance, propertyExpression, new RangeEditorViewModel(minimum, maximum));
        }

        public TBuilder WithEditor<T, TProperty, TEditor>(T instance, Expression<Func<T, TProperty>> propertyExpression)
            where TEditor : IEditor, new()
        {
            return WithEditor(instance, propertyExpression, new TEditor());
        }

        public TBuilder WithEditor<T, TProperty, TEditor>(T instance, Expression<Func<T, TProperty>> propertyExpression, TEditor editor)
            where TEditor : IEditor
        {
            var propertyName = ExpressionUtility.GetPropertyName(propertyExpression);
            editor.BoundPropertyDescriptor = BoundPropertyDescriptor.FromProperty(instance, propertyName);
            _inspectors.Add(editor);
            return (TBuilder) this;
        }
    }
}