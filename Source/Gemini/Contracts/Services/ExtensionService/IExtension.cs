using Gemini.Contracts.Gui.ViewModel;

namespace Gemini.Contracts.Services.ExtensionService
{
	public enum RelativeDirection
	{
		Before = 1,
		After
	}

	public interface IExtension : IViewModel
	{
		string ID { get; }
		string InsertRelativeToID { get; }
		RelativeDirection BeforeOrAfter { get; }
	}
}