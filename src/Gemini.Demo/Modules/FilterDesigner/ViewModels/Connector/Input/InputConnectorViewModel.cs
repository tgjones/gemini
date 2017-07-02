using Caliburn.Micro;
using System;
using System.Threading.Tasks.Dataflow;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Gemini.Demo.Modules.FilterDesigner.ViewModels
{
    public abstract class InputConnectorViewModel : ConnectorViewModel
    {

        public override ConnectorDirection ConnectorDirection
        {
            get { return ConnectorDirection.Input; }
        }

        private readonly BindableCollection<ConnectionViewModel> _connections = new BindableCollection<ConnectionViewModel>();
        public IObservableCollection<ConnectionViewModel> Connections
        {
            get { return _connections; }
        }
 
        public InputConnectorViewModel(ElementViewModel element, string name, Color color)
            : base(element, name, color)
        {
        }
    }
}