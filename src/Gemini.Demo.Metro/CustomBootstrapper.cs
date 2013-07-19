using System.Collections.Generic;
using System.Reflection;

namespace Gemini.Demo.Metro
{
    public class CustomBootstrapper : AppBootstrapper
    {
        protected override IEnumerable<Assembly> SelectAssemblies()
        {
            var result = new List<Assembly>(base.SelectAssemblies())
            {
                typeof(Gemini.Modules.Metro.ViewModels.MainWindowViewModel).Assembly
            };
            return result;
        }
    }
}