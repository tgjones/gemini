using System.Collections.Generic;

namespace Gemini.Contracts.Services.ExtensionService
{
	public interface IExtensionService
	{
		IList<T> Sort<T>(IEnumerable<T> extensionCollection) where T : IExtension;
	}
}