using System;

namespace Gemini.Framework.Menus
{
    public class MenuItemGroupDefinition
    {
        private readonly int _sortOrder;

        public MenuDefinitionBase Parent { get; private set; }

        public int SortOrder => _sortOrder;

        public MenuItemGroupDefinition(MenuDefinitionBase parent, int sortOrder)
        {
            Parent = parent;
            _sortOrder = sortOrder;
        }

        /// <summary>
        /// An optional predicate which is called using this instance,
        /// which when it returns true, informs that the menu should be
        /// excluded from view
        /// </summary>
        public Predicate<MenuItemGroupDefinition> DynamicExclusionPredicate { get; protected set; }

        public MenuItemGroupDefinition SetDynamicExclusionPredicate(
            Predicate<MenuItemGroupDefinition> predicate)
        {
            DynamicExclusionPredicate = predicate;
            return this;
        }
    }
}
