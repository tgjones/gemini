using System.ComponentModel;
using System.ComponentModel.Composition;
using Gemini.Contracts.Services.LoggingService;

namespace Gemini.Contracts.Gui.ViewModel
{
	[Export(ContractNames.ExtensionPoints.Host.Void, typeof(object))] // us to load so we get MEF to run
	public class AbstractViewModel : IViewModel
	{
		#region " logger singleton "
		/// <summary>
		/// Anyone who inherits from AbstractViewModel gets a free
		/// reference to the logging service.
		/// </summary>
		[Import(ContractNames.Services.Logging.LoggingService, typeof(ILoggingService))]
		protected ILoggingService logger
		{
			get
			{
				return m_logger;
			}
			set
			{
				m_logger = value;
			}
		}
		private static ILoggingService m_logger = null;
		#endregion

		#region " Implement INotifyPropertyChanged "
		/// <summary>
		/// Call this method to raise the PropertyChanged event when
		/// a property changes.  Note that you should use the
		/// NotifyPropertyChangedHelper class to create a cached
		/// copy of the PropertyChangedEventArgs object to pass
		/// into this method.  Usage:
		/// 
		/// static readonly PropertyChangedEventArgs m_$PropertyName$Args = 
		///     NotifyPropertyChangedHelper.CreateArgs<$ClassName$>(o => o.$PropertyName$);
		/// 
		/// In your property setter:
		///     PropertyChanged(this, m_$PropertyName$Args)
		/// 
		/// </summary>
		/// <param name="e">A cached event args object</param>
		protected void NotifyPropertyChanged(PropertyChangedEventArgs e)
		{
			var evt = PropertyChanged;
			if (evt != null)
			{
				evt(this, e);
			}
		}
		public event PropertyChangedEventHandler PropertyChanged;
		#endregion
	}
}