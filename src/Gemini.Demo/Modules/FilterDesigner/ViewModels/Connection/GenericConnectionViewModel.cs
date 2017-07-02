using Gemini.Demo.Modules.FilterDesigner.ViewModels.Connector.Input;
using Gemini.Demo.Modules.FilterDesigner.ViewModels.Connector.Output;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using System.Windows.Media;

namespace Gemini.Demo.Modules.FilterDesigner.ViewModels.Connection
{
    abstract class GenericConnectionViewModel<T> : ConnectionViewModel
    {
        public override Type ConnectionType { get; set; } = typeof(T);

        GenericOutputConnectorViewModel<T> from;
        internal override OutputConnectorViewModel _from { get { return from; } set { from = (GenericOutputConnectorViewModel<T>) value; _restoreLink(); } }

        GenericInputConnectorViewModel<T> to;
        internal override InputConnectorViewModel _to { get  { return to; } set  {
                to = (GenericInputConnectorViewModel<T>)value; _restoreLink(); } }


        private IDisposable _link;

        //raise condition
        void _restoreLink()
        {
            if(_link !=null) _link.Dispose();
            if (_from == null || _to == null) return;
            _link = from.output.LinkTo(to.target,new DataflowLinkOptions());
        }

        private void _exception(Exception x)
        {
            
        }

        public GenericConnectionViewModel(GenericOutputConnectorViewModel<T> from, GenericInputConnectorViewModel<T> to) :base(from,to)
        {
        }

        public GenericConnectionViewModel(GenericOutputConnectorViewModel<T> from) : base(from)
        {
        }

    }
}
