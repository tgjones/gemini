using System.ComponentModel;
using Gemini.Contracts.Utilities;

namespace Gemini.Contracts.Gui.Controls
{
	public abstract class AbstractRadioButton : AbstractToggleButton, IToggleButton
	{
		#region " GroupName "
		/// <summary>
		/// This is the GroupName displayed in the button itself.
		/// Best to set this property in the derived class's constructor.
		/// </summary>
		public string GroupName
		{
			get
			{
				return m_GroupName;
			}
			protected set
			{
				if (m_GroupName != value)
				{
					m_GroupName = value;
					NotifyPropertyChanged(m_GroupNameArgs);
				}
			}
		}
		private string m_GroupName = null;
		static readonly PropertyChangedEventArgs m_GroupNameArgs =
				NotifyPropertyChangedHelper.CreateArgs<AbstractRadioButton>(o => o.GroupName);

		#endregion

	}
}