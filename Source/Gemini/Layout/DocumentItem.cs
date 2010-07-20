using System.Xml.Serialization;

namespace Gemini.Layout
{
	public class DocumentItem
	{
		[XmlAttribute("name")]
		public string name;
		[XmlAttribute("memento")]
		public string memento;

		public DocumentItem()
		{
		}

		public DocumentItem(string Name, string Memento)
		{
			name = Name;
			memento = Memento;
		}
	}
}