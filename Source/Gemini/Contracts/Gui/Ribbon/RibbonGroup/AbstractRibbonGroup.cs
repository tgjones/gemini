using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using Gemini.Contracts.Gui.Controls;
using Gemini.Contracts.Services.ExtensionService;
using Gemini.Contracts.Utilities;

namespace Gemini.Contracts.Gui.Ribbon.RibbonGroup
{
	public abstract class AbstractRibbonGroup : AbstractControl, IRibbonGroup, IPartImportsSatisfiedNotification
	{
		#region " Title "
		public string Title
		{
			get
			{
				return m_Title;
			}
			protected set
			{
				if (m_Title != value)
				{
					m_Title = value;
					NotifyPropertyChanged(m_TitleArgs);
				}
			}
		}

		private string m_Title = null;
		static readonly PropertyChangedEventArgs m_TitleArgs =
				NotifyPropertyChangedHelper.CreateArgs<AbstractRibbonGroup>(o => o.Title);
		#endregion

		[Import(ContractNames.Services.Host.ExtensionService)]
		private IExtensionService ExtensionService { get; set; }

		public IEnumerable<IRibbonItem> Items { get; protected set; }

		protected abstract IEnumerable<IRibbonItem> ItemsInternal { get; set; }

		public void OnImportsSatisfied()
		{
			Items = ExtensionService.Sort(ItemsInternal);
		}
	}

	public class RibbonGroup : AbstractRibbonGroup
	{
		protected override IEnumerable<IRibbonItem> ItemsInternal { get; set; }

		public RibbonGroup(IEnumerable<IRibbonItem> ribbonItems, string title)
		{
			Items = ItemsInternal = ribbonItems;
			Title = title;
		}
	}
}