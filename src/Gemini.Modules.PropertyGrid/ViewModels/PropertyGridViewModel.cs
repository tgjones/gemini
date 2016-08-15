using System;
using System.ComponentModel.Composition;
using Gemini.Framework;
using Gemini.Framework.Commands;
using Gemini.Framework.Services;
using Gemini.Modules.UndoRedo.Commands;

namespace Gemini.Modules.PropertyGrid.ViewModels
{
	[Export(typeof(IPropertyGrid))]
	public class PropertyGridViewModel : Tool, IPropertyGrid, ICommandRerouter
	{
	    private readonly IShell _shell;

		public override PaneLocation PreferredLocation
		{
			get { return PaneLocation.Right; }
		}

		public override Uri IconSource
		{
			get { return new Uri("pack://application:,,,/Gemini.Modules.PropertyGrid;component/Resources/Icons/Properties.png"); }
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

        [ImportingConstructor]
        public PropertyGridViewModel(IShell shell)
        {
            _shell = shell;
            DisplayName = Properties.Resources.PropertyGridViewModel_PropertyGridViewModel_Properties;
        }

	    object ICommandRerouter.GetHandler(CommandDefinitionBase commandDefinition)
	    {
	        if (commandDefinition is UndoCommandDefinition ||
	            commandDefinition is RedoCommandDefinition)
	        {
	            return _shell.ActiveItem;
	        }

	        return null;
	    }
	}
}