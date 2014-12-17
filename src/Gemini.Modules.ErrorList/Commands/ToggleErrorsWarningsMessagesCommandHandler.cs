using System.ComponentModel.Composition;
using System.Linq;
using System.Threading.Tasks;
using Gemini.Framework.Commands;
using Gemini.Framework.Threading;

namespace Gemini.Modules.ErrorList.Commands
{
    [CommandHandler]
    public class ToggleErrorsWarningsMessagesCommandHandler :
        ICommandHandler<ToggleErrorsCommandDefinition>, 
        ICommandHandler<ToggleWarningsCommandDefinition>, 
        ICommandHandler<ToggleMessagesCommandDefinition>
    {
        private readonly IErrorList _errorList;

        [ImportingConstructor]
        public ToggleErrorsWarningsMessagesCommandHandler(IErrorList errorList)
        {
            _errorList = errorList;
        }

        void ICommandHandler<ToggleErrorsCommandDefinition>.Update(Command command)
        {
            command.Enabled = ErrorItemCount > 0;
            command.Checked = command.Enabled && _errorList.ShowErrors;
            command.Text = command.ToolTip = Pluralize("Error", ErrorItemCount);
        }

        Task ICommandHandler<ToggleErrorsCommandDefinition>.Run(Command command)
        {
            _errorList.ShowErrors = !_errorList.ShowErrors;
            return TaskUtility.Completed;
        }

        void ICommandHandler<ToggleWarningsCommandDefinition>.Update(Command command)
        {
            command.Enabled = WarningItemCount > 0;
            command.Checked = command.Enabled && _errorList.ShowWarnings;
            command.Text = command.ToolTip = Pluralize("Warning", WarningItemCount);
        }

        Task ICommandHandler<ToggleWarningsCommandDefinition>.Run(Command command)
        {
            _errorList.ShowWarnings = !_errorList.ShowWarnings;
            return TaskUtility.Completed;
        }

        void ICommandHandler<ToggleMessagesCommandDefinition>.Update(Command command)
        {
            command.Enabled = MessageItemCount > 0;
            command.Checked = command.Checked && _errorList.ShowMessages;
            command.Text = command.ToolTip = Pluralize("Message", MessageItemCount);
        }

        Task ICommandHandler<ToggleMessagesCommandDefinition>.Run(Command command)
        {
            _errorList.ShowMessages = !_errorList.ShowMessages;
            return TaskUtility.Completed;
        }

        private static string Pluralize(string text, int number)
        {
            if (number == 1)
                return number + " " + text;

            return number + " " + text + "s";
        }

        private int ErrorItemCount
        {
            get { return _errorList.Items.Count(x => x.ItemType == ErrorListItemType.Error); }
        }

        private int WarningItemCount
        {
            get { return _errorList.Items.Count(x => x.ItemType == ErrorListItemType.Warning); }
        }

        private int MessageItemCount
        {
            get { return _errorList.Items.Count(x => x.ItemType == ErrorListItemType.Message); }
        }
    }
}