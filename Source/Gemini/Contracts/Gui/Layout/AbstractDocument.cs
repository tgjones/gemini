using System;
using System.ComponentModel;
using Gemini.Contracts.Utilities;

namespace Gemini.Contracts.Gui.Layout
{
	public abstract class AbstractDocument : AbstractLayoutItem, IDocument
	{
		/// <summary>
		/// Override this in the derived class to take an action
		/// when the document is opened.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		public virtual void OnOpened(object sender, EventArgs e) { }

		/// <summary>
		/// Override this in the derived class to take an action
		/// before the document is closed.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		public virtual void OnClosing(object sender, CancelEventArgs e) { }

		/// <summary>
		/// Override this in the derived class to take an action
		/// after the document is closed.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		public virtual void OnClosed(object sender, EventArgs e) { }

		#region " Memento "
		/// <summary>
		/// Used to remember, for instance, the name of the file being edited by this doc.
		/// </summary>
		public string Memento
		{
			get
			{
				return m_Memento;
			}
			protected set
			{
				if (value == null)
				{
					throw new ArgumentNullException();
				}
				if (m_Memento != value)
				{
					m_Memento = value;
					NotifyPropertyChanged(m_MementoArgs);
				}
			}
		}
		private string m_Memento = string.Empty;
		static readonly PropertyChangedEventArgs m_MementoArgs =
				NotifyPropertyChangedHelper.CreateArgs<AbstractDocument>(o => o.Memento);
		static readonly string m_MementoName =
				NotifyPropertyChangedHelper.GetPropertyName<AbstractDocument>(o => o.Memento);

		#endregion


		/// <summary>
		/// This is the factory method.  By default is just returns
		/// the existing instance, but it can be overridden to 
		/// return a new instance based on the memento.
		/// </summary>
		public virtual IDocument CreateDocument(string memento)
		{
			if (Memento != string.Empty && memento != Memento)
			{
				throw new ArgumentException(
						"Can't create more than one document of this type.",
						m_MementoName);
			}
			Memento = memento;
			return this;
		}
	}
}