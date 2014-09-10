using System;
using Caliburn.Micro;
using Microsoft.Win32;

namespace Gemini.Framework.Results
{
	public class ShowCommonDialogResult : IResult
	{
		public event EventHandler<ResultCompletionEventArgs> Completed;

		private readonly CommonDialog _commonDialog;

		public ShowCommonDialogResult(CommonDialog commonDialog)
		{
			_commonDialog = commonDialog;
		}

		public void Execute(CoroutineExecutionContext context)
		{
			var result = _commonDialog.ShowDialog().GetValueOrDefault(false);
			Completed(this, new ResultCompletionEventArgs { WasCancelled = !result });
		}
	}
}