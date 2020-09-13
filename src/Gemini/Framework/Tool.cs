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
        public override ICommand CloseCommand => _closeCommand ?? (_closeCommand = new AsyncCommand(() => TryCloseAsync(null)));

        public abstract PaneLocation PreferredLocation { get; }

        public virtual double PreferredWidth => 200;

        public virtual double PreferredHeight => 200;

        private bool _isVisible;
        public bool IsVisible
        {
            get => _isVisible;
            set
            {
                _isVisible = value;
                NotifyOfPropertyChange(() => IsVisible);
            }
        }

        private ToolBarDefinition _toolBarDefinition;
        public ToolBarDefinition ToolBarDefinition
        {
            get => _toolBarDefinition;
            protected set
            {
                _toolBarDefinition = value;
                NotifyOfPropertyChange(() => ToolBar);
                NotifyOfPropertyChange();
            }
        }

        private IToolBar _toolBar;
        public IToolBar ToolBar
        {
            get
            {
                if (_toolBar != null)
                    return _toolBar;

                if (ToolBarDefinition == null)
                    return null;

                var toolBarBuilder = IoC.Get<IToolBarBuilder>();
                _toolBar = new ToolBarModel();
                toolBarBuilder.BuildToolBar(ToolBarDefinition, _toolBar);
                return _toolBar;
            }
        }

        // Tool windows should always reopen on app start by default.
        public override bool ShouldReopenOnStart => true;

        protected Tool()
        {
            IsVisible = true;
        }

        public override Task TryCloseAsync(bool? dialogResult = null)
        {
            IsVisible = false;
            return base.TryCloseAsync(dialogResult);
        }
    };
}
