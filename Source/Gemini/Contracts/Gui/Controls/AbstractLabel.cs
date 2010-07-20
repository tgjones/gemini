using System;
using System.ComponentModel;
using Gemini.Contracts.Utilities;

namespace Gemini.Contracts.Gui.Controls
{
	public abstract class AbstractLabel : AbstractControl, ILabel
	{

		#region " Text "
		/// <summary>
		/// This is the text displayed in the label itself.
		/// Best to set this property in the derived class's constructor.
		/// </summary>
		public string Text
		{
			get
			{
				return m_Text;
			}
			protected set
			{
				if (value == null)
				{
					throw new ArgumentNullException();
				}
				if (m_Text != value)
				{
					m_Text = value;
					NotifyPropertyChanged(m_TextArgs);
				}
			}
		}
		private string m_Text = String.Empty;
		static readonly PropertyChangedEventArgs m_TextArgs =
				NotifyPropertyChangedHelper.CreateArgs<AbstractLabel>(o => o.Text);
		#endregion

	}
}