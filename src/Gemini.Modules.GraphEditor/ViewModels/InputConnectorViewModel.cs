using System.Windows.Media;

namespace Gemini.Modules.GraphEditor.ViewModels
{
    public class InputConnectorViewModel : ConnectorViewModel
    {
        public override ConnectorDirection ConnectorDirection
        {
            get { return ConnectorDirection.Input; }
        }

        public InputConnectorViewModel(ElementViewModel element, string name, Color color)
            : base(element, name, color)
        {
            
        }
    }
}