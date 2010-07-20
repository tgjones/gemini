using System;

namespace Gemini.Contracts.Services.LoggingService
{
	public interface ILoggingService
	{
		void Debug(object message);
		void DebugWithFormat(string format, params object[] args);
		void Info(object message);
		void InfoWithFormat(string format, params object[] args);
		void Warn(object message);
		void Warn(object message, Exception exception);
		void WarnWithFormat(string format, params object[] args);
		void Error(object message);
		void Error(object message, Exception exception);
		void ErrorWithFormat(string format, params object[] args);
		void Fatal(object message);
		void Fatal(object message, Exception exception);
		void FatalWithFormat(string format, params object[] args);

		bool IsDebugEnabled { get; }
		bool IsInfoEnabled { get; }
		bool IsWarnEnabled { get; }
		bool IsErrorEnabled { get; }
		bool IsFatalEnabled { get; }
	}
}