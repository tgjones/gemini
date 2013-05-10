using System.Collections.Generic;
using Gemini.Modules.Inspector.Inspectors;

namespace Gemini.Modules.Inspector
{
    public interface IInspectableObject
    {
        IEnumerable<IInspector> Inspectors { get; }
    }

    public class InspectableObject : IInspectableObject
    {
        public IEnumerable<IInspector> Inspectors { get; set; }

        public InspectableObject(IEnumerable<IInspector> inspectors)
        {
            Inspectors = inspectors;
        }
    }
}