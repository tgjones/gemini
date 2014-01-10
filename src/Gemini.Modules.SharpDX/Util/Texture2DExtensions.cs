using System;
using SharpDX.Direct3D9;

namespace Gemini.Modules.SharpDX.Util
{
	internal static class Texture2DExtensions
	{
		public static bool IsShareable(this global::SharpDX.Direct3D11.Texture2D texture)
		{
			return texture.Description.OptionFlags.HasFlag(global::SharpDX.Direct3D11.ResourceOptionFlags.Shared);
		}

		public static Format GetTranslatedFormat(this global::SharpDX.Direct3D11.Texture2D texture)
		{
			switch (texture.Description.Format)
			{
				case global::SharpDX.DXGI.Format.R10G10B10A2_UNorm:
					return Format.A2B10G10R10;
				case global::SharpDX.DXGI.Format.R16G16B16A16_Float:
					return Format.A16B16G16R16F;
				case global::SharpDX.DXGI.Format.B8G8R8A8_UNorm:
					return Format.A8R8G8B8;
				default:
					return Format.Unknown;
			}
		}

		public static IntPtr GetSharedHandle(this global::SharpDX.Direct3D11.Texture2D texture)
		{
			var resource = texture.QueryInterface<global::SharpDX.DXGI.Resource>();
			var result = resource.SharedHandle;
			resource.Dispose();
			return result;
		}
	}
}