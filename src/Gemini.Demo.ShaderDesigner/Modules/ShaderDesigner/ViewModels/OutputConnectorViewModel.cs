using System.Windows.Media;

namespace Gemini.Demo.ShaderDesigner.Modules.ShaderDesigner.ViewModels
{
    public class OutputConnectorViewModel : ConnectorViewModel
    {
        public override ConnectorDirection ConnectorDirection
        {
            get { return ConnectorDirection.Output; }
        }

        public OutputConnectorViewModel(ElementViewModel element, string name, Color color)
            : base(element, name, color)
        {
        }
    }
}