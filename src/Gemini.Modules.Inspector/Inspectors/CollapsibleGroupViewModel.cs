using System.Collections.Generic;

namespace Gemini.Modules.Inspector.Inspectors
{
    public class CollapsibleGroupViewModel : InspectorBase
    {
        private readonly string _name;
        private readonly IEnumerable<IInspector> _children;

        public override string Name
        {
            get { return _name; }
        }

        public IEnumerable<IInspector> Children
        {
            get { return _children; }
        }

        public CollapsibleGroupViewModel(string name, IEnumerable<IInspector> children)
        {
            _name = name;
            _children = children;
        }
    }
}