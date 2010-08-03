using System.Collections.Generic;
using Caliburn.Core;
using Gemini.Framework;
using Gemini.Framework.Questions;
using Gemini.Framework.Ribbon;
using Gemini.Framework.Services;
using Gemini.Modules.Shell.ViewModels;

namespace Gemini.Modules.Shell
{
	public class Module : ModuleBase
	{
		protected override IEnumerable<ComponentInfo> GetComponents()
		{
			yield return Singleton<IShell, ShellViewModel>();
			yield return Singleton<IRibbon, RibbonViewModel>();
			yield return Singleton<IResourceManager, ResourceManager>();
			yield return PerRequest<IQuestionDialog, QuestionDialogViewModel>();
			yield return Singleton<IInputManager, DefaultInputManager>();
		}
	}
}