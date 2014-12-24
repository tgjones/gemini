using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Microsoft.Xna.Framework.Input;

namespace Gemini.Modules.Xna.Services
{
    public abstract class UIElementInputMapperBase : InputMapperBase, IDisposable
    {
        private readonly UIElement _uiElement;
        private readonly List<Key> _pressedKeys;

        protected UIElementInputMapperBase(UIElement uiElement)
        {
            _uiElement = uiElement;
            _pressedKeys = new List<Key>();

            _uiElement.KeyDown += HandleKeyDown;
            _uiElement.KeyUp += HandleKeyUp;
        }

        private void HandleKeyDown(object sender, KeyEventArgs e)
        {
            e.Handled = true;
            var key = GetKey(e);
            lock (_pressedKeys)
                if (!_pressedKeys.Contains(key))
                    _pressedKeys.Add(key);
        }

        private void HandleKeyUp(object sender, KeyEventArgs e)
        {
            e.Handled = true;
            lock (_pressedKeys)
                _pressedKeys.Remove(GetKey(e));
        }

        private static Key GetKey(KeyEventArgs e)
        {
            return (e.SystemKey != Key.None) ? e.SystemKey : e.Key;
        }

        public override KeyboardState GetKeyboardState()
        {
            lock (_pressedKeys)
                return new KeyboardState(_pressedKeys.Select(MapKey).ToArray());
        }

        public void Dispose()
        {
            OnDisposing();
        }

        protected virtual void OnDisposing()
        {
            _uiElement.KeyDown -= HandleKeyDown;
            _uiElement.KeyUp -= HandleKeyUp;
        }
    }
}