using Gemini.Demo.Modules.FilterDesigner.ViewModels.Connector.Input;
using Gemini.Demo.Modules.FilterDesigner.ViewModels.Connector.Output;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Gemini.Demo.Modules.FilterDesigner.ViewModels.Connection
{
    class BitmapSourceConnection : GenericConnectionViewModel<BitmapSource>
    {
        public BitmapSourceConnection(GenericOutputConnectorViewModel<BitmapSource> from, GenericInputConnectorViewModel<BitmapSource> to) :base(from,to)
        {
        }

        public BitmapSourceConnection(GenericOutputConnectorViewModel<BitmapSource> from) : base(from)
        {
        }

        Brush __color = new SolidColorBrush(System.Windows.Media.Color.FromRgb(36, 97, 121));
        internal override Brush _color { get  { return __color; } set { __color = value; } }
    }
}
