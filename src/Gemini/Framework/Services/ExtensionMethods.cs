using System.Reflection;

namespace Gemini.Framework.Services
{
	public static class ExtensionMethods
	{
		public static string GetExecutingAssemblyName()
		{
			return Assembly.GetExecutingAssembly().GetAssemblyName();
		}

		public static string GetAssemblyName(this Assembly assembly)
		{
			string name = assembly.FullName;
			return name.Substring(0, name.IndexOf(','));
		}
	}
}