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
    abstract class GenericOutputConnectorViewModel<T> : OutputConnectorViewModel
    {
        public ISourceBlock<T> output;

        public GenericOutputConnectorViewModel(ElementViewModel element, string name, Color color, ISourceBlock<T> valueCallback) : base(element, name, color)
        {
            output = valueCallback;
            Type = typeof(T);
        }

        public override Type Type { get; set; }
    }
}
