using System.Collections.Generic;
using Gemini.Modules.Inspector.Inspectors;

namespace Gemini.Modules.Inspector
{
    public class InspectableObject : IInspectableObject
    {
        public IEnumerable<IInspector> Inspectors { get; set; }

        public InspectableObject(IEnumerable<IInspector> inspectors)
        {
            Inspectors = inspectors;
        }
    }
}