using System;
using Caliburn.PresentationFramework.ApplicationModel;

namespace Gemini.Framework.Results
{
	public static class ExtensionMethods
    {
        public static IOpenResult<TTarget> ConfigureChild<TTarget>(this IOpenResult<TTarget> result, Action<TTarget> configure)
            where TTarget : IExtendedPresenter
        {
            result.OnConfigure = configure;
            return result;
        }

        public static IOpenResult<TTarget> WhenShuttingDown<TTarget>(this IOpenResult<TTarget> result, Action<TTarget> onShutdown)
            where TTarget : IExtendedPresenter
        {
            result.OnShutDown = onShutdown;
            return result;
        }

        public static IOpenResult<TTarget> WithData<TTarget, TData>(this IOpenResult<TTarget> result, TData data)
            where TTarget : IExtendedPresenter
        {
            result.SetData(data);
            return result;
        }
    }
}