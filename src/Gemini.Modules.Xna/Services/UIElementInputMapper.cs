using System.Windows;
using System.Windows.Input;
using Microsoft.Xna.Framework.Input;

namespace Gemini.Modules.Xna.Services
{
    public class UIElementInputMapper : UIElementInputMapperBase
    {
        private readonly UIElement _uiElement;
        private MouseState _currentState;
        private bool _mouseEventSinceLastUpdate;

        public UIElementInputMapper(UIElement uiElement)
            : base(uiElement)
        {
            _uiElement = uiElement;

            _uiElement.MouseMove += HandleMouseEvent;
            _uiElement.MouseWheel += HandleMouseEvent;
            _uiElement.MouseDown += HandleMouseEvent;
            _uiElement.MouseUp += HandleMouseEvent;
        }

        protected void HandleMouseEvent(object sender, MouseEventArgs e)
        {
            _mouseEventSinceLastUpdate = true;

            var position = e.GetPosition(_uiElement);
            var wheelDelta = (e is MouseWheelEventArgs)
                ? ((MouseWheelEventArgs) e).Delta
                : 0;

            _currentState = new MouseState(
                (int) position.X, (int) position.Y, wheelDelta,
                MapButtonState(e.LeftButton),
                MapButtonState(e.MiddleButton),
                MapButtonState(e.RightButton),
                MapButtonState(e.XButton1),
                MapButtonState(e.XButton2));
        }

        public override MouseState GetMouseState()
        {
            if (!_mouseEventSinceLastUpdate)
            {
                // Reset scroll wheel value if no scroll wheel events have happened since the last time 
                // GetMouseState() was called.
                _currentState = new MouseState(
                    _currentState.X, _currentState.Y, 0,
                    _currentState.LeftButton, _currentState.MiddleButton, _currentState.RightButton,
                    _currentState.XButton1, _currentState.XButton2);
            }
            _mouseEventSinceLastUpdate = false;
            return _currentState;
        }

        protected override void OnDisposing()
        {
            _uiElement.MouseMove -= HandleMouseEvent;
            _uiElement.MouseWheel -= HandleMouseEvent;
            _uiElement.MouseDown -= HandleMouseEvent;
            _uiElement.MouseUp -= HandleMouseEvent;

            base.OnDisposing();
        }
    }
}