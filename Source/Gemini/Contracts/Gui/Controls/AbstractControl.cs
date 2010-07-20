using System;
using System.ComponentModel;
using System.Windows;
using Gemini.Contracts.Conditions;
using Gemini.Contracts.Services.ExtensionService;
using Gemini.Contracts.Utilities;

namespace Gemini.Contracts.Gui.Controls
{
	public abstract class AbstractControl : AbstractExtension, IControl
	{
		public AbstractControl()
		{
		}

		#region " ToolTip "
		/// <summary>
		/// This is the tool tip displayed when the mouse hovers over the control.
		/// Best practice is to set this in the constructor of the derived
		/// class.
		/// </summary>
		public string ToolTip
		{
			get
			{
				return m_ToolTip;
			}
			protected set
			{

				string formattedValue = string.Empty;
				if (value != null)
				{
					formattedValue = value.Replace("\\n", Environment.NewLine);
				}
				if (m_ToolTip != formattedValue)
				{
					if (formattedValue == string.Empty)
					{
						m_ToolTip = null;
					}
					else
					{
						m_ToolTip = formattedValue;
					}
					NotifyPropertyChanged(m_ToolTipArgs);
				}
			}
		}
		private string m_ToolTip = null;
		static readonly PropertyChangedEventArgs m_ToolTipArgs =
				NotifyPropertyChangedHelper.CreateArgs<AbstractControl>(o => o.ToolTip);
		#endregion

		#region " Visible "
		/// <summary>
		/// Defaults to true. Set to false to make the control disappear.
		/// </summary>
		public bool Visible
		{
			get
			{
				return m_Visible;
			}
			private set
			{
				if (m_Visible != value)
				{
					m_Visible = value;
					NotifyPropertyChanged(m_VisibleArgs);
				}
			}
		}
		private bool m_Visible = true;
		static readonly PropertyChangedEventArgs m_VisibleArgs =
				NotifyPropertyChangedHelper.CreateArgs<AbstractControl>(o => o.Visible);
		#endregion

		#region " VisibleCondition "
		/// <summary>
		/// Set this to any ICondition object, and it will control
		/// the Visible property.
		/// </summary>
		public ICondition VisibleCondition
		{
			get
			{
				return m_VisibleCondition;
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException(m_VisibleConditionName);
				}
				if (m_VisibleCondition != value)
				{
					if (m_VisibleCondition != null)
					{
						//remove the old event handler
						m_VisibleCondition.ConditionChanged -= OnVisibleConditionChanged;
					}
					m_VisibleCondition = value;
					//add the new event handler
					m_VisibleCondition.ConditionChanged += OnVisibleConditionChanged;
					Visible = m_VisibleCondition.Condition;

					NotifyPropertyChanged(m_VisibleConditionArgs);
				}
			}
		}
		private ICondition m_VisibleCondition = null;
		static readonly PropertyChangedEventArgs m_VisibleConditionArgs =
				NotifyPropertyChangedHelper.CreateArgs<AbstractControl>(o => o.VisibleCondition);
		static readonly string m_VisibleConditionName =
				NotifyPropertyChangedHelper.GetPropertyName<AbstractControl>(o => o.VisibleCondition);

		private void OnVisibleConditionChanged(object sender, EventArgs e)
		{
			Visible = m_VisibleCondition.Condition;
		}
		#endregion

		#region " Padding "
		/// <summary>
		/// The padding inside the control.
		/// </summary>
		public Thickness Padding
		{
			get
			{
				return m_Padding;
			}
			protected set
			{
				if (value == null)
				{
					throw new ArgumentNullException(m_PaddingName);
				}
				if (m_Padding != value)
				{
					m_Padding = value;
					NotifyPropertyChanged(m_PaddingArgs);
				}
			}
		}
		private Thickness m_Padding = new Thickness(0);
		static readonly PropertyChangedEventArgs m_PaddingArgs =
				NotifyPropertyChangedHelper.CreateArgs<AbstractControl>(o => o.Padding);
		static readonly string m_PaddingName =
				NotifyPropertyChangedHelper.GetPropertyName<AbstractControl>(o => o.Padding);
		#endregion

		#region " Margin "
		/// <summary>
		/// The Margin inside the control.
		/// </summary>
		public Thickness Margin
		{
			get
			{
				return m_Margin;
			}
			protected set
			{
				if (value == null)
				{
					throw new ArgumentNullException(m_MarginName);
				}
				if (m_Margin != value)
				{
					m_Margin = value;
					NotifyPropertyChanged(m_MarginArgs);
				}
			}
		}
		private Thickness m_Margin = new Thickness(0);
		static readonly PropertyChangedEventArgs m_MarginArgs =
				NotifyPropertyChangedHelper.CreateArgs<AbstractControl>(o => o.Margin);
		static readonly string m_MarginName =
				NotifyPropertyChangedHelper.GetPropertyName<AbstractControl>(o => o.Margin);
		#endregion

		#region " Width "
		/// <summary>
		/// This is the Width of the control
		/// </summary>
		public double Width
		{
			get
			{
				return m_Width;
			}
			protected set
			{
				if (value < 0)
				{
					throw new ArgumentOutOfRangeException(m_WidthName);
				}
				if (m_Width != value)
				{
					m_Width = value;
					NotifyPropertyChanged(m_WidthArgs);
				}
			}
		}
		private double m_Width = double.NaN;
		static readonly PropertyChangedEventArgs m_WidthArgs =
				NotifyPropertyChangedHelper.CreateArgs<AbstractControl>(o => o.Width);
		static readonly string m_WidthName =
				NotifyPropertyChangedHelper.GetPropertyName<AbstractControl>(o => o.Width);
		#endregion

		#region " Height "
		/// <summary>
		/// This is the Height of the control
		/// </summary>
		public double Height
		{
			get
			{
				return m_Height;
			}
			protected set
			{
				if (value < 0)
				{
					throw new ArgumentOutOfRangeException(m_HeightName);
				}
				if (m_Height != value)
				{
					m_Height = value;
					NotifyPropertyChanged(m_HeightArgs);
				}
			}
		}
		private double m_Height = double.NaN;
		static readonly PropertyChangedEventArgs m_HeightArgs =
				NotifyPropertyChangedHelper.CreateArgs<AbstractControl>(o => o.Height);
		static readonly string m_HeightName =
				NotifyPropertyChangedHelper.GetPropertyName<AbstractControl>(o => o.Height);
		#endregion

	}
}