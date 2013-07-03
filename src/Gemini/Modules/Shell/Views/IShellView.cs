using System;
using Gemini.Framework;

namespace Gemini.Modules.Shell.Views
{
    public interface IShellView
    {
        void LoadLayout(Action<ITool> addToolCallback);
    }
}