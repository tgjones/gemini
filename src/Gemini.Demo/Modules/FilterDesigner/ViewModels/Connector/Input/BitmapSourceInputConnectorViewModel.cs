using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Gemini.Demo.Modules.FilterDesigner.ViewModels.Connector.Input
{
    class BitmapSourceInputConnectorViewModel : GenericInputConnectorViewModel<BitmapSource>
    {
        public BitmapSourceInputConnectorViewModel(ITargetBlock<BitmapSource> target, ElementViewModel element, string name, Color color) : base(target, element, name, color)
        {
        }
    }
}
