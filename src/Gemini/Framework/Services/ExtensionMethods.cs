using System.Reflection;
using Caliburn.Micro;

namespace Gemini.Framework.Services
{
	public static class ExtensionMethods
	{
		public static string GetExecutingAssemblyName()
		{
			return Assembly.GetExecutingAssembly().GetAssemblyName();
		}
	}
}