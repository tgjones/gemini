using System;
using System.Linq.Expressions;
using Gemini.Modules.Inspector.Xna.Inspectors;
using Microsoft.Xna.Framework;

namespace Gemini.Modules.Inspector.Xna
{
	public static class InspectorExtensions
	{
        public static TBuilder WithXnaColorEditor<TBuilder, T>(this InspectorBuilder<TBuilder> builder,
            T instance, Expression<Func<T, Color>> propertyExpression)
            where TBuilder : InspectorBuilder<TBuilder>
        {
            return builder.WithEditor<T, Color, XnaColorEditorViewModel>(instance, propertyExpression);
        }

		public static TBuilder WithVector3Editor<TBuilder, T>(this InspectorBuilder<TBuilder> builder,
			T instance, Expression<Func<T, Vector3>> propertyExpression)
			where TBuilder : InspectorBuilder<TBuilder>
		{
            return builder.WithEditor<T, Vector3, Vector3EditorViewModel>(instance, propertyExpression);
		}
	}
}