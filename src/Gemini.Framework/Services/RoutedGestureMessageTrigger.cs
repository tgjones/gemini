using System.Windows;
using System.Windows.Input;
using Caliburn.Core;
using Caliburn.PresentationFramework;

namespace Gemini.Framework.Services
{
	public class RoutedGestureMessageTrigger : BaseMessageTrigger
    {
        private readonly InputGesture _gesture;
        private bool _canExecute;

        public RoutedGestureMessageTrigger(InputGesture gesture)
        {
            _gesture = gesture;
        }

        public override void Attach(IInteractionNode node)
        {
            var command = new RoutedCommand();
            var element = node.UIElement as UIElement;

            if(element == null)
                throw new CaliburnException("You cannot add a RoutedGestureMessageTrigger to a non-UIElement.");

            element.InputBindings.Add(new InputBinding(command, _gesture));

            var commandBinding = new CommandBinding(command);

            commandBinding.CanExecute += CommandBinding_CanExecute;
            commandBinding.Executed += CommandBinding_Executed;

            element.CommandBindings.Add(commandBinding);

            base.Attach(node);
        }

        public override void UpdateAvailabilty(bool isAvailable)
        {
            _canExecute = isAvailable;
        }

        private void CommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Node.ProcessMessage(Message, e.Parameter);
        }

        private void CommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            Node.UpdateAvailability(this);
            e.CanExecute = _canExecute;
        }

        protected override Freezable CreateInstanceCore()
        {
            return new RoutedGestureMessageTrigger(_gesture);
        }
    }
}