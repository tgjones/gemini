#region File Description
//-----------------------------------------------------------------------------
// Copyright 2011, Nick Gravelyn.
// Licensed under the terms of the Ms-PL:
// http://www.microsoft.com/opensource/licenses.mspx#Ms-PL
//-----------------------------------------------------------------------------
#endregion

using System;
using System.Reflection;
using System.Runtime.InteropServices;
using Microsoft.Xna.Framework.Graphics;

namespace Gemini.Modules.Xna.Util
{
    /// <summary>
    /// An internal set of functionality used for interopping with native Win32 APIs.
    /// </summary>
    internal static class NativeMethods
    {
        #region Constants

        // Define the window styles we use
        public const int WS_CHILD = 0x40000000;
        public const int WS_VISIBLE = 0x10000000;

        // Define the Windows messages we will handle
        public const int WM_MOUSEMOVE = 0x0200;
        public const int WM_LBUTTONDOWN = 0x0201;
        public const int WM_LBUTTONUP = 0x0202;
        public const int WM_LBUTTONDBLCLK = 0x0203;
        public const int WM_RBUTTONDOWN = 0x0204;
        public const int WM_RBUTTONUP = 0x0205;
        public const int WM_RBUTTONDBLCLK = 0x0206;
        public const int WM_MBUTTONDOWN = 0x0207;
        public const int WM_MBUTTONUP = 0x0208;
        public const int WM_MBUTTONDBLCLK = 0x0209;
        public const int WM_MOUSEWHEEL = 0x020A;
        public const int WM_XBUTTONDOWN = 0x020B;
        public const int WM_XBUTTONUP = 0x020C;
        public const int WM_XBUTTONDBLCLK = 0x020D;
        public const int WM_MOUSELEAVE = 0x02A3;

        // Define the values that let us differentiate between the two extra mouse buttons
        public const int MK_XBUTTON1 = 0x020;
        public const int MK_XBUTTON2 = 0x040;

        // Define the cursor icons we use
        public const int IDC_ARROW = 32512;

        // Define the TME_LEAVE value so we can register for WM_MOUSELEAVE messages
        public const uint TME_LEAVE = 0x00000002;

        #endregion

        #region Delegates and Structs

        public delegate IntPtr WndProc(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam);

        public static readonly WndProc DefaultWindowProc = DefWindowProc;

        [StructLayout(LayoutKind.Sequential)]
        public struct TRACKMOUSEEVENT
        {
            public int cbSize;
            public uint dwFlags;
            public IntPtr hWnd;
            public uint dwHoverTime;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct WNDCLASSEX
        {
            public uint cbSize;
            public uint style;
            [MarshalAs(UnmanagedType.FunctionPtr)]
            public WndProc lpfnWndProc;
            public int cbClsExtra;
            public int cbWndExtra;
            public IntPtr hInstance;
            public IntPtr hIcon;
            public IntPtr hCursor;
            public IntPtr hbrBackground;
            public string lpszMenuName;
            public string lpszClassName;
            public IntPtr hIconSm;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct NativePoint
        {
            public int X;
            public int Y;
        }

        #endregion

        #region DllImports

        [DllImport("user32.dll", EntryPoint = "CreateWindowEx", CharSet = CharSet.Auto)]
        public static extern IntPtr CreateWindowEx(
            int exStyle,
            string className,
            string windowName,
            int style,
            int x, int y,
            int width, int height,
            IntPtr hwndParent,
            IntPtr hMenu,
            IntPtr hInstance,
            [MarshalAs(UnmanagedType.AsAny)] object pvParam);

        [DllImport("user32.dll", EntryPoint = "DestroyWindow", CharSet = CharSet.Auto)]
        public static extern bool DestroyWindow(IntPtr hwnd);

        [DllImport("user32.dll")]
        public static extern IntPtr DefWindowProc(IntPtr hWnd, uint uMsg, IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll")]
        public static extern IntPtr GetModuleHandle(string module);

        [DllImport("user32.dll")]
        public static extern IntPtr LoadCursor(IntPtr hInstance, int lpCursorName);

        [DllImport("user32.dll")]
        public static extern int TrackMouseEvent(ref TRACKMOUSEEVENT lpEventTrack);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.U2)]
        public static extern short RegisterClassEx([In] ref WNDCLASSEX lpwcx);

