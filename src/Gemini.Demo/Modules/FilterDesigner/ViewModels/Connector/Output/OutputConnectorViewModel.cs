using System;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Caliburn.Micro;
using System.Threading.Tasks.Dataflow;

namespace Gemini.Demo.Modules.FilterDesigner.ViewModels
{
    public abstract class OutputConnectorViewModel : ConnectorViewModel
    {

        public override ConnectorDirection ConnectorDirection
        {
            get { return ConnectorDirection.Output; }
        }

        private readonly BindableCollection<ConnectionViewModel> _connections;
        public IObservableCollection<ConnectionViewModel> Connections
        {
            get { return _connections; }
        }

        public OutputConnectorViewModel(ElementViewModel element, string name, Color color)
            : base(element, name, color)
        {
            _connections = new BindableCollection<ConnectionViewModel>();
        }

        internal ConnectionViewModel Connect(InputConnectorViewModel inputConnectorViewModel)
        {
            var x = GetNewConnection();
                x.To = inputConnectorViewModel;
            return x;
        }
    }
}