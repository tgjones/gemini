using System;
using Caliburn.PresentationFramework;

namespace Gemini.Framework.Results
{
	public abstract class OpenResultBase<TTarget> : IOpenResult<TTarget>
	{
		protected Action<TTarget> _setData;
		protected Action<TTarget> _onConfigure;
		protected Action<TTarget> _onShutDown;

		Action<TTarget> IOpenResult<TTarget>.OnConfigure
		{
			get { return _onConfigure; }
			set { _onConfigure = value; }
		}

		Action<TTarget> IOpenResult<TTarget>.OnShutDown
		{
			get { return _onShutDown; }
			set { _onShutDown = value; }
		}

		void IOpenResult<TTarget>.SetData<TData>(TData data)
		{
			_setData = child =>
			{
				var dataCentric = (IDataCentric<TData>) child;
				dataCentric.LoadData(data);
			};
		}

		public abstract void Execute(IRoutedMessageWithOutcome message, IInteractionNode handlingNode);
		public event Action<IResult, Exception> Completed = delegate { };

		protected virtual void OnCompleted(Exception exception)
		{
			Completed(this, exception);
		}
	}
}