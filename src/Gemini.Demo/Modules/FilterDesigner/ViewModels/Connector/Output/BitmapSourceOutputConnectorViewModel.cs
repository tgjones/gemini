using Gemini.Demo.Modules.FilterDesigner.ViewModels.Connection;
using Gridsum.DataflowEx;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Gemini.Demo.Modules.FilterDesigner.ViewModels.Connector.Output
{
    class BitmapSourceOutputConnectorViewModel : GenericOutputConnectorViewModel<BitmapSource>
    {
        public BitmapSourceOutputConnectorViewModel(ElementViewModel element, string name, Color color, ISourceBlock<BitmapSource> valueCallback) : base(element, name, color, valueCallback)
        {
        }

        public override ConnectionViewModel GetNewConnection()
        {
            return new BitmapSourceConnection(this);
        }
    }
}
