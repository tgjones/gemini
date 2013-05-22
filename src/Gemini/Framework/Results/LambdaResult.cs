using System;
using Caliburn.Micro;

namespace Gemini.Framework.Results
{
    public class LambdaResult : IResult
    {
        private readonly Action<ActionExecutionContext> _lambda;

        public LambdaResult(Action<ActionExecutionContext> lambda)
        {
            _lambda = lambda;
        }

        public void Execute(ActionExecutionContext context)
        {
            _lambda(context);

            var completedHandler = Completed;
            if (completedHandler != null)
                completedHandler(this, new ResultCompletionEventArgs());
        }

        public event EventHandler<ResultCompletionEventArgs> Completed;
    }
}