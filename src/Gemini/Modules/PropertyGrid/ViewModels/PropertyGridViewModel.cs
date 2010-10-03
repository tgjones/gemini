using Caliburn.Core;
using Gemini.Framework;
using Gemini.Framework.Services;

namespace Gemini.Modules.PropertyGrid.ViewModels
{
	public class PropertyGridViewModel : Screen, IPropertyGrid
	{
		private object _selectedObject;

		public object SelectedObject
		{
			get { return _selectedObject; }
			set
			{
				_selectedObject = value;
				NotifyOfPropertyChange("SelectedObject");
			}
		}
	}
}