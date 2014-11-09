using System.Threading.Tasks;

namespace Gemini.Framework.Threading
{
    public class TaskUtility
    {
        public static readonly Task Completed = Task.FromResult(true);
    }
}