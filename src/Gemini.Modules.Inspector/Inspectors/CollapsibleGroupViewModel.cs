using System.Collections.Generic;

namespace Gemini.Modules.Inspector.Inspectors
{
    public class CollapsibleGroupViewModel : InspectorBase
    {
        private static readonly Dictionary<string, bool> PersistedExpandCollapseStates = new Dictionary<string, bool>();

        private readonly string _name;
        private readonly IEnumerable<IInspector> _children;

        public override string Name
        {
            get { return _name; }
        }

        public override bool IsReadOnly
        {
            get { return false; }
        }

        public IEnumerable<IInspector> Children
        {
            get { return _children; }
        }

        private bool _isExpanded;
        public bool IsExpanded
        {
            get { return _isExpanded; }
            set
            {
                _isExpanded = value;
                PersistedExpandCollapseStates[_name] = value; // TODO: Key should be full path to this group, not just the name.
                NotifyOfPropertyChange();
            }
        }

        public CollapsibleGroupViewModel(string name, IEnumerable<IInspector> children)
        {
            _name = name;
            _children = children;

            if (!PersistedExpandCollapseStates.TryGetValue(_name, out _isExpanded))
                _isExpanded = true;
        }
    }
}