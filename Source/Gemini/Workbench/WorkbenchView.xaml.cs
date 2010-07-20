using System.ComponentModel.Composition;
using System.Windows;
using System.Windows.Data;
using Fluent;
using Gemini.Contracts;
using Gemini.Contracts.Gui.Ribbon;
using Gemini.Contracts.Gui.Ribbon.RibbonButton;
using Gemini.Contracts.Gui.Ribbon.RibbonGroup;
using Gemini.Contracts.Gui.Ribbon.RibbonTab;

namespace Gemini.Workbench
{
	/// <summary>
	/// Interaction logic for WorkBenchView.xaml
	/// </summary>
	[Export(ContractNames.CompositionPoints.Host.MainWindow, typeof(Window))]
	public partial class WorkbenchView : RibbonWindow
	{
		[ImportingConstructor]
		public WorkbenchView([Import(ContractNames.CompositionPoints.Workbench.ViewModel)] Workbench vm)
		{
			InitializeComponent();

			DataContext = vm;

			// ToolBarTray.ToolBars isn't a dependency property, so we
			// have to add the Tool Bars manually
			foreach (IRibbonTab ribbonTabViewModel in vm.RibbonTabs)
			{
				RibbonTabItem ribbonTab = new RibbonTabItem();
				ribbonTab.DataContext = ribbonTabViewModel;

				// Bind the Header Property
				ribbonTab.SetBinding(RibbonTabItem.HeaderProperty, new Binding("Title"));

				foreach (IRibbonGroup ribbonGroupViewModel in ribbonTabViewModel.Groups)
				{
					RibbonGroupBox ribbonGroupBox = new RibbonGroupBox();
					ribbonGroupBox.DataContext = ribbonGroupViewModel;

					// Bind the Header property
					ribbonGroupBox.SetBinding(RibbonGroupBox.HeaderProperty, new Binding("Title"));

					// Bind the Items property
					ribbonGroupBox.ItemTemplate = (DataTemplate) FindResource("RibbonGroupBoxItemTemplate");
					ribbonGroupBox.SetBinding(RibbonGroupBox.ItemsSourceProperty, new Binding("Items"));

					ribbonTab.Groups.Add(ribbonGroupBox);
				}

				rbnRibbon.Tabs.Add(ribbonTab);
			}

			foreach (IRibbonItem ribbonItemViewModel in vm.RibbonBackstageItems)
			{
				if (ribbonItemViewModel is AbstractRibbonButton)
				{
					Fluent.Button childButton = new Fluent.Button();
					childButton.DataContext = ribbonItemViewModel;
					childButton.SetBinding(Fluent.Button.TextProperty, "Text");
					childButton.SetBinding(Fluent.Button.CommandProperty, "Command");
					childButton.SetBinding(Fluent.Button.LargeIconProperty, "LargeIcon");
					childButton.SetBinding(Fluent.Button.IconProperty, "Icon");
					childButton.SetBinding(Fluent.Button.VisibilityProperty, "Visibility");
					childButton.SetBinding(Fluent.Button.ToolTipProperty, "ToolTip");
					childButton.SetBinding(Fluent.Button.SizeDefinitionProperty, "SizeDefinition");
					rbnRibbon.BackstageItems.Add(childButton);
				}
			}

			// hook up the event handlers so the viewmodel knows when we're closing
			this.Closing += vm.OnClosing;
			this.Closed += vm.OnClosed;
		}
	}
}
