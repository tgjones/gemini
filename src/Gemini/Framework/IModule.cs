using System;
using System.Collections.Generic;

namespace Gemini.Framework
{
	public interface IModule
	{
        IEnumerable<Type> DefaultTools { get; }

        void PreInitialize();
		void Initialize();
        void PostInitialize();
	}
}