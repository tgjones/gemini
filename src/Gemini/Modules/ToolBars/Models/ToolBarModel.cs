using Caliburn.Micro;

namespace Gemini.Modules.ToolBars.Models
{
	public class ToolBarModel : BindableCollection<ToolBarItemBase>, IToolBar
	{
        public void Add(params ToolBarItemBase[] items)
		{
			items.Apply(Add);
		}
	}
}