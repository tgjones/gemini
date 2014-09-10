using System;
using System.Windows;
using System.Windows.Controls;

namespace Gemini.Framework.Controls
{
    // Based on http://stackoverflow.com/questions/9490264/dynamicresource-for-style-basedon
    public class DynamicStyle
    {
        public static readonly DependencyProperty BaseStyleProperty = DependencyProperty.RegisterAttached(
            "BaseStyle", typeof(Style), typeof(DynamicStyle),
            new PropertyMetadata(OnStylesChanged));

        public static Style GetBaseStyle(DependencyObject obj)
        {
            return (Style) obj.GetValue(BaseStyleProperty);
        }

        public static void SetBaseStyle(DependencyObject obj, Style value)
        {
            obj.SetValue(BaseStyleProperty, value);
        }

        public static readonly DependencyProperty DerivedStyleProperty = DependencyProperty.RegisterAttached(
            "DerivedStyle", typeof(Style), typeof(DynamicStyle),
            new PropertyMetadata(OnStylesChanged));

        public static Style GetDerivedStyle(DependencyObject obj)
        {
            return (Style) obj.GetValue(DerivedStyleProperty);
        }

        public static void SetDerivedStyle(DependencyObject obj, Style value)
        {
            obj.SetValue(DerivedStyleProperty, value);
        }

        private static void OnStylesChanged(DependencyObject target, DependencyPropertyChangedEventArgs e)
        {
            var mergedStyles = GetMergedStyles<FrameworkElement>(target, GetBaseStyle(target), GetDerivedStyle(target));
            var element = (FrameworkElement) target;
            element.Style = mergedStyles;
        }

        public static readonly DependencyProperty ItemContainerBaseStyleProperty = DependencyProperty.RegisterAttached(
            "ItemContainerBaseStyle", typeof(Style), typeof(DynamicStyle),
            new PropertyMetadata(OnItemContainerStylesChanged));

        public static Style GetItemContainerBaseStyle(DependencyObject obj)
        {
            return (Style) obj.GetValue(ItemContainerBaseStyleProperty);
        }

        public static void SetItemContainerBaseStyle(DependencyObject obj, Style value)
        {
            obj.SetValue(ItemContainerBaseStyleProperty, value);
        }

        public static readonly DependencyProperty ItemContainerDerivedStyleProperty = DependencyProperty.RegisterAttached(
            "ItemContainerDerivedStyle", typeof(Style), typeof(DynamicStyle),
            new PropertyMetadata(OnItemContainerStylesChanged));

        public static Style GetItemContainerDerivedStyle(DependencyObject obj)
        {
            return (Style) obj.GetValue(ItemContainerDerivedStyleProperty);
        }

        public static void SetItemContainerDerivedStyle(DependencyObject obj, Style value)
        {
            obj.SetValue(ItemContainerDerivedStyleProperty, value);
        }

        private static void OnItemContainerStylesChanged(DependencyObject target, DependencyPropertyChangedEventArgs e)
        {
            var mergedStyles = GetMergedStyles<ItemsControl>(target,
                GetItemContainerBaseStyle(target), 
                GetItemContainerDerivedStyle(target));
            var element = (ItemsControl) target;
            element.ItemContainerStyle = mergedStyles;
        }

        public static Style GetMergedStyles<T>(DependencyObject target, Style baseStyle, Style derivedStyle)
            where T : DependencyObject
        {
            if (!(target is T))
                throw new InvalidCastException("Target must be " + typeof (T));

            if (derivedStyle == null) return baseStyle;
            if (baseStyle == null) return derivedStyle;

            var newStyle = new Style {BasedOn = baseStyle, TargetType = derivedStyle.TargetType};
            foreach (var setter in derivedStyle.Setters) newStyle.Setters.Add(setter);
            foreach (var trigger in derivedStyle.Triggers) newStyle.Triggers.Add(trigger);
            return newStyle;
        }
    }
}