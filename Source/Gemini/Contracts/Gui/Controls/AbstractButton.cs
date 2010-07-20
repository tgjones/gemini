using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Media.Imaging;
using Gemini.Contracts.Utilities;

namespace Gemini.Contracts.Gui.Controls
{
	public abstract class AbstractButton : AbstractCommandControl, IButton
	{
		protected AbstractButton()
		{
			/*CanExecuteChanged += delegate
			{
				NotifyPropertyChanged(m_IconArgs);
			};*/
		}

		#region " Text "
		/// <summary>
		/// This is the text displayed in the button itself.
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
				if (m_Text != value)
				{
					m_Text = value;
					NotifyPropertyChanged(m_TextArgs);
				}
			}
		}
		private string m_Text = null;
		static readonly PropertyChangedEventArgs m_TextArgs =
				NotifyPropertyChangedHelper.CreateArgs<AbstractButton>(o => o.Text);
		#endregion

		#region " Icon "
		/// <summary>
		/// Optional icon that can be displayed in the button.
		/// </summary>
		public BitmapSource Icon
		{
			get
			{
				return _icon;
			}
			protected set
			{
				if (_icon != value)
				{
					_icon = value;
					NotifyPropertyChanged(m_IconArgs);
				}
			}
		}
		private BitmapSource _icon;
		static readonly PropertyChangedEventArgs m_IconArgs =
				NotifyPropertyChangedHelper.CreateArgs<AbstractButton>(o => o.Icon);

		/// <summary>
		/// Optional icon that can be displayed in the button.
		/// </summary>
		public BitmapSource LargeIcon
		{
			get
			{
				return _largeIcon ?? Icon;
			}
			protected set
			{
				if (_largeIcon != value)
				{
					_largeIcon = value;
					NotifyPropertyChanged(m_LargeIconArgs);
				}
			}
		}
		private BitmapSource _largeIcon;
		static readonly PropertyChangedEventArgs m_LargeIconArgs =
				NotifyPropertyChangedHelper.CreateArgs<AbstractButton>(o => o.LargeIcon);

		/// <summary>
		/// This is a helper function so you can assign the Icon directly
		/// from a Bitmap, such as one from a resources file.
		/// </summary>
		/// <param name="value"></param>
		protected void SetIconFromBitmap(System.Drawing.Bitmap value)
		{
			BitmapSource b = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
					value.GetHbitmap(),
					IntPtr.Zero,
					Int32Rect.Empty,
					BitmapSizeOptions.FromEmptyOptions());
			Icon = b;
		}

		#endregion

		#region " IsCancel "
		/// <summary>
		/// Defaults to true. Set to false to make the control disappear.
		/// </summary>
		public bool IsCancel
		{
			get
			{
				return m_IsCancel;
			}
			protected set
			{
				if (m_IsCancel != value)
				{
					m_IsCancel = value;
					NotifyPropertyChanged(m_IsCancelArgs);
				}
			}
		}
		private bool m_IsCancel = false;
		static readonly PropertyChangedEventArgs m_IsCancelArgs =
				NotifyPropertyChangedHelper.CreateArgs<AbstractButton>(o => o.IsCancel);
		#endregion

		#region " IsDefault "
		/// <summary>
		/// Defaults to true. Set to false to make the control disappear.
		/// </summary>
		public bool IsDefault
		{
			get
			{
				return m_IsDefault;
			}
			protected set
			{
				if (m_IsDefault != value)
				{
					m_IsDefault = value;
					NotifyPropertyChanged(m_IsDefaultArgs);
				}
			}
		}
		private bool m_IsDefault = false;
		static readonly PropertyChangedEventArgs m_IsDefaultArgs =
				NotifyPropertyChangedHelper.CreateArgs<AbstractButton>(o => o.IsDefault);
		#endregion


	}
}