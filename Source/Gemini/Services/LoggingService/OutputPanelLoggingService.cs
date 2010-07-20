using System;
using System.ComponentModel.Composition;
using Gemini.Contracts;
using Gemini.Contracts.Services.LoggingService;

namespace Gemini.Services.LoggingService
{
	[Export(ContractNames.Services.Logging.LoggingService, typeof(ILoggingService))]
	public class OutputPanelLoggingService : ILoggingService
	{
		public void Debug(object message)
		{
			
		}

		public void DebugWithFormat(string format, params object[] args)
		{
			
		}

		public void Info(object message)
		{
			
		}

		public void InfoWithFormat(string format, params object[] args)
		{
			
		}

		public void Warn(object message)
		{
			throw new NotImplementedException();
		}

		public void Warn(object message, Exception exception)
		{
			throw new NotImplementedException();
		}

		public void WarnWithFormat(string format, params object[] args)
		{
			throw new NotImplementedException();
		}

		public void Error(object message)
		{
			throw new NotImplementedException();
		}

		public void Error(object message, Exception exception)
		{
			throw new NotImplementedException();
		}

		public void ErrorWithFormat(string format, params object[] args)
		{
			throw new NotImplementedException();
		}

		public void Fatal(object message)
		{
			throw new NotImplementedException();
		}

		public void Fatal(object message, Exception exception)
		{
			throw new NotImplementedException();
		}

		public void FatalWithFormat(string format, params object[] args)
		{
			throw new NotImplementedException();
		}

		public bool IsDebugEnabled
		{
			get { throw new NotImplementedException(); }
		}

		public bool IsInfoEnabled
		{
			get { throw new NotImplementedException(); }
		}

		public bool IsWarnEnabled
		{
			get { throw new NotImplementedException(); }
		}

		public bool IsErrorEnabled
		{
			get { throw new NotImplementedException(); }
		}

		public bool IsFatalEnabled
		{
			get { throw new NotImplementedException(); }
		}
	}
}