using System.ComponentModel;
using Gemini.Contracts.Utilities;

namespace Gemini.Contracts.Gui.Controls
{
	public abstract class AbstractToggleButton : AbstractButton, IToggleButton
	{

		#region " IsChecked "
		/// <summary>
		/// True if the item is checked, false otherwise.
		/// Calls OnIsCheckedChanged(), which can be overridden in the
		/// derived class to take an action when the status
		/// is toggled.
		/// </summary>
		public bool IsChecked
		{
			get
			{
				return m_IsChecked;
			}
			set
			{
				if (m_IsChecked != value)
				{
					m_IsChecked = value;
					NotifyPropertyChanged(m_IsCheckedArgs);
					OnIsCheckedChanged();
				}
			}
		}
		private bool m_IsChecked = false;
		static readonly PropertyChangedEventArgs m_IsCheckedArgs =
				NotifyPropertyChangedHelper.CreateArgs<AbstractToggleButton>(o => o.IsChecked);


		/// <summary>
		/// This method is called only if IsCheckable is true and
		/// the user changes the IsChecked property. Override it in 
		/// the derived class to take an action when it changes.
		/// </summary>
		protected virtual void OnIsCheckedChanged() { }

		#endregion

	}
}