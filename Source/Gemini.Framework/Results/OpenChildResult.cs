using System;
using Microsoft.Practices.ServiceLocation;
using Caliburn.PresentationFramework;
using Caliburn.PresentationFramework.ApplicationModel;

namespace Gemini.Framework.Results
{
	public class OpenChildResult<TChild> : OpenResultBase<TChild>
        where TChild : IExtendedPresenter
    {
        private Func<IPresenterHost> _locateParent;
        private readonly Func<TChild> _locateChild = () => ServiceLocator.Current.GetInstance<TChild>();

        public OpenChildResult() {}

        public OpenChildResult(TChild child)
        {
            _locateChild = () => child;
        }

        public OpenChildResult<TChild> In<TParent>()
            where TParent : IPresenterHost
        {
            _locateParent = () => ServiceLocator.Current.GetInstance<TParent>();
            return this;
        }

        public override void Execute(IRoutedMessageWithOutcome message, IInteractionNode handlingNode)
        {
            if(_locateParent == null)
                _locateParent = () => (IPresenterHost)handlingNode.MessageHandler.Unwrap();

            var parent = _locateParent();
            var child = _locateChild();

            if(_setData != null)
                _setData(child);

            if(_onConfigure != null)
                _onConfigure(child);

            parent.Open(child, delegate{
                child.WasShutdown += (s, e) =>{
                    if(_onShutDown != null)
                        _onShutDown(child);
                };

                OnCompleted(null);
            });
        }
    }
}