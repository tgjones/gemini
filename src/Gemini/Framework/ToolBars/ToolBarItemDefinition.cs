using System;
using System.Windows.Input;
using Gemini.Framework.Commands;

namespace Gemini.Framework.ToolBars
{
    public abstract class ToolBarItemDefinition
    {
        private readonly ToolBarItemGroupDefinition _group;
        private readonly int _sortOrder;

        public ToolBarItemGroupDefinition Group
        {
            get { return _group; }
        }

        public int SortOrder
        {
            get { return _sortOrder; }
        }

        public abstract string Text { get; }
        public abstract Uri IconSource { get; }
        public abstract KeyGesture KeyGesture { get; }
        public abstract CommandDefinitionBase CommandDefinition { get; }
 
        protected ToolBarItemDefinition(ToolBarItemGroupDefinition group, int sortOrder)
        {
            _group = group;
            _sortOrder = sortOrder;
        }
    }
}