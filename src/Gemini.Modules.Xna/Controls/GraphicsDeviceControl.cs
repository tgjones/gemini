﻿#region File Description
//-----------------------------------------------------------------------------
// Copyright 2011, Nick Gravelyn.
// Licensed under the terms of the Ms-PL: 
// http://www.microsoft.com/opensource/licenses.mspx#Ms-PL
//-----------------------------------------------------------------------------
#endregion

using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using Gemini.Modules.Xna.Util;
using Microsoft.Xna.Framework.Graphics;

namespace Gemini.Modules.Xna.Controls
{
    /// <summary>
    /// A control that enables XNA graphics rendering inside a WPF control through
    /// the use of a hosted child Hwnd.
    /// </summary>
    public class GraphicsDeviceControl : HwndHost
    {
        #region Fields

        // The name of our window class
        private const string WindowClass = "GraphicsDeviceControlHostWindowClass";

        // The HWND we present to when rendering
        private IntPtr _hWnd;

        // The GraphicsDeviceService that provides and manages our GraphicsDevice
        private GraphicsDeviceService _graphicsService;

        // Track if the application has focus
        private bool _applicationHasFocus;

        // Track if the mouse is in the window
        private bool _mouseInWindow;

        // Track the mouse state
        private readonly HwndMouseState _mouseState = new HwndMouseState();

        // Tracking whether we've "capture" the mouse
        private bool _isMouseCaptured;

        // The screen coordinates of the mouse when captured
        private int _capturedMouseX;
        private int _capturedMouseY;

        // The client coordinates of the mouse when captured
        private int _capturedMouseClientX;
        private int _capturedMouseClientY;

        #endregion

        #region Events

        /// <summary>
        /// Invoked when the control has initialized the GraphicsDevice.
        /// </summary>
        public event EventHandler<GraphicsDeviceEventArgs> LoadContent;

        /// <summary>
        /// Invoked when the control is ready to render XNA content
        /// </summary>
        public event EventHandler<GraphicsDeviceEventArgs> RenderXna;

        /// <summary>
        /// Invoked when the control receives a left mouse down message.
        /// </summary>
        public event EventHandler<HwndMouseEventArgs> HwndLButtonDown;

        /// <summary>
        /// Invoked when the control receives a left mouse up message.
        /// </summary>
        public event EventHandler<HwndMouseEventArgs> HwndLButtonUp;

        /// <summary>
        /// Invoked when the control receives a left mouse double click message.
        /// </summary>
        public event EventHandler<HwndMouseEventArgs> HwndLButtonDblClick;

        /// <summary>
        /// Invoked when the control receives a right mouse down message.
        /// </summary>
        public event EventHandler<HwndMouseEventArgs> HwndRButtonDown;

        /// <summary>
        /// Invoked when the control receives a right mouse up message.
        /// </summary>
        public event EventHandler<HwndMouseEventArgs> HwndRButtonUp;

        /// <summary>
        /// Invoked when the control receives a rigt mouse double click message.
        /// </summary>
        public event EventHandler<HwndMouseEventArgs> HwndRButtonDblClick;

        /// <summary>
        /// Invoked when the control receives a middle mouse down message.
        /// </summary>
        public event EventHandler<HwndMouseEventArgs> HwndMButtonDown;

        /// <summary>
        /// Invoked when the control receives a middle mouse up message.
        /// </summary>
        public event EventHandler<HwndMouseEventArgs> HwndMButtonUp;

        /// <summary>
        /// Invoked when the control receives a middle mouse double click message.
        /// </summary>
        public event EventHandler<HwndMouseEventArgs> HwndMButtonDblClick;

        /// <summary>
        /// Invoked when the control receives a mouse down message for the first extra button.
        /// </summary>
        public event EventHandler<HwndMouseEventArgs> HwndX1ButtonDown;

        /// <summary>
        /// Invoked when the control receives a mouse up message for the first extra button.
        /// </summary>
        public event EventHandler<HwndMouseEventArgs> HwndX1ButtonUp;

        /// <summary>
        /// Invoked when the control receives a double click message for the first extra mouse button.
        /// </summary>
        public event EventHandler<HwndMouseEventArgs> HwndX1ButtonDblClick;

