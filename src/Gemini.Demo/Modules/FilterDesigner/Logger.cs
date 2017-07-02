using Common.Logging.Simple;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Logging;
using Common.Logging.Factory;
using System.ComponentModel.Composition;
using Gemini.Modules.ErrorList;

namespace Gemini.Demo.Modules.FilterDesigner
{
    public class Logger : AbstractLogger
    {
        public override bool IsTraceEnabled => true;

        public override bool IsDebugEnabled => true;

        public override bool IsInfoEnabled => true;

        public override bool IsWarnEnabled => true;

        public override bool IsErrorEnabled => true;

        public override bool IsFatalEnabled => true;

        IErrorList _display;


        public Logger(IErrorList display)
        {
            _display = display;
        }


        protected override void WriteInternal(LogLevel level, object message, Exception exception)
        {

            if(message !=null) _display.AddItem(ErrorListItemType.Message, message.ToString());
            if(exception != null) _display.AddItem(ErrorListItemType.Error, exception.ToString());

        }
    }
}
