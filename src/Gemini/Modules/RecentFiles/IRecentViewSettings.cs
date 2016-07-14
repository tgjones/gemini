using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gemini.Modules.RecentFiles
{
    public interface IRecentViewSettings
    {
        IReadOnlyList<IRecentViewItem> RecentItems { get; }
    }
}
