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
