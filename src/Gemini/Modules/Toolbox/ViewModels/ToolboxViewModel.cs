using System;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using Caliburn.Micro;
using Gemini.Framework;
using Gemini.Framework.Services;
using Gemini.Modules.Toolbox.Services;

namespace Gemini.Modules.Toolbox.ViewModels
{
    [Export(typeof(IToolbox))]
    public class ToolboxViewModel : Tool, IToolbox
    {
        private readonly IToolboxService _toolboxService;

        public override PaneLocation PreferredLocation
        {
            get { return PaneLocation.Left; }
        }

        private readonly BindableCollection<ToolboxItemViewModel> _items;
        public IObservableCollection<ToolboxItemViewModel> Items
        {
            get { return _items; }
        }

        [ImportingConstructor]
        public ToolboxViewModel(IShell shell, IToolboxService toolboxService)
        {
            DisplayName = "Toolbox";

            _items = new BindableCollection<ToolboxItemViewModel>();
            
            var groupedItems = CollectionViewSource.GetDefaultView(_items);
            groupedItems.GroupDescriptions.Add(new PropertyGroupDescription("Category"));

            _toolboxService = toolboxService;

            if (DesignerProperties.GetIsInDesignMode(new DependencyObject()))
                return;

            shell.ActiveDocumentChanged += (sender, e) => RefreshToolboxItems(shell);
            RefreshToolboxItems(shell);
        }

        private void RefreshToolboxItems(IShell shell)
        {
            _items.Clear();

            if (shell.ActiveItem == null) 
                return;

            _items.AddRange(_toolboxService.GetToolboxItems(shell.ActiveItem.GetType())
                .Select(x => new ToolboxItemViewModel(x)));
        }
    }
}