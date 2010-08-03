using Caliburn.PresentationFramework.ApplicationModel;

namespace Gemini.Modules.Output
{
	public interface IOutput : IExtendedPresenter
	{
		void Clear();
		void Append(string text);
	}
}