        /// <summary>
        /// Invoked when the control receives a mouse down message for the second extra button.
        /// </summary>
        public event EventHandler<HwndMouseEventArgs> HwndX2ButtonDown;

        /// <summary>
        /// Invoked when the control receives a mouse up message for the second extra button.
        /// </summary>
        public event EventHandler<HwndMouseEventArgs> HwndX2ButtonUp;

        /// <summary>
        /// Invoked when the control receives a double click message for the first extra mouse button.
        /// </summary>
        public event EventHandler<HwndMouseEventArgs> HwndX2ButtonDblClick;

        /// <summary>
        /// Invoked when the control receives a mouse move message.
        /// </summary>
        public event EventHandler<HwndMouseEventArgs> HwndMouseMove;

        /// <summary>
        /// Invoked when the control first gets a mouse move message.
        /// </summary>
        public event EventHandler<HwndMouseEventArgs> HwndMouseEnter;

        /// <summary>
        /// Invoked when the control gets a mouse leave message.
        /// </summary>
        public event EventHandler<HwndMouseEventArgs> HwndMouseLeave;

        #endregion

        #region Construction and Disposal

        public GraphicsDeviceControl()
        {
            // We must be notified of the control finishing loading so we can get the GraphicsDeviceService
            Loaded += OnXnaWindowHostLoaded;

            // We must be notified of the control changing sizes so we can resize the GraphicsDeviceService
            SizeChanged += OnXnaWindowHostSizeChanged;

            // We must be notified of the application foreground status for our mouse input events
            Application.Current.Activated += OnApplicationActivated;
            Application.Current.Deactivated += OnApplicationDeactivated;

            // We use the CompositionTarget.Rendering event to trigger the control to draw itself
            CompositionTarget.Rendering += OnCompositionTargetRendering;
        }

