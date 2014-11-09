using System.Threading.Tasks;

namespace Gemini.Framework.Commands
{
    public abstract class CommandHandler
    {
        public virtual void Update(Command command)
        {
            
        }

        public abstract Task Run();
    }
}