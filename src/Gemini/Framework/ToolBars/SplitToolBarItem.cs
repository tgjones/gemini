using System;
using System.Collections.Generic;
using Caliburn.Micro;

namespace Gemini.Framework.ToolBars
{
    public class SplitToolBarItem : ToolBarItem
    {
        private object _dropDownContent;
        public object DropDownContent
        {
            get { return _dropDownContent; }
            set
            {
                _dropDownContent = value;
                NotifyOfPropertyChange(() => DropDownContent);
            }
        }

        public SplitToolBarItem(string text) : base(text)
        {
        }

        public SplitToolBarItem(string text, Func<IEnumerable<IResult>> execute)
            : base(text, execute)
        {
        }

        public SplitToolBarItem(string text, Func<IEnumerable<IResult>> execute, Func<bool> canExecute) 
            : base(text, execute, canExecute)
        {
        }
    }
}