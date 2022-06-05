using System.Threading.Tasks;
using System.Windows.Input;
using Caliburn.Micro;
using Gemini.Framework.Services;
using Gemini.Framework.ToolBars;
using Gemini.Modules.ToolBars;
using Gemini.Modules.ToolBars.Models;

namespace Gemini.Framework
{
    public abstract class Tool : LayoutItemBase, ITool
    {
        private ICommand _closeCommand;
        public override ICommand CloseCommand => _closeCommand ?? (_closeCommand = new AsyncCommand(() => TryCloseAsync()));

        public abstract PaneLocation PreferredLocation { get; }

        public virtual double PreferredWidth => 200;

        public virtual double PreferredHeight => 200;

        private bool _isVisible = true;
        public bool IsVisible
        {
            get => _isVisible;
            set => Set(ref _isVisible, value);
        }

        private ToolBarDefinition _toolBarDefinition;
        public ToolBarDefinition ToolBarDefinition
        {
            get => _toolBarDefinition;
            protected set
            {
                if (Set(ref _toolBarDefinition, value))
                {
                    if (ToolBarDefinition is null)
                    {
                        ToolBar = null;
                        return;
                    }

                    var toolBarBuilder = IoC.Get<IToolBarBuilder>();
                    var toolBar = new ToolBarModel();
                    toolBarBuilder.BuildToolBar(ToolBarDefinition, toolBar);
                    ToolBar = toolBar;
                }
            }
        }

        private IToolBar _toolBar;
        public IToolBar ToolBar
        {
            get => _toolBar;
            private set => Set(ref _toolBar, value);
        }

        // Tool windows should always reopen on app start by default.
        public override bool ShouldReopenOnStart => true;

        public override Task TryCloseAsync(bool? dialogResult = null)
        {
            IsVisible = false;
            return base.TryCloseAsync(dialogResult);
        }
    };
}
