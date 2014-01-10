// Copyright (c) 2010-2013 SharpDX - Alexandre Mutel
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
// -----------------------------------------------------------------------------
// Original code from SlimMath project. http://code.google.com/p/slimmath/
// Greetings to SlimDX Group. Original code published with the following license:
// -----------------------------------------------------------------------------
/*
* Copyright (c) 2007-2011 SlimDX Group
* 
* Permission is hereby granted, free of charge, to any person obtaining a copy
* of this software and associated documentation files (the "Software"), to deal
* in the Software without restriction, including without limitation the rights
* to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
* copies of the Software, and to permit persons to whom the Software is
* furnished to do so, subject to the following conditions:
* 
* The above copyright notice and this permission notice shall be included in
* all copies or substantial portions of the Software.
* 
* THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
* IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
* FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
* AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
* LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
* OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
* THE SOFTWARE.
*/

using System;
using System.Windows;
using System.Windows.Interop;
using Gemini.Modules.SharpDX.Services;
using Gemini.Modules.SharpDX.Util;
using SharpDX.Direct3D9;

namespace Gemini.Modules.SharpDX.Controls
{
	public class D3D11ImageSource : D3DImage, IDisposable
	{
		private Texture _renderTarget;

		public D3D11ImageSource(Window parentWindow)
        {
			DeviceService.StartD3D(parentWindow);
        }

		public void Dispose()
		{
			SetRenderTargetDX10(null);
			Disposer.RemoveAndDispose(ref _renderTarget);
			DeviceService.EndD3D();
		}

		internal void InvalidateD3DImage()
		{
			if (_renderTarget == null)
				return;

			Lock();
			AddDirtyRect(new Int32Rect(0, 0, PixelWidth, PixelHeight));
			Unlock();
		}

		internal void SetRenderTargetDX10(global::SharpDX.Direct3D11.Texture2D renderTarget)
		{
			if (_renderTarget != null)
			{
				_renderTarget = null;

				Lock();
				SetBackBuffer(D3DResourceType.IDirect3DSurface9, IntPtr.Zero);
				Unlock();
			}

			if (renderTarget == null)
				return;

			if (!renderTarget.IsShareable())
				throw new ArgumentException("Texture must be created with ResourceOptionFlags.Shared");

			var format = renderTarget.GetTranslatedFormat();
			if (format == Format.Unknown)
				throw new ArgumentException("Texture format is not compatible with OpenSharedResource");

			var handle = renderTarget.GetSharedHandle();
			if (handle == IntPtr.Zero)
				throw new ArgumentException("Handle could not be retrieved");

			_renderTarget = new Texture(DeviceService.D3DDevice, 
				renderTarget.Description.Width, 
				renderTarget.Description.Height, 
				1, Usage.RenderTarget, format, 
				Pool.Default, ref handle);

			using (Surface surface = _renderTarget.GetSurfaceLevel(0))
			{
				Lock();
				SetBackBuffer(D3DResourceType.IDirect3DSurface9, surface.NativePointer);
				Unlock();
			}
		}
	}
}