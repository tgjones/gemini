using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Windows;

namespace Gemini.Framework.Services
{
    public sealed class SettingsPropertyChangedEventManager<TApplicationSettings>
        where TApplicationSettings : INotifyPropertyChanged
    {
        private readonly TApplicationSettings _applicationSettings;
        private readonly List<IWeakEventListener> _eventListeners = new List<IWeakEventListener>();

        public SettingsPropertyChangedEventManager(TApplicationSettings applicationSettings)
        {
            _applicationSettings = applicationSettings;
        }

        public void AddListener<TSettingsProperty>(
            Expression<Func<TApplicationSettings, TSettingsProperty>> property,
            Action<TSettingsProperty> onPropertyChanged)
        {
            string propertyName = ExtensionMethods.GetPropertyName(property);

            var listener = new PropertyChangedEventListener<TSettingsProperty>(_applicationSettings, property,
                onPropertyChanged);
            PropertyChangedEventManager.AddListener(_applicationSettings, listener, propertyName);

            _eventListeners.Add(listener);
        }

        private class PropertyChangedEventListener<TSettingsProperty> : IWeakEventListener
        {
            private readonly TApplicationSettings _applicationSettings;
            private readonly Func<TApplicationSettings, TSettingsProperty> _getSettingsProperty;
            private readonly Action<TSettingsProperty> _onPropertyChanged;

            public PropertyChangedEventListener(TApplicationSettings applicationSettings,
                Expression<Func<TApplicationSettings, TSettingsProperty>> property,
                Action<TSettingsProperty> onPropertyChanged)
            {
                _applicationSettings = applicationSettings;
                _onPropertyChanged = onPropertyChanged;
                _getSettingsProperty = property.Compile();
            }

            public bool ReceiveWeakEvent(Type managerType, object sender, EventArgs e)
            {
                _onPropertyChanged(_getSettingsProperty(_applicationSettings));

                return true;
            }
        }
    }
}