        protected override void Dispose(bool disposing)
        {
            // Release our reference to the GraphicsDeviceService if we have one
            if (_graphicsService != null)
            {
                _graphicsService.Release(disposing);
                _graphicsService = null;
            }

            // Unhook the Rendering event so we no longer attempt to draw
            CompositionTarget.Rendering -= OnCompositionTargetRendering;

            base.Dispose(disposing);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Captures the mouse, hiding it and trapping it inside the window bounds.
        /// </summary>
        /// <remarks>
        /// This method is useful for tooling scenarios where you only care about the mouse deltas
        /// and want the user to be able to continue interacting with the window while they move
        /// the mouse. A good example of this is rotating an object based on the mouse deltas where
        /// through capturing you can spin and spin without having the cursor leave the window.
        /// </remarks>
        public new void CaptureMouse()
        {
            // Don't do anything if the mouse is already captured
            if (_isMouseCaptured)
                return;

            NativeMethods.ShowCursor(false);
            _isMouseCaptured = true;

            // Store the current cursor position so we can reset the cursor back
            // whenever we get a move message
            var p = new NativeMethods.POINT();
            NativeMethods.GetCursorPos(ref p);
            _capturedMouseX = p.X;
            _capturedMouseY = p.Y;

            // Get the client position of this point
            NativeMethods.ScreenToClient(_hWnd, ref p);
            _capturedMouseClientX = p.X;
            _capturedMouseClientY = p.Y;
        }

        /// <summary>
        /// Releases the capture of the mouse which makes it visible and allows it to leave the window bounds.
        /// </summary>
        public new void ReleaseMouseCapture()
        {
            // Don't do anything if the mouse isn't captured
            if (!_isMouseCaptured)
                return;

            NativeMethods.ShowCursor(true);
            _isMouseCaptured = false;
        }

        #endregion

        #region Graphics Device Control Implementation

        private void OnCompositionTargetRendering(object sender, EventArgs e)
        {
            // If we've captured the mouse, reset the cursor back to the captured position
            if (_isMouseCaptured &&
                (int) _mouseState.Position.X != _capturedMouseX &&
                (int) _mouseState.Position.Y != _capturedMouseY)
            {
                NativeMethods.SetCursorPos(_capturedMouseX, _capturedMouseY);

                _mouseState.Position = _mouseState.PreviousPosition = new Point(_capturedMouseClientX, _capturedMouseClientY);
            }

            // If we have no graphics service, we can't draw
            if (_graphicsService == null)
                return;

            // Get the current width and height of the control
            var width = (int) ActualWidth;
            var height = (int) ActualHeight;

            // If the control has no width or no height, skip drawing since it's not visible
            if (width < 1 || height < 1)
                return;

            // This piece of code is copied from the WinForms equivalent
            string deviceResetError = HandleDeviceReset(width, height);
            if (!string.IsNullOrEmpty(deviceResetError))
                return;

            // Create the active viewport to which we'll render our content
            var viewport = new Viewport(0, 0, width, height);
            _graphicsService.GraphicsDevice.Viewport = viewport;

            // Invoke the event to render this control
            RaiseRenderXna(new GraphicsDeviceEventArgs(_graphicsService.GraphicsDevice));

            // Present to the screen, but only use the visible area of the back buffer
            _graphicsService.GraphicsDevice.Present(viewport.Bounds, null, _hWnd);
        }

        /// <summary>
        /// Helper used by BeginDraw. This checks the graphics device status,
        /// making sure it is big enough for drawing the current control, and
        /// that the device is not lost. Returns an error string if the device
        /// could not be reset.
        /// </summary>
        private string HandleDeviceReset(int width, int height)
        {
            bool deviceNeedsReset;

            switch (_graphicsService.GraphicsDevice.GraphicsDeviceStatus)
            {
                case GraphicsDeviceStatus.Lost:
                    // If the graphics device is lost, we cannot use it at all.
                    return "Graphics device lost";

                case GraphicsDeviceStatus.NotReset:
                    // If device is in the not-reset state, we should try to reset it.
                    deviceNeedsReset = true;
                    break;

                default:
                    // If the device state is ok, check whether it is big enough.
                    PresentationParameters pp = _graphicsService.GraphicsDevice.PresentationParameters;

                    deviceNeedsReset = (width > pp.BackBufferWidth) ||
                                       (height > pp.BackBufferHeight);
                    break;
            }

            // Do we need to reset the device?
            if (deviceNeedsReset)
            {
                try
                {
                    _graphicsService.ResetDevice(width, height);
                }
                catch (Exception e)
                {
                    return "Graphics device reset failed\n\n" + e;
                }
            }

            return null;
        }

        private void OnXnaWindowHostLoaded(object sender, RoutedEventArgs e)
        {
            // If we don't yet have a GraphicsDeviceService reference, we must add one for this control
            if (_graphicsService == null)
            {
                _graphicsService = GraphicsDeviceService.AddRef(_hWnd, (int) ActualWidth, (int) ActualHeight);

                // Invoke the LoadContent event
                RaiseLoadContent(new GraphicsDeviceEventArgs(_graphicsService.GraphicsDevice));
            }
        }

        protected virtual void RaiseLoadContent(GraphicsDeviceEventArgs args)
        {
            var handler = LoadContent;
            if (handler != null)
                handler(this, args);
        }

        protected virtual void RaiseRenderXna(GraphicsDeviceEventArgs args)
        {
            var handler = RenderXna;
            if (handler != null)
                handler(this, args);
        }

        private void OnXnaWindowHostSizeChanged(object sender, SizeChangedEventArgs e)
        {
            // If we have a reference to the GraphicsDeviceService, we must reset it based on our updated size
            if (_graphicsService != null)
                _graphicsService.ResetDevice((int) ActualWidth, (int) ActualHeight);
        }

        private void OnApplicationActivated(object sender, EventArgs e)
        {
            _applicationHasFocus = true;
        }

        private void OnApplicationDeactivated(object sender, EventArgs e)
        {
            _applicationHasFocus = false;
            ResetMouseState();

            if (_mouseInWindow)
            {
                _mouseInWindow = false;
                RaiseHwndMouseLeave(new HwndMouseEventArgs(_mouseState));
            }

            ReleaseMouseCapture();
        }

        private void ResetMouseState()
        {
            // We need to invoke events for any buttons that were pressed
            bool fireL = _mouseState.LeftButton == MouseButtonState.Pressed;
            bool fireM = _mouseState.MiddleButton == MouseButtonState.Pressed;
            bool fireR = _mouseState.RightButton == MouseButtonState.Pressed;
            bool fireX1 = _mouseState.X1Button == MouseButtonState.Pressed;
            bool fireX2 = _mouseState.X2Button == MouseButtonState.Pressed;

            // Update the state of all of the buttons
            _mouseState.LeftButton = MouseButtonState.Released;
            _mouseState.MiddleButton = MouseButtonState.Released;
            _mouseState.RightButton = MouseButtonState.Released;
            _mouseState.X1Button = MouseButtonState.Released;
            _mouseState.X2Button = MouseButtonState.Released;

            // Fire any events
            var args = new HwndMouseEventArgs(_mouseState);
            if (fireL && HwndLButtonUp != null)
                HwndLButtonUp(this, args);
            if (fireM && HwndMButtonUp != null)
                HwndMButtonUp(this, args);
            if (fireR && HwndRButtonUp != null)
                HwndRButtonUp(this, args);
            if (fireX1 && HwndX1ButtonUp != null)
                HwndX1ButtonUp(this, args);
            if (fireX2 && HwndX2ButtonUp != null)
                HwndX2ButtonUp(this, args);

            // The mouse is no longer considered to be in our window
            _mouseInWindow = false;
        }

        #endregion

        #region HWND Management

        protected override HandleRef BuildWindowCore(HandleRef hwndParent)
        {
            // Create the host window as a child of the parent
            _hWnd = CreateHostWindow(hwndParent.Handle);
            return new HandleRef(this, _hWnd);
        }

        protected override void DestroyWindowCore(HandleRef hwnd)
        {
            // Destroy the window and reset our hWnd value
            NativeMethods.DestroyWindow(hwnd.Handle);
            _hWnd = IntPtr.Zero;
        }

        /// <summary>
        /// Creates the host window as a child of the parent window.
        /// </summary>
        private IntPtr CreateHostWindow(IntPtr hWndParent)
        {
            // Register our window class
            RegisterWindowClass();

            // Create the window
            return NativeMethods.CreateWindowEx(0, WindowClass, "",
               NativeMethods.WS_CHILD | NativeMethods.WS_VISIBLE,
               0, 0, (int) Width, (int) Height, hWndParent, IntPtr.Zero, IntPtr.Zero, 0);
        }

        /// <summary>
        /// Registers the window class.
        /// </summary>
        private void RegisterWindowClass()
        {
            var wndClass = new NativeMethods.WNDCLASSEX();
            wndClass.cbSize = (uint) Marshal.SizeOf(wndClass);
            wndClass.hInstance = NativeMethods.GetModuleHandle(null);
            wndClass.lpfnWndProc = NativeMethods.DefaultWindowProc;
            wndClass.lpszClassName = WindowClass;
            wndClass.hCursor = NativeMethods.LoadCursor(IntPtr.Zero, NativeMethods.IDC_ARROW);

            NativeMethods.RegisterClassEx(ref wndClass);
        }

        #endregion

        #region WndProc Implementation

        protected override IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            switch (msg)
            {
                case NativeMethods.WM_LBUTTONDOWN:
                    _mouseState.LeftButton = MouseButtonState.Pressed;
                    RaiseHwndLButtonDown(new HwndMouseEventArgs(_mouseState));
                    break;
                case NativeMethods.WM_LBUTTONUP:
                    _mouseState.LeftButton = MouseButtonState.Released;
                    RaiseHwndLButtonUp(new HwndMouseEventArgs(_mouseState));
                    break;
                case NativeMethods.WM_LBUTTONDBLCLK:
                    RaiseHwndLButtonDblClick(new HwndMouseEventArgs(_mouseState, MouseButton.Left));
                    break;
                case NativeMethods.WM_RBUTTONDOWN:
                    _mouseState.RightButton = MouseButtonState.Pressed;
                    RaiseHwndRButtonDown(new HwndMouseEventArgs(_mouseState));
                    break;
                case NativeMethods.WM_RBUTTONUP:
                    _mouseState.RightButton = MouseButtonState.Released;
                    RaiseHwndRButtonDown(new HwndMouseEventArgs(_mouseState));
                    break;
                case NativeMethods.WM_RBUTTONDBLCLK:
                    RaiseHwndRButtonDblClick(new HwndMouseEventArgs(_mouseState, MouseButton.Right));
                    break;
                case NativeMethods.WM_MBUTTONDOWN:
                    _mouseState.MiddleButton = MouseButtonState.Pressed;
                    RaiseHwndMButtonDown(new HwndMouseEventArgs(_mouseState));
                    break;
                case NativeMethods.WM_MBUTTONUP:
                    _mouseState.MiddleButton = MouseButtonState.Released;
                    RaiseHwndMButtonUp(new HwndMouseEventArgs(_mouseState));
                    break;
                case NativeMethods.WM_MBUTTONDBLCLK:
                    RaiseHwndMButtonDblClick(new HwndMouseEventArgs(_mouseState, MouseButton.Middle));
                    break;
                case NativeMethods.WM_XBUTTONDOWN:
                    if (((int) wParam & NativeMethods.MK_XBUTTON1) != 0)
                    {
                        _mouseState.X1Button = MouseButtonState.Pressed;
                        RaiseHwndX1ButtonDown(new HwndMouseEventArgs(_mouseState));
                    }
                    else if (((int) wParam & NativeMethods.MK_XBUTTON2) != 0)
                    {
                        _mouseState.X2Button = MouseButtonState.Pressed;
                        RaiseHwndX2ButtonDown(new HwndMouseEventArgs(_mouseState));
                    }
                    break;
                case NativeMethods.WM_XBUTTONUP:
                    if (((int) wParam & NativeMethods.MK_XBUTTON1) != 0)
                    {
                        _mouseState.X1Button = MouseButtonState.Released;
                        RaiseHwndX1ButtonUp(new HwndMouseEventArgs(_mouseState));
                    }
                    else if (((int) wParam & NativeMethods.MK_XBUTTON2) != 0)
                    {
                        _mouseState.X2Button = MouseButtonState.Released;
                        RaiseHwndX2ButtonUp(new HwndMouseEventArgs(_mouseState));
                    }
                    break;
                case NativeMethods.WM_XBUTTONDBLCLK:
                    if (((int) wParam & NativeMethods.MK_XBUTTON1) != 0)
                        RaiseHwndX1ButtonDblClick(new HwndMouseEventArgs(_mouseState, MouseButton.XButton1));
                    else if (((int) wParam & NativeMethods.MK_XBUTTON2) != 0)
                        RaiseHwndX2ButtonDblClick(new HwndMouseEventArgs(_mouseState, MouseButton.XButton2));
                    break;
                case NativeMethods.WM_MOUSEMOVE:
                    // If the application isn't in focus, we don't handle this message
                    if (!_applicationHasFocus)
                        break;

                    // record the prevous and new position of the mouse
                    _mouseState.PreviousPosition = _mouseState.Position;
                    _mouseState.Position = new Point(
                        NativeMethods.GetXLParam((int) lParam),
                        NativeMethods.GetYLParam((int) lParam));

                    if (!_mouseInWindow)
                    {
                        _mouseInWindow = true;

                        // if the mouse is just entering, use the same position for the previous state
                        // so we don't get weird deltas happening when the move event fires
                        _mouseState.PreviousPosition = _mouseState.Position;

                        RaiseHwndMouseEnter(new HwndMouseEventArgs(_mouseState));

                        // send the track mouse event so that we get the WM_MOUSELEAVE message
                        var tme = new NativeMethods.TRACKMOUSEEVENT
                        {
                            cbSize = Marshal.SizeOf(typeof (NativeMethods.TRACKMOUSEEVENT)),
                            dwFlags = NativeMethods.TME_LEAVE,
                            hWnd = hwnd
                        };
                        NativeMethods.TrackMouseEvent(ref tme);
                    }

                    // Only fire the mouse move if the position actually changed
                    if (_mouseState.Position != _mouseState.PreviousPosition)
                        RaiseHwndMouseMove(new HwndMouseEventArgs(_mouseState));

                    break;
                case NativeMethods.WM_MOUSELEAVE:

                    // If we have capture, we ignore this message because we're just
                    // going to reset the cursor position back into the window
                    if (_isMouseCaptured)
                        break;

                    // Reset the state which releases all buttons and 
                    // marks the mouse as not being in the window.
                    ResetMouseState();

                    RaiseHwndMouseLeave(new HwndMouseEventArgs(_mouseState));

                    break;
            }

            return base.WndProc(hwnd, msg, wParam, lParam, ref handled);
        }

