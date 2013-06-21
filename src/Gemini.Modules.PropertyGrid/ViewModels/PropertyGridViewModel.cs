using System;
using System.ComponentModel.Composition;
using Gemini.Framework;
using Gemini.Framework.Services;

namespace Gemini.Modules.PropertyGrid.ViewModels
{
	[Export(typeof(IPropertyGrid))]
	public class PropertyGridViewModel : Tool, IPropertyGrid
	{
		public override PaneLocation PreferredLocation
		{
			get { return PaneLocation.Right; }
		}

		public override Uri IconSource
		{
			get { return new Uri("pack://application:,,,/Gemini;component/Resources/Icons/Properties.png"); }
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

        public PropertyGridViewModel()
        {
            DisplayName = "Properties";
        }
	}
}