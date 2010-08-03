using Gemini.Framework;
using Gemini.Framework.Services;
using Gemini.Modules.Output;

namespace Gemini.Demo.Modules.Startup
{
	public class Module : ModuleBase
	{
		protected override void Initialize()
		{
			Container.GetInstance<IShell>().Title = "Gemini Demo";
			Container.GetInstance<IOutput>().Append("Started up");
		}
	}
}