using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace Gemini.Modules.GraphEditor.Controls
{
    public class ConnectionItemsControl : ListBox
    {
        public ConnectionItemsControl()
        {
            SelectionMode = SelectionMode.Extended;
        }

        protected override DependencyObject GetContainerForItemOverride()
        {
            return new ConnectionItem();
            
        }

        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return item is ConnectionItem;
        }
    }
}