        [DllImport("user32.dll")]
        public static extern int ScreenToClient(IntPtr hWnd, ref NativePoint pt);

        [DllImport("user32.dll")]
        public static extern int SetFocus(IntPtr hWnd);

        [DllImport("user32.dll")]
        public static extern IntPtr GetFocus();

        [DllImport("user32.dll")]
        public static extern IntPtr SetCapture(IntPtr hWnd);

        [DllImport("user32.dll")]
        public static extern bool ReleaseCapture();

        [DllImport("user32.dll")]
        public static extern bool GetCursorPos(ref NativePoint point);

        [DllImport("user32.dll")]
        public static extern bool SetCursorPos(int x, int y);

        [DllImport("user32.dll")]
        public static extern int ShowCursor(bool bShow);

        #endregion

        #region Helpers
        
        public static int GetXLParam(int lParam) 
        {
            return LowWord(lParam);
        }

        public static int GetYLParam(int lParam) 
        {
            return HighWord(lParam);
        }

        public static int GetWheelDeltaWParam(int wParam)
        {
            return HighWord(wParam);
        }

        public static int LowWord(int input)
        {
            return (short) (input & 0xffff);
        }

        public static int HighWord(int input)
        {
            return (short) (input >> 16);
        }

        #endregion

        #region Interfaces

        [ComImport, Guid("D0223B96-BF7A-43fd-92BD-A43B0D82B9EB"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public interface IDirect3DDevice9
        {
            void TestCooperativeLevel();
            void GetAvailableTextureMem();
            void EvictManagedResources();
            void GetDirect3D();
            void GetDeviceCaps();
            void GetDisplayMode();
            void GetCreationParameters();
            void SetCursorProperties();
            void SetCursorPosition();
            void ShowCursor();
            void CreateAdditionalSwapChain();
            void GetSwapChain();
            void GetNumberOfSwapChains();
            void Reset();
            void Present();
            int GetBackBuffer(uint swapChain, uint backBuffer, int type, out IntPtr backBufferPointer);
        }

        [ComImport, Guid("85C31227-3DE5-4f00-9B3A-F11AC38C18B5"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        private interface IDirect3DTexture9
        {
            void GetDevice();
            void SetPrivateData();
            void GetPrivateData();
            void FreePrivateData();
            void SetPriority();
            void GetPriority();
            void PreLoad();
            void GetType();
            void SetLOD();
            void GetLOD();
            void GetLevelCount();
            void SetAutoGenFilterType();
            void GetAutoGenFilterType();
            void GenerateMipSubLevels();
            void GetLevelDesc();
            int GetSurfaceLevel(uint level, out IntPtr surfacePointer);
        }

        #endregion

        #region Wrapper methods

        public static unsafe IntPtr GetDirect3DDevice(GraphicsDevice graphics)
        {
            FieldInfo comPtr = graphics.GetType().GetField("pComPtr", BindingFlags.NonPublic | BindingFlags.Instance);
            return new IntPtr(Pointer.Unbox(comPtr.GetValue(graphics)));
        }

        public static IntPtr GetBackBuffer(GraphicsDevice graphicsDevice)
        {
            IntPtr surfacePointer;
            var device = GetIUnknownObject<IDirect3DDevice9>(graphicsDevice);
            Marshal.ThrowExceptionForHR(device.GetBackBuffer(0, 0, 0, out surfacePointer));
            Marshal.ReleaseComObject(device);
            return surfacePointer;
        }

        public static IntPtr GetRenderTargetSurface(RenderTarget2D renderTarget)
        {
            IntPtr surfacePointer;
            var texture = GetIUnknownObject<IDirect3DTexture9>(renderTarget);
            Marshal.ThrowExceptionForHR(texture.GetSurfaceLevel(0, out surfacePointer));
            Marshal.ReleaseComObject(texture);
            return surfacePointer;
        }

        public static T GetIUnknownObject<T>(object container)
        {
            unsafe
            {
                //Get the COM object pointer from the D3D object and marshal it as one of the interfaces defined below
                var deviceField = container.GetType().GetField("pComPtr", BindingFlags.NonPublic | BindingFlags.Instance);
                var devicePointer = new IntPtr(Pointer.Unbox(deviceField.GetValue(container)));
                return (T) Marshal.GetObjectForIUnknown(devicePointer);
            }
        }

        #endregion
    }
}
