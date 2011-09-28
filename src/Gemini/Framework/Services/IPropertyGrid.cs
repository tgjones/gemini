using Caliburn.Micro;

namespace Gemini.Framework.Services
{
	public interface IPropertyGrid : IScreen
	{
		object SelectedObject { get; set; }
	}
}