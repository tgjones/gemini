using System.Windows;
using Gemini.Contracts.Services.ExtensionService;

namespace Gemini.Contracts.Gui.Layout
{
	public interface ILayoutItem : IExtension
	{
		string Name { get; }
		string Title { get; }

		void OnGotFocus(object sender, RoutedEventArgs e);
		void OnLostFocus(object sender, RoutedEventArgs e);
	}
}