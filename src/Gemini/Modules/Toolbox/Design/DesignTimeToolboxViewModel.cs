using Gemini.Modules.Toolbox.ViewModels;

namespace Gemini.Modules.Toolbox.Design
{
    public class DesignTimeToolboxViewModel : ToolboxViewModel
    {
        public DesignTimeToolboxViewModel()
            : base(null, null)
        {
            Items.Add(new ToolboxItemViewModel(new Models.ToolboxItem { Name = "Foo", Category = "General" }));
            Items.Add(new ToolboxItemViewModel(new Models.ToolboxItem { Name = "Bar", Category = "General" }));
            Items.Add(new ToolboxItemViewModel(new Models.ToolboxItem { Name = "Baz", Category = "Misc" }));
        }
    }
}