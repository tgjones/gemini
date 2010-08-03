using System.Windows;
using System.Windows.Input;
using Caliburn.PresentationFramework;
using Caliburn.PresentationFramework.Commands;

namespace Gemini.Framework.Services
{
	public class DefaultInputManager : IInputManager
	{
		public void SetShortcut(DependencyObject view, InputGesture gesture, object command)
		{
			var trigger = new RoutedGestureMessageTrigger(gesture)
			{
				Message = new CommandMessage
				{
					Command = command
				}
			};

			var triggers = Message.GetTriggers(view);

			if (triggers == null)
			{
				triggers = new RoutedMessageTriggerCollection { trigger };
				Message.SetTriggers(view, triggers);
			}
			else triggers.Add(trigger);
		}
	}
}