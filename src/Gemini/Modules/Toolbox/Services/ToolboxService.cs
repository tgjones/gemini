using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using Caliburn.Micro;
using Gemini.Modules.Toolbox.Models;

namespace Gemini.Modules.Toolbox.Services
{
    [Export(typeof(IToolboxService))]
    public class ToolboxService : IToolboxService
    {
        private readonly Dictionary<Type, IEnumerable<ToolboxItem>> _toolboxItems;

        public ToolboxService()
        {
            _toolboxItems = AssemblySource.Instance
                .SelectMany(x => x.GetTypes().Where(y => y.GetAttributes<ToolboxItemAttribute>(false).Any()))
                .Select(x =>
                {
                    var attribute = x.GetAttributes<ToolboxItemAttribute>(false).First();
                    return new ToolboxItem
                    {
                        DocumentType = attribute.DocumentType,
                        Name = attribute.Name,
                        Category = attribute.Category,
                        ItemType = x
                    };
                })
                .GroupBy(x => x.DocumentType)
                .ToDictionary(x => x.Key, x => x.AsEnumerable());
        }

        public IEnumerable<ToolboxItem> GetToolboxItems(Type documentType)
        {
            IEnumerable<ToolboxItem> result;
            if (_toolboxItems.TryGetValue(documentType, out result))
                return result;
            return new List<ToolboxItem>();
        }
    }
}