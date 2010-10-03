using Caliburn.Core;

namespace Gemini.Framework
{
	public interface IModule
	{
		void Configure(IContainer container);
		void Start();
	}
}