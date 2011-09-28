using System.ComponentModel.Composition;
using System.Reflection;
using Gemini.Framework;
using Gemini.Framework.Services;
using Gemini.Modules.Output;

namespace Gemini.Demo.Modules.Startup
{
	[Export(typeof(IModule))]
	public class Module : IModule
	{
		[Import]
		private IShell _shell;

		[Import]
		private IOutput _output;

		[Import]
		private IPropertyGrid _propertyGrid;

		[Import]
		private IResourceManager _resourceManager;

		public void Initialize()
		{
			_shell.Title = "Gemini Demo";
			_shell.StatusBar.Message = "Hello world!";
			_shell.Icon = _resourceManager.GetBitmap("Resources/Icon.png", 
				Assembly.GetExecutingAssembly().GetAssemblyName());

			_output.Append("Started up");

			_propertyGrid.SelectedObject = _shell;
		}
	}
}