using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using AvalonDock;
using Caliburn.PresentationFramework;
using Fluent;
using Gemini.Framework.Ribbon;
using Gemini.Framework.Services;
using Microsoft.Practices.ServiceLocation;
using Caliburn.PresentationFramework.ApplicationModel;
using Pane = Gemini.Framework.Services.Pane;

namespace Gemini.Modules.Shell.Views
{
	/// <summary>
	/// Interaction logic for Shell.xaml
	/// </summary>
	public partial class ShellView : RibbonWindow, IShellView
	{
		private readonly Dictionary<Pane, DockablePane> _panes;
		private readonly IViewStrategy _viewStrategy;
		private readonly IBinder _binder;

		public ShellView()
			: this(ServiceLocator.Current.GetInstance<IBinder>(), ServiceLocator.Current.GetInstance<IViewStrategy>()) { }

		public ShellView(IBinder binder, IViewStrategy viewStrategy)
		{
			InitializeComponent();

			_viewStrategy = viewStrategy;
			_binder = binder;
			_panes = new Dictionary<Pane, DockablePane>
			{
				{ Pane.Left, Left },
				{ Pane.Right, Right },
				{ Pane.Bottom, Bottom }
			};
		}

		public void InitializeRibbon(IRibbon ribbonModel)
		{
			// ToolBarTray.ToolBars isn't a dependency property, so we
			// have to add the Tool Bars manually
			foreach (IRibbonTab ribbonTabViewModel in ribbonModel.Tabs)
				InitializeRibbonTab(ribbonTabViewModel);

			foreach (IRibbonBackstageItem ribbonItemViewModel in ribbonModel.BackstageItems)
			{
				if (ribbonItemViewModel is RibbonButton)
				{
					Fluent.Button childButton = new Fluent.Button();
					childButton.DataContext = ribbonItemViewModel;
					childButton.SetBinding(Fluent.Button.TextProperty, "Text");
					childButton.SetBinding(Caliburn.PresentationFramework.Actions.Action.TargetProperty, new Binding());
					childButton.SetBinding(Message.AttachProperty, "ActionText");
					childButton.SetBinding(Fluent.Button.LargeIconProperty, "LargeIcon");
					childButton.SetBinding(Fluent.Button.IconProperty, "Icon");
					//childButton.SetBinding(Fluent.Button.VisibilityProperty, "Visibility");
					//childButton.SetBinding(Fluent.Button.ToolTipProperty, "ToolTip");
					//childButton.SetBinding(Fluent.Button.SizeDefinitionProperty, "SizeDefinition");
					rbnRibbon.BackstageItems.Add(childButton);
				}
			}
		}

		private void InitializeRibbonTab(IRibbonTab ribbonTabViewModel)
		{
			RibbonTabItem ribbonTab = new RibbonTabItem();
			ribbonTab.DataContext = ribbonTabViewModel;

			// Bind the Header Property
			ribbonTab.SetBinding(RibbonTabItem.HeaderProperty, new Binding("Title"));

			foreach (IRibbonGroup ribbonGroupViewModel in ribbonTabViewModel.Groups)
				InitializeRibbonGroup(ribbonTab, ribbonGroupViewModel);

			rbnRibbon.Tabs.Add(ribbonTab);
		}

		private void InitializeRibbonGroup(RibbonTabItem ribbonTab, IRibbonGroup ribbonGroupViewModel)
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

		public void ShowTool(Pane pane, IExtendedPresenter model)
		{
			var view = _viewStrategy.GetView(model, null, null);

			if (view is DockableContent)
			{
				DockableContent dockableContentView = (DockableContent) view;
				dockableContentView.Show(
					Manager,
					(AnchorStyle) Enum.Parse(typeof(AnchorStyle), pane.ToString()));
			}
			else
			{
				var content = CreateContent<DockableContent>(view, model);

				var host = _panes[pane];
				host.Items.Add(content);

				Refresh();
			}
		}

		public void OpenDocument(IExtendedPresenter model)
		{
			var view = _viewStrategy.GetView(model, null, null);

			if (view is DocumentContent)
				((DocumentContent) view).Show(Manager);
			else
			{
				var content = CreateContent<DocumentContent>(view, model);

				content.PropertyChanged += HostPropertyChanged;
				content.Closing += (s, e) => HostClosing(model, e);

				Document.Items.Add(content);

				model.WasShutdown += delegate
				{
					Document.Items.Remove(content);
				};

				//content.SetAsActive();
				Manager.ActiveDocument = content;
			}
		}

		private T CreateContent<T>(object view, IExtendedPresenter model)
				where T : ManagedContent, new()
		{
			var contentControl = new T { Content = view };

			contentControl.SetBinding(
					ManagedContent.TitleProperty,
					"DisplayName"
					);

			_binder.Bind(model, contentControl, null);

			foreach (var binding in InputBindings)
			{
				contentControl.InputBindings.Add((InputBinding) binding);
			}

			foreach (var binding in CommandBindings)
			{
				contentControl.CommandBindings.Add((CommandBinding) binding);
			}

			return contentControl;
		}

		private void Refresh()
		{
			//HACK: For some reason, the layout doesn't update unless I change the size...
			SetValue(WidthProperty, ActualWidth + 1);
			SetValue(WidthProperty, ActualWidth - 1);
		}

		private void HostPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName != "IsActiveDocument") return;

			var host = (ManagedContent) sender;

			if (host.IsActiveDocument)
				((IShell) DataContext).ActivateDocument((IExtendedPresenter) host.DataContext);
		}

		private void HostClosing(IExtendedPresenter rootModel, CancelEventArgs e)
		{
			((IShell) DataContext).CloseDocument(rootModel, success => { e.Cancel = !success; });
		}
	}
}