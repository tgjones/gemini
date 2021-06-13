using System.Windows;
using System.Windows.Controls;

namespace Gemini.Framework.Utils
{
    public static class ItemsControlUtility
    {
        public static void UpdateSeparatorsVisibility(ItemsControl itemsControl)
        {
            Separator lastSeparator = null;
            var foundItemsBefore = false;
            var foundItemsAfter = false;

            var itemsCount = itemsControl.Items.Count;
            for (int i = 0; i < itemsCount; i++)
            {
                var container = itemsControl.ItemContainerGenerator.ContainerFromIndex(i);
                switch (container)
                {
                    case Separator newSeparator:
                        if (lastSeparator != null)
                        {
                            lastSeparator.Visibility = foundItemsBefore && foundItemsAfter ? Visibility.Visible : Visibility.Collapsed;

                            // If last separator is not visible, items found before it should still be considered as item found before next separator.
                            if (lastSeparator.Visibility != Visibility.Visible)
                            {
                                foundItemsBefore = foundItemsBefore || foundItemsAfter;
                                foundItemsAfter = false;
                                lastSeparator = newSeparator;
                                break;
                            }
                        }

                        foundItemsBefore = foundItemsAfter;
                        foundItemsAfter = false;
                        lastSeparator = newSeparator;
                        break;

                    case UIElement uiElement when uiElement.Visibility == Visibility.Visible:
                        foundItemsAfter = true;
                        break;
                }
            }

            if (lastSeparator != null)
                lastSeparator.Visibility = foundItemsBefore && foundItemsAfter ? Visibility.Visible : Visibility.Collapsed;
        }
    }
}