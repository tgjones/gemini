using System;
using System.Linq.Expressions;
using Gemini.Modules.Inspector.Xna.Inspectors;

namespace Gemini.Modules.Inspector.Xna
{
	public static class InspectorExtensions
	{
		public static TBuilder WithVector3Editor<TBuilder, T, TProperty>(this InspectorBuilder<TBuilder> builder,
			T instance, Expression<Func<T, TProperty>> propertyExpression)
			where TBuilder : InspectorBuilder<TBuilder>
		{
			return builder.WithEditor<T, TProperty, Vector3EditorViewModel>(instance, propertyExpression);
		}
	}
}