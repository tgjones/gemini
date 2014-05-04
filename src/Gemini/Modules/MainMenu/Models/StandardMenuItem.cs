using System;
using System.Globalization;
using System.Reflection;
using System.Windows.Controls;
using System.Windows.Input;
using Caliburn.Micro;
using Gemini.Framework;
using Gemini.Framework.Services;

namespace Gemini.Modules.MainMenu.Models
{
	public class StandardMenuItem : MenuItemBase, IExecutableItem
	{
		private readonly Func<bool> _canExecute = () => true;
		private KeyGesture _keyGesture;

		private string _text;
		public string Text
		{
			get { return _text; }
			set { _text = value; NotifyOfPropertyChange(() => Text); }
		}

		public Image Icon { get; private set; }

		public string ActionText
		{
			get { return "Execute"; }
		}

		public bool CanExecute
		{
			get { return _canExecute(); }
		}

		public string InputGestureText
		{
			get
			{
				return _keyGesture == null
					? string.Empty
					: _keyGesture.GetDisplayStringForCulture(CultureInfo.CurrentUICulture);
			}
		}

        public StandardMenuItem(string text)
            : base(string.IsNullOrEmpty(text) ? null : text.Replace("_", string.Empty))
		{
			Text = text;
		}

        public StandardMenuItem(string name, string text)
            : base(name)
        {
            this.Text = text;
        }

		public StandardMenuItem(string text, Func<bool> canExecute)
			: this(text)
		{
			_canExecute = canExecute ?? (() => true);
		}

        public StandardMenuItem(string name, string text, Func<bool> canExecute)
            : this(name, text)
        {
            this._canExecute = canExecute ?? (() => true);
        }

        public void RaiseCanExecuteChanged()
        {
            NotifyOfPropertyChange(() => CanExecute);
        }

		#region Fluent interface

		public StandardMenuItem WithGlobalShortcut(ModifierKeys modifier, Key key)
		{
			_keyGesture = new KeyGesture(key, modifier);
			IoC.Get<IInputManager>().SetShortcut(_keyGesture, this);
			return this;
		}

		public StandardMenuItem WithIcon()
		{
			return WithIcon(Assembly.GetCallingAssembly(), "Resources/Icons/" + Name.Replace(" ", string.Empty) + ".png");
		}

		public StandardMenuItem WithIcon(string path)
		{
			return WithIcon(Assembly.GetCallingAssembly(), path);
		}

		public StandardMenuItem WithIcon(Assembly source, string path)
		{
			var manager = IoC.Get<IResourceManager>();
			var iconSource = manager.GetBitmap(path, source.GetAssemblyName());

			if (source != null)
				Icon = new Image
				{
					Source = iconSource,
					Width = 16,
					Height = 16
				};

			return this;
		}

		#endregion
	}
}