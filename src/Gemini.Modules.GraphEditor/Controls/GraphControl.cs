using System.Collections;
using System.Windows;
using System.Windows.Controls;

namespace Gemini.Modules.GraphEditor.Controls
{
    public class GraphControl : Control
    {
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
    }
}