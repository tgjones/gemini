using System.Windows.Media;
using Caliburn.Micro;
using Gemini.Framework;

namespace Gemini.Modules.GraphEditor.ViewModels
{
    public class GraphViewModel : Document
    {
        public override string DisplayName
        {
            get { return "[New Graph]"; }
        }

        private readonly BindableCollection<ElementViewModel> _elements;
        public IObservableCollection<ElementViewModel> Elements
        {
            get { return _elements; }
        }

        private readonly BindableCollection<ConnectionViewModel> _connections;
        public IObservableCollection<ConnectionViewModel> Connections
        {
            get { return _connections; }
        }

        public GraphViewModel()
        {
            _elements = new BindableCollection<ElementViewModel>();
            _connections = new BindableCollection<ConnectionViewModel>();

            var element1 = AddElement(100, 100, "Add");
            var element2 = AddElement(300, 150, "Multiply");

            Connections.Add(new ConnectionViewModel(
                element1.OutputConnectors[0], 
                element2.InputConnectors[0]));
        }

        public ElementViewModel AddElement(double x, double y, string name)
        {
            var element = new ElementViewModel { X = x, Y = y, Name = name };
            element.InputConnectors.Add(new ConnectorViewModel(element) { Color = Colors.Red });
            element.InputConnectors.Add(new ConnectorViewModel(element) { Color = Colors.Blue });
            element.OutputConnectors.Add(new ConnectorViewModel(element) { Color = Colors.Green });
            _elements.Add(element);
            return element;
        }
    }
}