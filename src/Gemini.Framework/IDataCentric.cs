namespace Gemini.Framework
{
    public interface IDataCentric<TData>
    {
        void LoadData(TData data);
    }
}