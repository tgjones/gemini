namespace Gemini.Modules.Inspector
{
    public class InspectableObjectBuilder : InspectorBuilder<InspectableObjectBuilder>
    {
        public InspectableObject ToInspectableObject()
        {
            return new InspectableObject(Inspectors);
        }
    }
}