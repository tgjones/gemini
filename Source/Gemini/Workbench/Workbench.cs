using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Windows.Input;
using Gemini.Contracts;
using Gemini.Contracts.Gui.Layout;
using Gemini.Contracts.Gui.Ribbon;
using Gemini.Contracts.Gui.Ribbon.RibbonTab;
using Gemini.Contracts.Gui.ViewModel;
using Gemini.Contracts.Services.ExtensionService;
using Gemini.Contracts.Utilities;

namespace Gemini.Workbench
{
	[Export(ContractNames.CompositionPoints.Workbench.ViewModel)]
	public class Workbench : AbstractViewModel, IPartImportsSatisfiedNotification
	{
		#region Imported properties

		[Import(ContractNames.Services.Host.ExtensionService)]
		private IExtensionService extensionService { get; set; }

		[Import(ContractNames.Services.Layout.LayoutManager, typeof(ILayoutManager))]
		public ILayoutManager LayoutManager { get; set; }

		[ImportMany(ContractNames.ExtensionPoints.Workbench.Ribbon.Tabs, typeof(IRibbonTab), AllowRecomposition = true)]
		private IEnumerable<IRibbonTab> ribbonTabs { get; set; }

		[ImportMany(ContractNames.ExtensionPoints.Workbench.Ribbon.Backstage, typeof(IRibbonItem), AllowRecomposition = true)]
		private IEnumerable<IRibbonItem> ribbonBackstageItems { get; set; }

		[ImportMany(ContractNames.ExtensionPoints.Workbench.Pads, typeof(IPad), AllowRecomposition = true)]
		private IEnumerable<Lazy<IPad, IPadMeta>> pads { get; set; }

		[ImportMany(ContractNames.ExtensionPoints.Workbench.Documents, typeof(IDocument), AllowRecomposition = true)]
		private IEnumerable<Lazy<IDocument, IDocumentMeta>> documents { get; set; }

		#endregion

		public void OnImportsSatisfied()
		{
			// when this is called, all imports that could be satisfied have been satisfied.
			RibbonTabs = extensionService.Sort(ribbonTabs);
			RibbonBackstageItems = extensionService.Sort(ribbonBackstageItems);
			LayoutManager.SetAllPadsDocuments(pads, documents);
		}

		#region "ToolBars"

		public IEnumerable<IRibbonTab> RibbonTabs
		{
			get
			{
				return m_RibbonTabs;
			}
			set
			{
				if (m_RibbonTabs != value)
				{
					m_RibbonTabs = value;
					NotifyPropertyChanged(m_RibbonTabsArgs);
				}
			}
		}
		private IEnumerable<IRibbonTab> m_RibbonTabs = null;
		static readonly PropertyChangedEventArgs m_RibbonTabsArgs =
				NotifyPropertyChangedHelper.CreateArgs<Workbench>(o => o.RibbonTabs);

		public IEnumerable<IRibbonItem> RibbonBackstageItems
		{
			get
			{
				return m_RibbonBackstageItems;
			}
			set
			{
				if (m_RibbonTabs != value)
				{
					m_RibbonBackstageItems = value;
					NotifyPropertyChanged(m_RibbonBackstageItemsArgs);
				}
			}
		}
		private IEnumerable<IRibbonItem> m_RibbonBackstageItems = null;
		static readonly PropertyChangedEventArgs m_RibbonBackstageItemsArgs =
				NotifyPropertyChangedHelper.CreateArgs<Workbench>(o => o.RibbonBackstageItems);

		#endregion

		private readonly CommandBindingCollection _commandBindings = new CommandBindingCollection();
		public CommandBindingCollection CommandBindings
		{
			get { return _commandBindings; }
		}

		public void AddCommandBinding(ICommand command, ExecutedRoutedEventHandler executed, CanExecuteRoutedEventHandler canExecute)
		{
			CommandBinding commandBinding = new CommandBinding(command, executed, canExecute);
			CommandManager.RegisterClassCommandBinding(GetType(), commandBinding);
			CommandBindings.Add(commandBinding);
		}

		public void OnClosing(object sender, EventArgs e)
		{
			logger.Info("Workbench closing.");
			LayoutManager.UnloadingWorkbench();
		}

		public void OnClosed(object sender, EventArgs e)
		{
			logger.Info("Workbench closed.");
		}

	}
}