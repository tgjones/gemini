using System;
using System.Collections.Generic;
using Caliburn.Micro;

namespace Gemini.Tests.TestInfrastructure
{
    internal sealed class IoCOverrideScope : IDisposable
    {
        private readonly Func<Type, string, object> _originalGetInstance;
        private readonly Func<Type, IEnumerable<object>> _originalGetAllInstances;
        private readonly Action<object> _originalBuildUp;
        private bool _isDisposed;

        internal IoCOverrideScope(
            Func<Type, string, object> getInstance,
            Func<Type, IEnumerable<object>> getAllInstances,
            Action<object> buildUp)
        {
            if (getInstance == null)
                throw new ArgumentNullException(nameof(getInstance));
            if (getAllInstances == null)
                throw new ArgumentNullException(nameof(getAllInstances));
            if (buildUp == null)
                throw new ArgumentNullException(nameof(buildUp));

            _originalGetInstance = IoC.GetInstance;
            _originalGetAllInstances = IoC.GetAllInstances;
            _originalBuildUp = IoC.BuildUp;

            IoC.GetInstance = getInstance;
            IoC.GetAllInstances = getAllInstances;
            IoC.BuildUp = buildUp;
        }

        public void Dispose()
        {
            if (_isDisposed)
                return;

            IoC.GetInstance = _originalGetInstance;
            IoC.GetAllInstances = _originalGetAllInstances;
            IoC.BuildUp = _originalBuildUp;
            _isDisposed = true;
        }
    }
}
