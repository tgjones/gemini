using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using Gemini.Contracts.Gui.Controls;
using Gemini.Contracts.Gui.Ribbon.RibbonGroup;
using Gemini.Contracts.Services.ExtensionService;
using Gemini.Contracts.Utilities;

namespace Gemini.Contracts.Gui.Ribbon.RibbonTab
{
	public abstract class AbstractRibbonTab : AbstractControl, IRibbonTab, IPartImportsSatisfiedNotification
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
				NotifyPropertyChangedHelper.CreateArgs<AbstractRibbonTab>(o => o.Title);

		#endregion

		[Import(ContractNames.Services.Host.ExtensionService)]
		private IExtensionService ExtensionService { get; set; }

		public IEnumerable<IRibbonGroup> Groups { get; private set; }

		protected abstract IEnumerable<IRibbonGroup> GroupsInternal { get; set; }

		public void OnImportsSatisfied()
		{
			Groups = ExtensionService.Sort(GroupsInternal);
		}
	}
}