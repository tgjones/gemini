using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using Gemini.Framework.Services;
using Gemini.Modules.MainMenu;
using Gemini.Modules.ToolBars;

namespace Gemini.Framework
{
	public abstract class ModuleBase : IModule
	{
        [Import]
        private IMainWindow _mainWindow;

	    protected IMainWindow MainWindow
	    {
	        get { return _mainWindow; }
	    }

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

        protected IToolBars ToolBars
        {
            get { return _shell.ToolBars; }
        }

        public virtual IEnumerable<IDocument> DefaultDocuments
        {
            get { yield break; }
        }

	    public virtual IEnumerable<Type> DefaultTools
	    {
            get { yield break; }
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