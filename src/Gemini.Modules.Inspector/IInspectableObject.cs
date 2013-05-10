using System.Collections.Generic;
using Gemini.Modules.Inspector.Inspectors;

namespace Gemini.Modules.Inspector
{
    public interface IInspectableObject
    {
        IEnumerable<IInspector> Inspectors { get; }
    }
}