using Common.Logging.Factory;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Logging;
using Gemini.Modules.ErrorList;

namespace Gemini.Demo.Modules.FilterDesigner
{
    [Export(typeof(LoggerFacory))]
    public class LoggerFacory : AbstractCachingLoggerFactoryAdapter
    {
        Logger l;
        IErrorList _display;

        [ImportingConstructor]
        public LoggerFacory(IErrorList display)
        {
            _display = display;
        }


        protected override ILog CreateLogger(string name)
        {
            return l = l ?? new Logger(_display);
        }
    }
}
