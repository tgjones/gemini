using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Gemini.Framework.Commands
{
    public abstract class CommandHandler
    {
        public virtual void Update(Command command)
        {
            
        }

        public abstract Task Run(Command command);

        /// <summary>
        /// Only override for "list"-type commands
        /// (commands that expand into a list of commands)
        /// </summary>
        public virtual void Update(Command command, List<Command> commands)
        {
            throw new NotSupportedException();
        }
    }
}