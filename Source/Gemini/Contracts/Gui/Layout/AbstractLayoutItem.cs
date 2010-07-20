using System;
using System.ComponentModel;
using System.Windows;
using Gemini.Contracts.Services.ExtensionService;
using Gemini.Contracts.Utilities;

namespace Gemini.Contracts.Gui.Layout
{
	public abstract class AbstractLayoutItem : AbstractExtension, ILayoutItem
	{
		#region " ILayoutItem Implementation "

		#region " Name "
		/// <summary>
		/// Used to uniquely identify the layout item.
		/// </summary>
		public string Name
		{
			get
			{
				return m_Name;
			}
			protected set
			{
				if (value == null)
				{
					throw new ArgumentNullException();
				}
				if (m_Name != value)
				{
					m_Name = value;
					NotifyPropertyChanged(m_NameArgs);
				}
			}
		}
		private string m_Name = string.Empty;
		static readonly PropertyChangedEventArgs m_NameArgs =
				NotifyPropertyChangedHelper.CreateArgs<AbstractLayoutItem>(o => o.Name);

		#endregion

		#region " Title "
		/// <summary>
		/// Shows up as a title of the layout item.
		/// </summary>
		public string Title
		{
			get
			{
				return m_Title;
			}
			protected set
			{
				if (value == null)
				{
					throw new ArgumentNullException();
				}
				if (m_Title != value)
				{
					m_Title = value;
					NotifyPropertyChanged(m_TitleArgs);
				}
			}
		}
		private string m_Title = string.Empty;
		static readonly PropertyChangedEventArgs m_TitleArgs =
				NotifyPropertyChangedHelper.CreateArgs<AbstractLayoutItem>(o => o.Title);

		#endregion

		public virtual void OnGotFocus(object sender, RoutedEventArgs e) { }
		public virtual void OnLostFocus(object sender, RoutedEventArgs e) { }

		#endregion

	}
}