namespace Gemini.Modules.Inspector.Inspectors
{
    public interface IInspector
    {
        string Name { get; }
        bool IsReadOnly { get; }
    }
}