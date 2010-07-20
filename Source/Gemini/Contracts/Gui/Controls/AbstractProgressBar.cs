using System.ComponentModel;
using Gemini.Contracts.Utilities;

namespace Gemini.Contracts.Gui.Controls
{
	public abstract class AbstractProgressBar : AbstractControl, IProgressBar
	{
		#region " Minimum "
		/// <summary>
		/// This is the Minimum value of the progress bar - default is zero
		/// </summary>
		public double Minimum
		{
			get
			{
				return m_Minimum;
			}
			protected set
			{
				if (m_Minimum != value)
				{
					m_Minimum = value;
					NotifyPropertyChanged(m_MinimumArgs);
				}
			}
		}
		private double m_Minimum = 0;
		static readonly PropertyChangedEventArgs m_MinimumArgs =
				NotifyPropertyChangedHelper.CreateArgs<AbstractProgressBar>(o => o.Minimum);
		#endregion

		#region " Maximum "
		/// <summary>
		/// This is the Maximum value of the progress bar - default is 100
		/// </summary>
		public double Maximum
		{
			get
			{
				return m_Maximum;
			}
			protected set
			{
				if (m_Maximum != value)
				{
					m_Maximum = value;
					NotifyPropertyChanged(m_MaximumArgs);
				}
			}
		}
		private double m_Maximum = 100;
		static readonly PropertyChangedEventArgs m_MaximumArgs =
				NotifyPropertyChangedHelper.CreateArgs<AbstractProgressBar>(o => o.Maximum);
		#endregion

		#region " Value "
		/// <summary>
		/// This is the Value value of the progress bar - default is zero
		/// </summary>
		public double Value
		{
			get
			{
				return m_Value;
			}
			protected set
			{
				if (m_Value != value)
				{
					m_Value = value;
					NotifyPropertyChanged(m_ValueArgs);
				}
			}
		}
		private double m_Value = 0;
		static readonly PropertyChangedEventArgs m_ValueArgs =
				NotifyPropertyChangedHelper.CreateArgs<AbstractProgressBar>(o => o.Value);
		#endregion

	}
}