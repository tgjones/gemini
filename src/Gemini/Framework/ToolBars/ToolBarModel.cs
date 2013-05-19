using Caliburn.Micro;

namespace Gemini.Framework.ToolBars
{
	public class ToolBarModel : BindableCollection<ToolBarItemBase>, IToolBar
	{
        public void Add(params ToolBarItemBase[] items)
		{
			items.Apply(Add);
		}
	}
}