﻿using System.ComponentModel.Composition;
using Caliburn.Micro;
using Gemini.Modules.ToolBars.Controls;
using Gemini.Modules.ToolBars.Views;

namespace Gemini.Modules.ToolBars.ViewModels
{
    [Export(typeof(IToolBars))]
    public class ToolBarsViewModel : ViewAware, IToolBars
    {
        private readonly BindableCollection<IToolBar> _items;
        public IObservableCollection<IToolBar> Items
        {
            get { return _items; }
        }

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

        public ToolBarsViewModel()
        {
            _items = new BindableCollection<IToolBar>();
        }

        public void Add(params IToolBar[] items)
        {
            foreach (var item in items)
            {
                Items.Add(item);
            }
        }

        protected override void OnViewLoaded(object view)
        {
            // TODO: Ideally, the ToolBarTray control would expose ToolBars
            // as a dependency property. We could use an attached property
            // to workaround this. But for now, toolbars need to be
            // created prior to the following code being run.
            foreach (var toolBar in Items)
                ((IToolBarsView) view).ToolBarTray.ToolBars.Add(new ToolBarEx
                {
                    ItemsSource = toolBar
                });

            base.OnViewLoaded(view);
        }
    }
}