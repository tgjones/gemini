using System;
using System.ComponentModel;
using Gemini.Contracts.Gui.ViewModel;
using Gemini.Contracts.Utilities;

namespace Gemini.Contracts.Services.ExtensionService
{
	public abstract class AbstractExtension : AbstractViewModel, IExtension
	{

		#region " ID "
		/// <summary>
		/// This is a unique string used to identify the extension.
		/// The point of this is so that other extensions can insert themselves
		/// before or after this item in the list of extensions.  Example: "File"
		/// Should *always* be set in the derived class's constructor.
		/// </summary>
		public string ID
		{
			get
			{
				return m_ID;
			}
			protected set
			{
				if (value == null)
				{
					throw new ArgumentNullException(m_IDName);
				}
				if (value == string.Empty)
				{
					throw new ArgumentException(m_IDName);
				}
				if (m_ID != value)
				{
					m_ID = value;
					NotifyPropertyChanged(m_IDArgs);
				}
			}
		}
		// Defaults to a Guid so at least it will be unique.
		private string m_ID = Guid.NewGuid().ToString();
		static readonly PropertyChangedEventArgs m_IDArgs =
				NotifyPropertyChangedHelper.CreateArgs<AbstractExtension>(o => o.ID);
		static readonly string m_IDName =
				NotifyPropertyChangedHelper.GetPropertyName<AbstractExtension>(o => o.ID);
		#endregion

		#region " InsertRelativeToID "
		/// <summary>
		/// If specified, the extension list will try to insert this extension 
		/// before or after the extension with this ID.
		/// </summary>
		public string InsertRelativeToID
		{
			get
			{
				return m_InsertRelativeToID;
			}
			protected set
			{
				if (m_InsertRelativeToID != value)
				{
					m_InsertRelativeToID = value;
					NotifyPropertyChanged(m_InsertRelativeToIDArgs);
				}
			}
		}
		private string m_InsertRelativeToID = null;
		static readonly PropertyChangedEventArgs m_InsertRelativeToIDArgs =
				NotifyPropertyChangedHelper.CreateArgs<AbstractExtension>(o => o.InsertRelativeToID);
		#endregion

		#region " BeforeOrAfter "
		/// <summary>
		/// If specified, the extension list will try to insert this extension 
		/// before or after the extension with this ID.
		/// </summary>
		public RelativeDirection BeforeOrAfter
		{
			get
			{
				return m_BeforeOrAfter;
			}
			protected set
			{
				if (m_BeforeOrAfter != value)
				{
					m_BeforeOrAfter = value;
					NotifyPropertyChanged(m_BeforeOrAfterArgs);
				}
			}
		}
		private RelativeDirection m_BeforeOrAfter = RelativeDirection.Before;
		static readonly PropertyChangedEventArgs m_BeforeOrAfterArgs =
				NotifyPropertyChangedHelper.CreateArgs<AbstractExtension>(o => o.BeforeOrAfter);
		#endregion


	}
}