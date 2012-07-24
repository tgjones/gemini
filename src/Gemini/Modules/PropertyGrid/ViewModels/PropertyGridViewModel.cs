using System.ComponentModel.Composition;
using Gemini.Framework;
using Gemini.Framework.Services;

namespace Gemini.Modules.PropertyGrid.ViewModels
{
	[Export(typeof(IPropertyGrid))]
	public class PropertyGridViewModel : Tool, IPropertyGrid
	{
		public override string DisplayName
		{
			get { return "Properties"; }
		}

		private object _selectedObject;
		public object SelectedObject
		{
			get { return _selectedObject; }
			set
			{
				_selectedObject = value;
				NotifyOfPropertyChange(() => SelectedObject);
			}
		}
	}
}