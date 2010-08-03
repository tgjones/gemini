using System.Collections.Generic;
using Caliburn.Core;
using Gemini.Framework;
using Gemini.Framework.Services;

namespace Gemini.Modules.StatusBar
{
	public class Module : ModuleBase
	{
		protected override IEnumerable<ComponentInfo> GetComponents()
		{
			yield return Singleton<IStatusBar, StatusBarViewModel>();
		}
	}
}