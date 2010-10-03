using System;
using Gemini.Framework.Questions;
using Microsoft.Practices.ServiceLocation;
using Caliburn.PresentationFramework;
using Caliburn.PresentationFramework.ApplicationModel;

namespace Gemini.Framework.Results
{
	public class OpenModalResult<TModal> : OpenResultBase<TModal>
        where TModal : IExtendedPresenter
    {
        private readonly Func<TModal> _locateModal = () => ServiceLocator.Current.GetInstance<TModal>();

        public OpenModalResult() {}

        public OpenModalResult(TModal child)
        {
            _locateModal = () => child;
        }

        public override void Execute(IRoutedMessageWithOutcome message, IInteractionNode handlingNode)
        {
            var windowManager = ServiceLocator.Current.GetInstance<IWindowManager>();
            var child = _locateModal();

            if(_setData != null)
                _setData(child);

            if(_onConfigure != null)
                _onConfigure(child);

            child.WasShutdown +=
                (s, e) =>{
                    if(_onShutDown != null)
                        _onShutDown(child);

                    OnCompleted(null);
                };

            windowManager.ShowDialog(child, null, HandleShutdown);
        }

        private void HandleShutdown(ISubordinate model, Action completed)
        {
            model.Execute(completed);
        }
    }
}