using System;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Reflection;

namespace Gemini.Contracts.Utilities
{
	/// <summary>
	/// This class is used along with classes that implement or hook into
	/// the INotifyPropertyChanged interface.  This class lets you use
	/// reflection to create and cache the PropertyChangedEventArgs object
	/// or the Property Name (string) at runtime rather than hard coding
	/// property names throughout the program.
	/// 
	/// Design goals:
	///  - Improve maintainability by making everything strongly typed
	///  - Improve performance by caching the PropertyChangedEventArgs
	/// 
	/// Based on ideas found here, but rewritten to include GetPropertyName:
	/// http://compositeextensions.codeplex.com/Thread/View.aspx?ThreadId=53731
	/// </summary>
	public static class NotifyPropertyChangedHelper
	{
		/// <summary>
		/// Use this to create and cache a PropertyChangedEventArgs object
		/// as a static member of a class which you can use as the 
		/// parameter to the PropertyChanged event.  Usage:
		/// 
		/// static readonly PropertyChangedEventArgs m_$PropertyName$Args = 
		///     NotifyPropertyChangedHelper.CreateArgs<$ClassName$>(o => o.$PropertyName$);
		/// 
		/// In your property setter:
		///     PropertyChanged(this, m_$PropertyName$Args)
		/// 
		/// </summary>
		/// <typeparam name="T">The type that has the property</typeparam>
		/// <param name="expression"></param>
		/// <returns>A PropertyChangedEventArgs object for caching</returns>
		public static PropertyChangedEventArgs CreateArgs<T>(Expression<Func<T, object>> expression)
		{
			return new PropertyChangedEventArgs(GetPropertyName<T>(expression));
		}

		/// <summary>
		/// Use this to create and cache a string of the property name
		/// as a static member of a class which you can use to 
		/// compare against the PropertyChangedEventArgs.PropertyName
		/// In a PropertyChanged event handler.  Usage:
		/// 
		/// static readonly string m_$PropertyName$Name = 
		///     NotifyPropertyChangedHelper.GetPropertyName<$ClassName$>(o => o.$PropertyName$);
		/// 
		/// In your PropertyChanged event handler:
		///     if (e.PropertyName == m_$PropertyName$Name)
		/// 
		/// </summary>
		/// <typeparam name="T">The type that has the property</typeparam>
		/// <param name="expression"></param>
		/// <returns>A PropertyChangedEventArgs object for caching</returns>
		public static string GetPropertyName<T>(Expression<Func<T, object>> expression)
		{
			PropertyInfo propertyInfo = GetPropertyInfo<T>(expression);
			return propertyInfo.Name;
		}

		private static PropertyInfo GetPropertyInfo<T>(Expression<Func<T, object>> expression)
		{
			var lambda = expression as LambdaExpression;
			MemberExpression memberExpression;
			if (lambda.Body is UnaryExpression)
			{
				var unaryExpression = lambda.Body as UnaryExpression;
				memberExpression = unaryExpression.Operand as MemberExpression;
			}
			else
			{
				memberExpression = lambda.Body as MemberExpression;
			}

			return memberExpression.Member as PropertyInfo;
		}
	}
}