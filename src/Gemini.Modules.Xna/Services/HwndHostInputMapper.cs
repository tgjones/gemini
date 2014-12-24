using Gemini.Framework.Controls;
using Gemini.Modules.Xna.Controls;
using Microsoft.Xna.Framework.Input;

namespace Gemini.Modules.Xna.Services
{
    public class HwndHostInputMapper : UIElementInputMapperBase
    {
        private readonly GraphicsDeviceControl _hwndHost;
        private MouseState _currentState;
        private bool _mouseEventSinceLastUpdate;

        public HwndHostInputMapper(GraphicsDeviceControl hwndHost)
            : base(hwndHost)
        {
            _hwndHost = hwndHost;

            _hwndHost.HwndMouseMove += HandleMouseEvent;
            _hwndHost.HwndMouseWheel += HandleMouseEvent;
            _hwndHost.HwndLButtonDown += HandleMouseEvent;
            _hwndHost.HwndLButtonUp += HandleMouseEvent;
            _hwndHost.HwndRButtonDown += HandleMouseEvent;
            _hwndHost.HwndRButtonUp += HandleMouseEvent;
            _hwndHost.HwndMButtonDown += HandleMouseEvent;
            _hwndHost.HwndMButtonUp += HandleMouseEvent;
            _hwndHost.HwndX1ButtonDown += HandleMouseEvent;
            _hwndHost.HwndX1ButtonUp += HandleMouseEvent;
            _hwndHost.HwndX2ButtonDown += HandleMouseEvent;
            _hwndHost.HwndX2ButtonUp += HandleMouseEvent;
        }

        private void HandleMouseEvent(object sender, HwndMouseEventArgs e)
        {
            _mouseEventSinceLastUpdate = true;
            var position = e.GetPosition(_hwndHost);
            _currentState = new MouseState(
                (int) position.X, (int) position.Y, e.WheelDelta,
                MapButtonState(e.LeftButton),
                MapButtonState(e.MiddleButton),
                MapButtonState(e.RightButton),
                MapButtonState(e.X1Button),
                MapButtonState(e.X2Button));
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
            _hwndHost.HwndMouseMove -= HandleMouseEvent;
            _hwndHost.HwndMouseWheel -= HandleMouseEvent;
            _hwndHost.HwndLButtonDown -= HandleMouseEvent;
            _hwndHost.HwndLButtonUp -= HandleMouseEvent;
            _hwndHost.HwndRButtonDown -= HandleMouseEvent;
            _hwndHost.HwndRButtonUp -= HandleMouseEvent;
            _hwndHost.HwndMButtonDown -= HandleMouseEvent;
            _hwndHost.HwndMButtonUp -= HandleMouseEvent;
            _hwndHost.HwndX1ButtonDown -= HandleMouseEvent;
            _hwndHost.HwndX1ButtonUp -= HandleMouseEvent;
            _hwndHost.HwndX2ButtonDown -= HandleMouseEvent;
            _hwndHost.HwndX2ButtonUp -= HandleMouseEvent;

            base.OnDisposing();
        }
    }
}