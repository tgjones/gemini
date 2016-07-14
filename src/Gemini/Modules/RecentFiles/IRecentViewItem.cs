using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gemini.Modules.RecentFiles
{
    public interface IRecentViewItem
    {
        Guid Id { get; }
        string FilePath { get; }
    }
}
