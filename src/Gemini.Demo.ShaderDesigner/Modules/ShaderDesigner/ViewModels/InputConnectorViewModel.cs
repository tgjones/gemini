using System;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Gemini.Demo.ShaderDesigner.Modules.ShaderDesigner.ViewModels
{
    public class InputConnectorViewModel : ConnectorViewModel
    {
        public event EventHandler ConnectionChanged;

        public override ConnectorDirection ConnectorDirection
        {
            get { return ConnectorDirection.Input; }
        }

        private ConnectionViewModel _connection;
        public ConnectionViewModel Connection
        {
            get { return _connection; }
            set
            {
                _connection = value;
                if (ConnectionChanged != null)
                    ConnectionChanged(this, EventArgs.Empty);
                NotifyOfPropertyChange(() => Connection);
            }
        }

        public BitmapSource Value
        {
            get
            {
                if (Connection == null || Connection.From == null)
                    return null;

                return Connection.From.Value;
            }
        }

        public InputConnectorViewModel(ElementViewModel element, string name, Color color)
            : base(element, name, color)
        {
            
        }
    }
}