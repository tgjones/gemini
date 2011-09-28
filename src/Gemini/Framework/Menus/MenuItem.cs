using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Windows.Controls;
using System.Windows.Input;
using Caliburn.Micro;
using Gemini.Framework.Services;

namespace Gemini.Framework.Menus
{
	public class MenuItem : PropertyChangedBase, IEnumerable<MenuItem>
	{
		private readonly Func<IEnumerable<IResult>> _execute;
		private readonly Func<bool> _canExecute = () => true;
		private KeyGesture _keyGesture;

		#region Static stuff

		public static MenuItem Separator
		{
			get { return new MenuItem { IsSeparator = true }; }
		}

		#endregion

		#region Properties

		public IObservableCollection<MenuItem> Children { get; private set; }

		private string _text;
		public string Text
		{
			get { return _text; }
			set { _text = value; NotifyOfPropertyChange(() => Text); }
		}

		private bool _isSeparator;
		public bool IsSeparator
		{
			get { return _isSeparator; }
			set { _isSeparator = value; NotifyOfPropertyChange(() => IsSeparator); }
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

		public string Name
		{
			get { return string.IsNullOrEmpty(Text) ? null : Text.Replace("_", string.Empty); }
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

		#endregion

		#region Constructors

		private MenuItem()
		{
			Children = new BindableCollection<MenuItem>();
		}

		public MenuItem(string text)
			: this()
		{
			Text = text;
		}

		public MenuItem(string text, Func<IEnumerable<IResult>> execute)
			: this(text)
		{
			_execute = execute;
		}

		public MenuItem(string text, Func<IEnumerable<IResult>> execute, Func<bool> canExecute)
			: this(text, execute)
		{
			_canExecute = canExecute;
		}

		#endregion

		public void Add(params MenuItem[] menuItems)
		{
			menuItems.Apply(Children.Add);
		}

		public IEnumerable<IResult> Execute()
		{
			return _execute != null ? _execute() : new IResult[] { };
		}

		public IEnumerator<MenuItem> GetEnumerator()
		{
			return Children.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		#region Fluent interface

		public MenuItem WithGlobalShortcut(ModifierKeys modifier, Key key)
		{
			_keyGesture = new KeyGesture(key, modifier);
			IoC.Get<IInputManager>().SetShortcut(_keyGesture, this);
			return this;
		}

		public MenuItem WithIcon()
		{
			return WithIcon(Assembly.GetCallingAssembly(), "Resources/Icons/" + Name.Replace(" ", string.Empty) + ".png");
		}

		public MenuItem WithIcon(string path)
		{
			return WithIcon(Assembly.GetCallingAssembly(), path);
		}

		public MenuItem WithIcon(Assembly source, string path)
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