        protected virtual void RaiseHwndLButtonDown(HwndMouseEventArgs args)
        {
            var handler = HwndLButtonDown;
            if (handler != null)
                handler(this, args);
        }

        protected virtual void RaiseHwndLButtonUp(HwndMouseEventArgs args)
        {
            var handler = HwndLButtonUp;
            if (handler != null)
                handler(this, args);
        }

        protected virtual void RaiseHwndRButtonDown(HwndMouseEventArgs args)
        {
            var handler = HwndRButtonDown;
            if (handler != null)
                handler(this, args);
        }

        protected virtual void RaiseHwndRButtonUp(HwndMouseEventArgs args)
        {
            var handler = HwndRButtonUp;
            if (handler != null)
                handler(this, args);
        }

        protected virtual void RaiseHwndMButtonDown(HwndMouseEventArgs args)
        {
            var handler = HwndMButtonDown;
            if (handler != null)
                handler(this, args);
        }

        protected virtual void RaiseHwndMButtonUp(HwndMouseEventArgs args)
        {
            var handler = HwndMButtonUp;
            if (handler != null)
                handler(this, args);
        }

        protected virtual void RaiseHwndLButtonDblClick(HwndMouseEventArgs args)
        {
            var handler = HwndLButtonDblClick;
            if (handler != null)
                handler(this, args);
        }

