using System;
using System.ComponentModel.Composition;
using Caliburn.Micro;

namespace Gemini.Framework.Services
{
    /// <summary>
    /// Used for interop with code that requires an IServiceProvider. This class
    /// defers to the MEF container to resolve services.
    /// </summary>
    [Export(typeof(IServiceProvider))]
    public class ServiceProvider : IServiceProvider
    {
        /// <summary>
        /// Looks up the specified service.
        /// </summary>
        public object GetService(Type serviceType)
        {
            return IoC.GetInstance(serviceType, null);
        }
    }
}