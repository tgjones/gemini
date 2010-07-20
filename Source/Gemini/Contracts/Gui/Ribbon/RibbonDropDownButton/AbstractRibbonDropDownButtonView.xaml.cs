using System.ComponentModel.Composition;
using System.Windows;
using System.Windows.Controls;
using Gemini.Contracts.Gui.Ribbon.RibbonButton;

namespace Gemini.Contracts.Gui.Ribbon.RibbonDropDownButton
{
	[Export(ContractNames.ExtensionPoints.Host.Views, typeof(ResourceDictionary))]
	public partial class AbstractRibbonDropDownButtonView : ResourceDictionary
	{
		public AbstractRibbonDropDownButtonView()
		{
			InitializeComponent();
		}

		private void DropDownButton_Loaded(object sender, RoutedEventArgs e)
		{
			Fluent.DropDownButton button = (Fluent.DropDownButton) sender;
			AbstractRibbonDropDownButton viewModel = (AbstractRibbonDropDownButton) button.DataContext;

			button.Items.Clear();
			foreach (IRibbonItem ribbonItemViewModel in viewModel.Items)
			{
				/*Text="{Binding Text}" Command="{Binding Command}" Icon="{Binding Icon}" LargeIcon="{Binding LargeIcon}"
							Visibility="{Binding Visible, Converter={StaticResource BooleanToVisibilityConverter}}"
							ToolTip="{Binding ToolTip}"
							SizeDefinition="{Binding SizeDefinition}"
				 * */
				if (ribbonItemViewModel is AbstractRibbonButton)
				{
					Fluent.MenuItem childButton = new Fluent.MenuItem();
					childButton.DataContext = ribbonItemViewModel;
					childButton.SetBinding(Fluent.MenuItem.TextProperty, "Text");
					childButton.SetBinding(Fluent.MenuItem.CommandParameterProperty, "CommandParameter");
					childButton.SetBinding(Fluent.MenuItem.CommandProperty, "Command");
					childButton.SetBinding(Fluent.MenuItem.IconProperty, "Icon");
					childButton.SetBinding(Fluent.MenuItem.VisibilityProperty, "Visibility");
					childButton.SetBinding(Fluent.MenuItem.ToolTipProperty, "ToolTip");
					childButton.SetBinding(Fluent.MenuItem.SizeDefinitionProperty, "SizeDefinition");
					button.Items.Add(childButton);
				}
				else if (ribbonItemViewModel is RibbonSeparator.RibbonSeparator)
				{
					button.Items.Add(new Separator());
				}
			}
		}
	}
}
