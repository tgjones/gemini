using System;
using System.Reflection;
using System.Windows.Media.Imaging;
using Caliburn.Core;
using Gemini.Framework.Services;
using Microsoft.Practices.ServiceLocation;

namespace Gemini.Framework.Ribbon
{
	public class RibbonButtonBase<TRibbonButton> : PropertyChangedBase, IRibbonItem, IRibbonBackstageItem
		where TRibbonButton : RibbonButtonBase<TRibbonButton>
	{
		private readonly Func<bool> _canExecute;
		private string _text;
		private Sizes _sizes;
		private BitmapSource _icon, _largeIcon;

		#region Properties

		public string Name
		{
			get { return string.IsNullOrEmpty(Text) ? null : Text.Replace("_", string.Empty); }
		}

		public string Text
		{
			get { return _text; }
			set { _text = value; NotifyOfPropertyChange("Text"); }
		}

		public Sizes Sizes
		{
			get { return _sizes; }
			set { _sizes = value; NotifyOfPropertyChange("Sizes"); }
		}

		public BitmapSource Icon
		{
			get { return _icon; }
		}

		public BitmapSource LargeIcon
		{
			get { return _largeIcon ?? _icon; }
		}

		public string ActionText
		{
			get { return "Execute"; }
		}

		public bool CanExecute
		{
			get { return _canExecute(); }
		}

		#endregion

		public RibbonButtonBase(string text, Func<bool> canExecute = null, Sizes sizes = Sizes.Middle)
		{
			Text = text;
			_canExecute = canExecute ?? (() => true);
			Sizes = sizes;
		}

		public TRibbonButton WithIcon()
		{
			return WithIcon(Assembly.GetCallingAssembly(), "Resources/Icons/" + Name.Replace(" ", string.Empty) + ".png");
		}

		public TRibbonButton WithIcon(string path)
		{
			return WithIcon(Assembly.GetCallingAssembly(), path);
		}

		public TRibbonButton WithIcon(Assembly source, string path)
		{
			var manager = ServiceLocator.Current.GetInstance<IResourceManager>();
			var iconSource = manager.GetBitmap(path, source.GetAssemblyName());

			if (source != null)
				_icon = iconSource;

			return (TRibbonButton) this;
		}

		public TRibbonButton WithLargeIcon()
		{
			return WithLargeIcon(Assembly.GetCallingAssembly(), "Resources/Icons/" + Name.Replace(" ", string.Empty) + "Large.png");
		}

		public TRibbonButton WithLargeIcon(string path)
		{
			return WithLargeIcon(Assembly.GetCallingAssembly(), path);
		}

		public TRibbonButton WithLargeIcon(Assembly source, string path)
		{
			var manager = ServiceLocator.Current.GetInstance<IResourceManager>();
			var iconSource = manager.GetBitmap(path, source.GetAssemblyName());

			if (source != null)
				_largeIcon = iconSource;

			return (TRibbonButton) this;
		}
	}
}