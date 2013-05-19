using System.ComponentModel.Composition;
using Caliburn.Micro;

namespace Gemini.Framework.ToolBars
{
    [Export(typeof(IToolBars))]
    public class ToolBars : BindableCollection<IToolBar>, IToolBars
    {
        private bool _visible;
        public bool Visible
        {
            get { return _visible; }
            set
            {
                _visible = value;
                NotifyOfPropertyChange("Visible");
            }
        }

        public void Add(params IToolBar[] items)
        {
            items.Apply(Add);
        }
    }
}