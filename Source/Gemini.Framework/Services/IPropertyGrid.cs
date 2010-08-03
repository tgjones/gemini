using Caliburn.PresentationFramework.ApplicationModel;

namespace Gemini.Framework.Services
{
	public interface IPropertyGrid : IExtendedPresenter
	{
		object SelectedObject { get; set; }
	}
}