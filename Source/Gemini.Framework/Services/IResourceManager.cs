using System.IO;

namespace Gemini.Framework.Services
{
	public interface IResourceManager
	{
		Stream GetStream(string relativeUri, string assemblyName);
	}
}