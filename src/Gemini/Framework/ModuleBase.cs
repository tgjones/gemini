using System.ComponentModel.Composition;
using Gemini.Framework.Menus;
using Gemini.Framework.Services;
using Gemini.Framework.ToolBars;

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

        protected IToolBar ToolBar
        {
            get { return _shell.ToolBar; }
        }

        public virtual void PreInitialize()
        {
            
        }

		public virtual void Initialize()
		{
		    
		}

        public virtual void PostInitialize()
        {

        }
	}
}