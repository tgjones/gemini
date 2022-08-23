using System;

namespace Gemini.Framework.Menus
{
    public abstract class MenuItemDefinition : MenuDefinitionBase
    {
        private readonly int _sortOrder;

        public MenuItemGroupDefinition Group { get; private set; }

        public override int SortOrder => _sortOrder;

        protected MenuItemDefinition(MenuItemGroupDefinition group, int sortOrder)
        {
            Group = group;
            _sortOrder = sortOrder;
        }

        public MenuItemDefinition SetDynamicExclusionPredicate(
            Predicate<MenuDefinitionBase> predicate)
        {
            DynamicExclusionPredicate = predicate;
            return this;
        }
    }
}
