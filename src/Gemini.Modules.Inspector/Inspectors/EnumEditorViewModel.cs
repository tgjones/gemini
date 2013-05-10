using System;
using System.Collections.Generic;
using System.Linq;

namespace Gemini.Modules.Inspector.Inspectors
{
    public class EnumValueViewModel<TEnum>
    {
        public TEnum Value { get; set; }
        public string Text { get; set; }
    }

    public class EnumEditorViewModel<TEnum> : EditorBase<TEnum>
    {
        private readonly List<EnumValueViewModel<TEnum>> _items;
        public IEnumerable<EnumValueViewModel<TEnum>> Items
        {
            get { return _items; }
        }

        public EnumEditorViewModel()
        {
            _items = Enum.GetValues(typeof(TEnum)).Cast<TEnum>().Select(x => new EnumValueViewModel<TEnum>
            {
                Value = x,
                Text = Enum.GetName(typeof(TEnum), x)
            }).ToList();
        }
    }
}