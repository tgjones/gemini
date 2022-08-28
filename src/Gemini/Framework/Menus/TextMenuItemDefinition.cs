using System;
using System.Windows.Input;
using Gemini.Framework.Commands;

namespace Gemini.Framework.Menus
{
    public class TextMenuItemDefinition : MenuItemDefinition
    {
        private readonly string _text;
        private readonly Uri _iconSource;

        public override string Text => _text;

        public override Uri IconSource => _iconSource;

        public override KeyGesture KeyGesture => null;

        public override CommandDefinitionBase CommandDefinition => null;

        public TextMenuItemDefinition(MenuItemGroupDefinition group, int sortOrder, string text, Uri iconSource = null)
            : base(group, sortOrder)
        {
            _text = text;
            _iconSource = iconSource;
        }
    }
}
