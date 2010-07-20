using Gemini.Contracts.Services.ExtensionService;

namespace Gemini.Contracts.Application.Startup
{
	public interface IExecutableCommand : IExtension
	{
		void Run(params object[] args);
	}
}