using System;
using System.ComponentModel;
using System.Text;
using System.Windows;
using System.Windows.Markup;
using System.Xml;
using Caliburn.Micro;
using Gemini.Framework.Services;

namespace Gemini.Modules.StatusBar.Controls
{
    [MarkupExtensionReturnType(typeof(Style))]
    public class DynamicStyleExtension : MarkupExtension
    {
        [ConstructorArgument("resourceKey")]
        public object ResourceKey { get; set; }

        [DefaultValue(null)]
        public object BasedOn { get; set; }

        public DynamicStyleExtension(object resourceKey)
        {
            ResourceKey = resourceKey;
        }

        public DynamicStyleExtension()
        {
            
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            if (ResourceKey == null)
                return null;

            var provideValueTarget = serviceProvider.GetService(typeof(IProvideValueTarget)) as IProvideValueTarget;
            if (provideValueTarget != null)
            {
                var targetObject = provideValueTarget.TargetObject as FrameworkElement;
                var targetProperty = provideValueTarget.TargetProperty as DependencyProperty;

                if (targetObject != null)
                {
                    var style = targetObject.TryFindResource(ResourceKey) as Style;
                    if (style == null)
                        return null;

                    if (targetProperty != null)
                        IoC.Get<IShell>().CurrentThemeChanged += (sender, e) =>
                        {
                            var newStyle = CloneStyle(style, GetBasedOnStyle(targetObject));
                            targetObject.SetValue(targetProperty, newStyle);
                        };

                    style.BasedOn = GetBasedOnStyle(targetObject);
                    return style;
                }
            }

            return null;
        }

        private Style GetBasedOnStyle(FrameworkElement frameworkElement)
        {
            if (frameworkElement == null)
                return null;

            return frameworkElement.TryFindResource(BasedOn) as Style;
        }

        private static Style CloneStyle(Style style, Style newBasedOn)
        {
            var sb = new StringBuilder();
            var writer = XmlWriter.Create(sb, new XmlWriterSettings
            {
                Indent = true,
                ConformanceLevel = ConformanceLevel.Fragment,
                OmitXmlDeclaration = true,
                NamespaceHandling = NamespaceHandling.OmitDuplicates,
            });

            var mgr = new XamlDesignerSerializationManager(writer)
            {
                XamlWriterMode = XamlWriterMode.Expression
            };

            XamlWriter.Save(style, mgr);

            var clonedStyle = (Style) XamlReader.Parse(sb.ToString());
            clonedStyle.BasedOn = newBasedOn;
            return clonedStyle;
        }
    }
}