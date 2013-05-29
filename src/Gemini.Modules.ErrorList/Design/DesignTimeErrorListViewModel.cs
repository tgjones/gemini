using Gemini.Modules.ErrorList.ViewModels;

namespace Gemini.Modules.ErrorList.Design
{
    public class DesignTimeErrorListViewModel : ErrorListViewModel
    {
        public DesignTimeErrorListViewModel()
        {
            Items.Add(new ErrorListItem
            {
                ItemType = ErrorListItemType.Error,
                Number = 1,
                Description = "This is an error.",
                Path = "File1.txt",
                Line = 42,
                Column = 24
            });
            Items.Add(new ErrorListItem
            {
                ItemType = ErrorListItemType.Warning,
                Number = 2,
                Description = "This is a warning.",
                Path = "File1.txt",
                Line = 1,
                Column = 2
            });
            Items.Add(new ErrorListItem
            {
                ItemType = ErrorListItemType.Message,
                Number = 3,
                Description = "This is a message.",
            });
        }
    }
}