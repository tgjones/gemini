using System;
using System.Windows.Input;
using Caliburn.Micro;
using Gemini.Framework.Commands;

namespace Gemini.Framework.Menus
{
    public class MenuBarDefinition
    {

    }

    public abstract class MenuDefinitionBase
    {
        public abstract int SortOrder { get; }
        public abstract string Text { get; }
        public abstract Uri IconSource { get; }
        public abstract KeyGesture KeyGesture { get; }
        public abstract CommandDefinition CommandDefinition { get; }
    }

    public class MenuDefinition : MenuDefinitionBase
    {
        private readonly MenuBarDefinition _menuBar;
        private readonly int _sortOrder;
        private readonly string _text;

        public MenuBarDefinition MenuBar
        {
            get { return _menuBar; }
        }

        public override int SortOrder
        {
            get { return _sortOrder; }
        }

        public override string Text
        {
            get { return _text; }
        }

        public override Uri IconSource
        {
            get { return null; }
        }

        public override KeyGesture KeyGesture
        {
            get { return null; }
        }

        public override CommandDefinition CommandDefinition
        {
            get { return null; }
        }

        public MenuDefinition(MenuBarDefinition menuBar, int sortOrder, string text)
        {
            _menuBar = menuBar;
            _sortOrder = sortOrder;
            _text = text;
        }
    }

    public class MenuItemGroupDefinition
    {
        private readonly MenuDefinitionBase _parent;
        private readonly int _sortOrder;

        public MenuDefinitionBase Parent
        {
            get { return _parent; }
        }

        public int SortOrder
        {
            get { return _sortOrder; }
        }

        public MenuItemGroupDefinition(MenuDefinitionBase parent, int sortOrder)
        {
            _parent = parent;
            _sortOrder = sortOrder;
        }
    }

    public abstract class MenuItemDefinition : MenuDefinitionBase
    {
        private readonly MenuItemGroupDefinition _group;
        private readonly int _sortOrder;

        public MenuItemGroupDefinition Group
        {
            get { return _group; }
        }

        public override int SortOrder
        {
            get { return _sortOrder; }
        }

        protected MenuItemDefinition(MenuItemGroupDefinition group, int sortOrder)
        {
            _group = group;
            _sortOrder = sortOrder;
        }
    }

    public class CommandMenuItemDefinition<TCommandDefinition> : MenuItemDefinition
        where TCommandDefinition : CommandDefinition
    {
        private readonly CommandDefinition _commandDefinition;

        public override string Text
        {
            get
            {
                if (CommandDefinition != null)
                    return CommandDefinition.Text;
                return null;
            }
        }

        public override Uri IconSource
        {
            get
            {
                if (CommandDefinition != null)
                    return CommandDefinition.IconSource;
                return null;
            }
        }

        public override KeyGesture KeyGesture
        {
            get
            {
                if (CommandDefinition != null)
                    return CommandDefinition.KeyGesture;
                return null;
            }
        }

        public override CommandDefinition CommandDefinition
        {
            get { return _commandDefinition; }
        }

        public CommandMenuItemDefinition(MenuItemGroupDefinition group, int sortOrder)
            : base(group, sortOrder)
        {
            _commandDefinition = IoC.Get<ICommandService>().GetCommandDefinition(typeof(TCommandDefinition));
        }
    }

    public class TextMenuItemDefinition : MenuItemDefinition
    {
        private readonly string _text;
        private readonly Uri _iconSource;

        public override string Text
        {
            get { return _text; }
        }

        public override Uri IconSource
        {
            get { return _iconSource; }
        }

        public override KeyGesture KeyGesture
        {
            get { return null; }
        }

        public override CommandDefinition CommandDefinition
        {
            get { return null; }
        }

        public TextMenuItemDefinition(MenuItemGroupDefinition group, int sortOrder, string text, Uri iconSource = null)
            : base(group, sortOrder)
        {
            _text = text;
            _iconSource = iconSource;
        }
    }
}