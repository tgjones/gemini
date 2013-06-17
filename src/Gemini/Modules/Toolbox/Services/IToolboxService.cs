using System;
using System.Collections.Generic;
using Gemini.Modules.Toolbox.Models;

namespace Gemini.Modules.Toolbox.Services
{
    public interface IToolboxService
    {
        IEnumerable<ToolboxItem> GetToolboxItems(Type documentType);
    }
}