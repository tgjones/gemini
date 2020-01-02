using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Gemini.Framework
{
    public interface IAsyncCommand : ICommand
    {
        Task ExecuteAsync();
        bool CanExecute();
    }

    public class AsyncCommand : IAsyncCommand
    {
        public event EventHandler CanExecuteChanged;

        private bool _isExecuting;
        private readonly Func<Task> _execute;
        private readonly Func<bool> _canExecute;

        public AsyncCommand(Func<Task> execute, Func<bool> canExecute = null)
        {
            _execute = execute;
            _canExecute = canExecute;
        }

        public bool CanExecute() => !_isExecuting && (_canExecute?.Invoke() ?? true);

        public async Task ExecuteAsync()
        {
            if (CanExecute())
            {
                try
                {
                    _isExecuting = true;
                    await _execute();
                }
                finally
                {
                    _isExecuting = false;
                }
            }

            RaiseCanExecuteChanged();
        }

        public void RaiseCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);

        bool ICommand.CanExecute(object parameter) => CanExecute();

        void ICommand.Execute(object parameter)
        {
            TryExecuteTask(ExecuteAsync());
        }

        private static async void TryExecuteTask(Task task)
        {
            try
            {
                await task;
            }
            catch
            {
                throw;
            }
        }
    }
}
