using System.ComponentModel.Composition;
using Gemini.Framework.Menus;
using Gemini.Framework.Services;

namespace Gemini.Framework
{
	public abstract class ModuleBase : IModule
	{
		[Import]
		private IShell _shell;

		protected IShell Shell
		{
			get { return _shell; }
		}

		protected IMenu MainMenu
		{
			get { return _shell.MainMenu; }
		}

		public abstract void Initialize();
	}
}