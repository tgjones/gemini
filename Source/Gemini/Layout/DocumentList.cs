using System.Collections;
using System.Xml.Serialization;

namespace Gemini.Layout
{
	[XmlRoot("documentList")]
	public class DocumentList
	{
		private ArrayList listOfDocs = new ArrayList();

		[XmlElement("item")]
		public DocumentItem[] Items
		{
			get
			{
				DocumentItem[] items = new DocumentItem[listOfDocs.Count];
				listOfDocs.CopyTo(items);
				return items;
			}
			set
			{
				if (value == null) return;
				DocumentItem[] items = (DocumentItem[]) value;
				listOfDocs.Clear();
				foreach (DocumentItem item in items)
					listOfDocs.Add(item);
			}
		}

		public int AddItem(DocumentItem item)
		{
			return listOfDocs.Add(item);
		}
	}
}