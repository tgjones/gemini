using System.Collections;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace Gemini.Modules.GraphEditor.Controls
{
    // Inspired by studying http://www.codeproject.com/Articles/182683/NetworkView-A-WPF-custom-control-for-visualizing-a
    // Thank you Ashley Davis!
    public class GraphControl : Control
    {
        private ElementItemsControl _elementItemsControl;
        private ConnectionItemsControl _connectionItemsControl;

        static GraphControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(GraphControl),
                new FrameworkPropertyMetadata(typeof(GraphControl)));
        }

        #region Dependency properties

        public static readonly DependencyProperty ElementsSourceProperty = DependencyProperty.Register(
            "ElementsSource", typeof(IEnumerable), typeof(GraphControl));

        public IEnumerable ElementsSource
        {
            get { return (IEnumerable) GetValue(ElementsSourceProperty); }
            set { SetValue(ElementsSourceProperty, value); }
        }

        public static readonly DependencyProperty ElementItemContainerStyleProperty = DependencyProperty.Register(
            "ElementItemContainerStyle", typeof(Style), typeof(GraphControl));

        public Style ElementItemContainerStyle
        {
            get { return (Style) GetValue(ElementItemContainerStyleProperty); }
            set { SetValue(ElementItemContainerStyleProperty, value); }
        }

        public static readonly DependencyProperty ElementItemTemplateProperty = DependencyProperty.Register(
            "ElementItemTemplate", typeof(DataTemplate), typeof(GraphControl));

        public DataTemplate ElementItemTemplate
        {
            get { return (DataTemplate) GetValue(ElementItemTemplateProperty); }
            set { SetValue(ElementItemTemplateProperty, value); }
        }

        public static readonly DependencyProperty ElementItemDataTemplateSelectorProperty = DependencyProperty.Register(
            "ElementItemDataTemplateSelector", typeof(DataTemplateSelector), typeof(GraphControl), new PropertyMetadata(null));

        public DataTemplateSelector ElementItemDataTemplateSelector
        {
            get { return (DataTemplateSelector) GetValue(ElementItemDataTemplateSelectorProperty); }
            set { SetValue(ElementItemDataTemplateSelectorProperty, value); }
        }

        public static readonly DependencyProperty ConnectionItemDataTemplateSelectorProperty = DependencyProperty.Register(
            "ConnectionItemDataTemplateSelector", typeof(DataTemplateSelector), typeof(GraphControl), new PropertyMetadata(null));

        public DataTemplateSelector ConnectionItemDataTemplateSelector
        {
            get { return (DataTemplateSelector) GetValue(ConnectionItemDataTemplateSelectorProperty); }
            set { SetValue(ConnectionItemDataTemplateSelectorProperty, value); }
        }

        public static readonly DependencyProperty ConnectionsSourceProperty = DependencyProperty.Register(
            "ConnectionsSource", typeof(IEnumerable), typeof(GraphControl));

        public IEnumerable ConnectionsSource
        {
            get { return (IEnumerable) GetValue(ConnectionsSourceProperty); }
            set { SetValue(ConnectionsSourceProperty, value); }
        }

        public static readonly DependencyProperty ConnectionItemContainerStyleProperty = DependencyProperty.Register(
            "ConnectionItemContainerStyle", typeof(Style), typeof(GraphControl));

        public Style ConnectionItemContainerStyle
        {
            get { return (Style) GetValue(ConnectionItemContainerStyleProperty); }
            set { SetValue(ConnectionItemContainerStyleProperty, value); }
        }

        public static readonly DependencyProperty ConnectionItemTemplateProperty = DependencyProperty.Register(
            "ConnectionItemTemplate", typeof(DataTemplate), typeof(GraphControl));

        public DataTemplate ConnectionItemTemplate
        {
            get { return (DataTemplate) GetValue(ConnectionItemTemplateProperty); }
            set { SetValue(ConnectionItemTemplateProperty, value); }
        }

        #endregion

        #region Routed events

        public static readonly RoutedEvent ConnectionDragStartedEvent = EventManager.RegisterRoutedEvent(
            "ConnectionDragStarted", RoutingStrategy.Bubble, typeof(ConnectionDragStartedEventHandler), 
            typeof(GraphControl));

        public static readonly RoutedEvent ConnectionDraggingEvent = EventManager.RegisterRoutedEvent(
            "ConnectionDragging", RoutingStrategy.Bubble, typeof(ConnectionDraggingEventHandler), 
            typeof(GraphControl));

        public static readonly RoutedEvent ConnectionDragCompletedEvent = EventManager.RegisterRoutedEvent(
            "ConnectionDragCompleted", RoutingStrategy.Bubble, typeof(ConnectionDragCompletedEventHandler), 
            typeof(GraphControl));

        public event ConnectionDragStartedEventHandler ConnectionDragStarted
        {
            add { AddHandler(ConnectionDragStartedEvent, value); }
            remove { RemoveHandler(ConnectionDragStartedEvent, value); }
        }

        public event ConnectionDraggingEventHandler ConnectionDragging
        {
            add { AddHandler(ConnectionDraggingEvent, value); }
            remove { RemoveHandler(ConnectionDraggingEvent, value); }
        }

        public event ConnectionDragCompletedEventHandler ConnectionDragCompleted
        {
            add { AddHandler(ConnectionDragCompletedEvent, value); }
            remove { RemoveHandler(ConnectionDragCompletedEvent, value); }
        }

        #endregion

        public event SelectionChangedEventHandler SelectionChanged;
        public event SelectionChangedEventHandler ConnectionSelectionChanged;
        public IList SelectedElements
        {
            get { return _elementItemsControl.SelectedItems; }
        }

        public IList SelectedConnections
        {
            get { return _connectionItemsControl.SelectedItems; }
        }

        public void SelectAll()
        {
            _elementItemsControl.SelectAll();
            _connectionItemsControl.SelectAll();
        }

        public void UnselectAll()
        {
            _elementItemsControl.UnselectAll();
            _connectionItemsControl.UnselectAll();
        }

        public GraphControl()
        {
            AddHandler(ConnectorItem.ConnectorDragStartedEvent, new ConnectorItemDragStartedEventHandler(OnConnectorItemDragStarted));
            AddHandler(ConnectorItem.ConnectorDraggingEvent, new ConnectorItemDraggingEventHandler(OnConnectorItemDragging));
            AddHandler(ConnectorItem.ConnectorDragCompletedEvent, new ConnectorItemDragCompletedEventHandler(OnConnectorItemDragCompleted));
        }

        public override void OnApplyTemplate()
        {
            _elementItemsControl = (ElementItemsControl) Template.FindName("PART_ElementItemsControl", this);
            _elementItemsControl.SelectionChanged += OnElementItemsControlSelectChanged;
            _connectionItemsControl = ((ConnectionItemsControl) Template.FindName("PART_ConnectionItemsControl", this));
            _connectionItemsControl.SelectionChanged += OnConnectionsControlSelectionChanged;
            base.OnApplyTemplate();
        }

        private void OnConnectionsControlSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ConnectionSelectionChanged?.Invoke(this, e);
        }

        private void OnElementItemsControlSelectChanged(object sender, SelectionChangedEventArgs e)
        {
            var handler = SelectionChanged;
            if (handler != null)
                handler(this, new SelectionChangedEventArgs(Selector.SelectionChangedEvent, e.RemovedItems, e.AddedItems));
        }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            _elementItemsControl.SelectedItems.Clear();
            _connectionItemsControl.SelectedItems.Clear();
            base.OnMouseLeftButtonDown(e);
        }

        internal int GetMaxZIndex()
        {
            return _elementItemsControl.Items.Cast<object>()
                .Select(item => (ElementItem) _elementItemsControl.ItemContainerGenerator.ContainerFromItem(item))
                .Select(elementItem => elementItem.ZIndex)
                .Concat(new[] { 0 })
                .Max();
        }

        #region Connection dragging

        private ConnectorItem _draggingSourceConnector;
        private object _draggingConnectionDataContext;

        private void OnConnectorItemDragStarted(object sender, ConnectorItemDragStartedEventArgs e)
        {
            e.Handled = true;

            _draggingSourceConnector = (ConnectorItem) e.OriginalSource;

            var eventArgs = new ConnectionDragStartedEventArgs(ConnectionDragStartedEvent, this, 
                _draggingSourceConnector.ParentElementItem, _draggingSourceConnector);
            RaiseEvent(eventArgs);

            _draggingConnectionDataContext = eventArgs.Connection;

            if (_draggingConnectionDataContext == null)
                e.Cancel = true;
        }

        private void OnConnectorItemDragging(object sender, ConnectorItemDraggingEventArgs e)
        {
            e.Handled = true;

            var connectionDraggingEventArgs =
                new ConnectionDraggingEventArgs(ConnectionDraggingEvent, this,
                    _draggingSourceConnector.ParentElementItem, _draggingConnectionDataContext,
                    _draggingSourceConnector);
            RaiseEvent(connectionDraggingEventArgs);
        }

        private void OnConnectorItemDragCompleted(object sender, ConnectorItemDragCompletedEventArgs e)
        {
            e.Handled = true;

            RaiseEvent(new ConnectionDragCompletedEventArgs(ConnectionDragCompletedEvent, this, 
                _draggingSourceConnector.ParentElementItem, _draggingConnectionDataContext,
                _draggingSourceConnector));
             
            _draggingSourceConnector = null;
            _draggingConnectionDataContext = null;
        }

        #endregion
    }
}
