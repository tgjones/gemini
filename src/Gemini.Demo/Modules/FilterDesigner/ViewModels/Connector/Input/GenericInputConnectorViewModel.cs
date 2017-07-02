using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using System.Windows.Media;

namespace Gemini.Demo.Modules.FilterDesigner.ViewModels.Connector.Input
{
    abstract class GenericInputConnectorViewModel<T> : InputConnectorViewModel 
    {
        public ITargetBlock<T> target;

        public GenericInputConnectorViewModel(ITargetBlock<T> target, ElementViewModel element, string name, Color color) : base( element, name, color)
        {
            this.target = target;
            Type = typeof(T);
        }
        public override ConnectionViewModel GetNewConnection()
        {
            //throw new NotImplementedException();
            return null;
        }

        public override Type Type { get; set; }
    }
}
