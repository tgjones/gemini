using System.Collections;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Gemini.Modules.GraphEditor.Controls
{
    // Inspired by reading http://www.codeproject.com/Articles/85603/A-WPF-custom-control-for-zooming-and-panning
    // Thank you Ashley Davis!
    public class GraphControl : Control
    {
        private ElementItemsControl _elementItemsControl = null;

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

        public IList SelectedElements
        {
            get { return _elementItemsControl.SelectedItems; }
        }

        public override void OnApplyTemplate()
        {
            _elementItemsControl = (ElementItemsControl) Template.FindName("PART_ElementItemsControl", this);
            base.OnApplyTemplate();
        }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            _elementItemsControl.SelectedItems.Clear();
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
    }
}