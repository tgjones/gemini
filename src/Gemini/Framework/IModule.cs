namespace Gemini.Framework
{
	public interface IModule
	{
        void PreInitialize();
		void Initialize();
        void PostInitialize();
	}
}