        protected virtual void RaiseHwndRButtonDblClick(HwndMouseEventArgs args)
        {
            var handler = HwndRButtonDblClick;
            if (handler != null)
                handler(this, args);
        }

        protected virtual void RaiseHwndMButtonDblClick(HwndMouseEventArgs args)
        {
            var handler = HwndMButtonDblClick;
            if (handler != null)
                handler(this, args);
        }

        protected virtual void RaiseHwndMouseEnter(HwndMouseEventArgs args)
        {
            var handler = HwndMouseEnter;
            if (handler != null)
                handler(this, args);
        }

        protected virtual void RaiseHwndX1ButtonDown(HwndMouseEventArgs args)
        {
            var handler = HwndX1ButtonDown;
            if (handler != null)
                handler(this, args);
        }

        protected virtual void RaiseHwndX1ButtonUp(HwndMouseEventArgs args)
        {
            var handler = HwndX1ButtonUp;
            if (handler != null)
                handler(this, args);
        }

        protected virtual void RaiseHwndX2ButtonDown(HwndMouseEventArgs args)
        {
            var handler = HwndX2ButtonDown;
            if (handler != null)
                handler(this, args);
        }

        protected virtual void RaiseHwndX2ButtonUp(HwndMouseEventArgs args)
        {
            var handler = HwndX2ButtonUp;
            if (handler != null)
                handler(this, args);
        }

        protected virtual void RaiseHwndX1ButtonDblClick(HwndMouseEventArgs args)
        {
            var handler = HwndX1ButtonDblClick;
            if (handler != null)
                handler(this, args);
        }

        protected virtual void RaiseHwndX2ButtonDblClick(HwndMouseEventArgs args)
        {
            var handler = HwndX2ButtonDblClick;
            if (handler != null)
                handler(this, args);
        }

        protected virtual void RaiseHwndMouseLeave(HwndMouseEventArgs args)
        {
            var handler = HwndMouseLeave;
            if (handler != null)
                handler(this, args);
        }

        protected virtual void RaiseHwndMouseMove(HwndMouseEventArgs args)
        {
            var handler = HwndMouseMove;
            if (handler != null)
                handler(this, args);
        }

        #endregion
    }
}