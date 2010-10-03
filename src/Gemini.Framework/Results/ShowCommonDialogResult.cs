using System;
using Microsoft.Win32;
using Caliburn.PresentationFramework;

namespace Gemini.Framework.Results
{
	public class ShowCommonDialogResult : IResult
    {
        private readonly CommonDialog _commonDialog;

        public ShowCommonDialogResult(CommonDialog commonDialog)
        {
            _commonDialog = commonDialog;
        }

        public void Execute(IRoutedMessageWithOutcome message, IInteractionNode handlingNode)
        {
            var result = _commonDialog.ShowDialog().GetValueOrDefault(false);

            if (result)
                Completed(this, null);
            else Completed(this, new CancelResult());
        }

        public event Action<IResult, Exception> Completed = delegate { };
    }
}