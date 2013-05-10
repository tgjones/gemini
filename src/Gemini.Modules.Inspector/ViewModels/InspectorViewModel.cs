using System.ComponentModel.Composition;
using Gemini.Framework;
using Gemini.Framework.Services;

namespace Gemini.Modules.Inspector.ViewModels
{
    [Export(typeof(IInspectorTool))]
    public class InspectorViewModel : Tool, IInspectorTool
    {
        public override string DisplayName
        {
            get { return "Inspector"; }
        }

        public override PaneLocation PreferredLocation
        {
            get { return PaneLocation.Right; }
        }

        public override double PreferredWidth
        {
            get { return 300; }
        }

        private IInspectableObject _selectedObject;
        public IInspectableObject SelectedObject
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