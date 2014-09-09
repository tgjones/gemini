using System.Windows;
using System.Windows.Input;

namespace Gemini.Modules.MainMenu.Controls
{
	public class MenuEx : System.Windows.Controls.Menu
	{
	    public static readonly DependencyProperty AutoHideProperty = DependencyProperty.Register(
	        "AutoHide", typeof (bool), typeof (MenuEx), new PropertyMetadata(default(bool), AutoHidePropertyChangedCallback));

		private object _currentItem;

        public bool AutoHide
        {
            get { return (bool)GetValue(AutoHideProperty); }
            set { SetValue(AutoHideProperty, value); }
        }

		protected override bool IsItemItsOwnContainerOverride(object item)
		{
			_currentItem = item;
			return base.IsItemItsOwnContainerOverride(item);
		}

		protected override DependencyObject GetContainerForItemOverride()
		{
			return MenuItemEx.GetContainer(this, _currentItem);
		}

	    protected override void OnGotKeyboardFocus(KeyboardFocusChangedEventArgs e)
	    {
	        base.OnGotKeyboardFocus(e);

	        UpdateVisibility();
	    }

	    protected override void OnLostKeyboardFocus(KeyboardFocusChangedEventArgs e)
	    {
	        base.OnLostKeyboardFocus(e);

            UpdateVisibility();
	    }

	    protected override void OnLostFocus(RoutedEventArgs e)
	    {
	        base.OnLostFocus(e);

            UpdateVisibility();
	    }

	    private static void AutoHidePropertyChangedCallback(DependencyObject dependencyObject,
	        DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
	    {
	        var menu = (MenuEx) dependencyObject;
            menu.UpdateVisibility();
	    }

	    private void UpdateVisibility()
        {
            if (!AutoHide)
	        {
                Height = double.NaN;
                return;
	        }

	        if (IsKeyboardFocused || IsFocused || IsKeyboardFocusWithin)
	        {
	            Height = double.NaN;
	        }
	        else
	        {
	            Height = 0;
	        }
        }
	}
}