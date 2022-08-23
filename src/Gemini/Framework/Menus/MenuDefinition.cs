using System;
using System.Windows.Input;
using Gemini.Framework.Commands;

namespace Gemini.Framework.Menus
{
    public class MenuDefinition : MenuDefinitionBase
    {
        private readonly int _sortOrder;
        private readonly string _text;

        public MenuBarDefinition MenuBar { get; private set; }

        public override int SortOrder => _sortOrder;

        public override string Text => _text;

        public override Uri IconSource => null;

        public override KeyGesture KeyGesture => null;

        public override CommandDefinitionBase CommandDefinition => null;

        public MenuDefinition(MenuBarDefinition menuBar, int sortOrder, string text)
        {
            MenuBar = menuBar;
            _sortOrder = sortOrder;
            _text = text;
        }

        public MenuDefinition SetDynamicExclusionPredicate(
            Predicate<MenuDefinitionBase> predicate)
        {
            DynamicExclusionPredicate = predicate;
            return this;
        }
    }
}
