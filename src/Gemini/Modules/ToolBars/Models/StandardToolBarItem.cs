using System;
using System.Globalization;
using System.Reflection;
using System.Windows.Input;
using System.Windows.Media;
using Caliburn.Micro;
using Gemini.Framework;
using Gemini.Framework.Services;

namespace Gemini.Modules.ToolBars.Models
{
	public class StandardToolBarItem : ToolBarItemBase, IExecutableItem
    {
		private readonly Func<bool> _canExecute = () => true;
		private KeyGesture _keyGesture;

		private string _text;
		public string Text
		{
			get { return _text; }
			set { _text = value; NotifyOfPropertyChange(() => Text); }
		}

	    private ImageSource _icon;
	    public ImageSource Icon
	    {
	        get { return _icon; }
	        set
	        {
                _icon = value;
	            NotifyOfPropertyChange(() => Icon);
	        }
	    }

	    public string ActionText
		{
			get { return "Execute"; }
		}

		public bool CanExecute
		{
			get { return _canExecute(); }
		}

		public override string Name
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

		public StandardToolBarItem(string text)
		{
			Text = text;
		}

        public StandardToolBarItem(string text, Func<bool> canExecute)
			: this(text)
		{
			_canExecute = canExecute;
		}

        public void RaiseCanExecuteChanged()
        {
            NotifyOfPropertyChange(() => CanExecute);
        }

		#region Fluent interface

		public StandardToolBarItem WithGlobalShortcut(ModifierKeys modifier, Key key)
		{
			_keyGesture = new KeyGesture(key, modifier);
			IoC.Get<IInputManager>().SetShortcut(_keyGesture, this);
			return this;
		}

        public StandardToolBarItem WithIcon()
		{
			return WithIcon(Assembly.GetCallingAssembly(), "Resources/Icons/" + Name.Replace(" ", string.Empty) + ".png");
		}

        public StandardToolBarItem WithIcon(string path)
		{
			return WithIcon(Assembly.GetCallingAssembly(), path);
		}

        public StandardToolBarItem WithIcon(Assembly source)
        {
            return WithIcon(source, "Resources/Icons/" + Name.Replace(" ", string.Empty) + ".png");
        }

        public StandardToolBarItem WithIcon(Assembly source, string path)
		{
			var manager = IoC.Get<IResourceManager>();
			Icon = manager.GetBitmap(path, source.GetAssemblyName());

			return this;
		}

		#endregion
	}
}