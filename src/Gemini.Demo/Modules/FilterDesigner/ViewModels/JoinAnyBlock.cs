using Gridsum.DataflowEx;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace Gemini.Demo.Modules.FilterDesigner
{
    public class JoinAny<T1, T2>
    {
        public Task Completion => throw new NotImplementedException();
        public Dataflow<T1, T1> Target1;
        public Dataflow<T2, T2> Target2;
        public Dataflow<(T1, T2), (T1, T2)> source;
        public bool first;
        T1 I1;
        T2 I2;
        public JoinAny()
        {
            Target1 = new BufferBlock<T1>(new DataflowBlockOptions()).ToDataflow();
            Target2 = new BufferBlock<T2>(new DataflowBlockOptions()).ToDataflow();
            Target1.OutputBlock.AsObservable().Subscribe(x => _in1(x));
            Target2.OutputBlock.AsObservable().Subscribe(x => _in2(x));

            source = new BufferBlock<(T1, T2)>().ToDataflow();

        }
        private void _in2(T2 x)
        {
            I2 = x;
            if (I1 != null) source.SendAsync((I1, I2));
        }
        private void _in1(T1 x)
        {
            I1 = x;
            if (I2 != null) source.SendAsync((I1, I2));
        }
    }
}
