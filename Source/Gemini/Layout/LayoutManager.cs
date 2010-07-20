using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Xml.Serialization;
using AvalonDock;
using Gemini.Contracts;
using Gemini.Contracts.Gui.Layout;
using Gemini.Contracts.Services.LoggingService;
using Gemini.Contracts.Utilities;

namespace Gemini.Layout
{
	[Export(ContractNames.Services.Layout.LayoutManager, typeof(ILayoutManager))]
	public class LayoutManager : ILayoutManager
	{
		private readonly DocumentPane _documentPane = new DocumentPane(); // documents are put in here
		private readonly Dictionary<AnchorStyle, DockablePane> _dockablePanes = new Dictionary<AnchorStyle, DockablePane>();
		private readonly ResizingPanel _resizingPanel;
		private readonly ResizingPanel _resizingPanelTop;

		public LayoutManager()
		{
			// Configuration:
			// - DockingManager
			//   - ResizingPanel (Vertical)
			//     - Resizing Panel (Horizontal)
			//       - DockablePane (Left)
			//       - DocumentPane
			//       - DockablePane (Right)
			//     - DockablePane (Bottom)
			_resizingPanel = new ResizingPanel
			{
				Name = "ResizingPanel",
				Orientation = Orientation.Vertical
			};
			_resizingPanelTop = new ResizingPanel
			{
				Name = "ResizingPanelTop",
				Orientation = Orientation.Horizontal
			};
			_resizingPanel.Children.Add(_resizingPanelTop);

			DockablePane dockablePane = new DockablePane();
			ResizingPanel.SetResizeWidth(dockablePane, new GridLength(250));
			_resizingPanelTop.Children.Add(dockablePane);
			_dockablePanes[AnchorStyle.Left] = dockablePane;

			_documentPane.Name = "DocumentPane";
			_resizingPanelTop.Children.Add(_documentPane);

			dockablePane = new DockablePane();
			ResizingPanel.SetResizeWidth(dockablePane, new GridLength(250));
			_resizingPanelTop.Children.Add(dockablePane);
			_dockablePanes[AnchorStyle.Right] = dockablePane;

			dockablePane = new DockablePane { };
			ResizingPanel.SetResizeHeight(dockablePane, new GridLength(200));
			_resizingPanel.Children.Add(dockablePane);
			_dockablePanes[AnchorStyle.Bottom] = dockablePane;

			DockManager.Content = _resizingPanel;

			DockManager.Loaded += DockManager_Loaded;
			DockManager.LayoutUpdated += DockManager_LayoutUpdated;
		}

		[Import(ContractNames.Services.Logging.LoggingService, typeof(ILoggingService))]
		private ILoggingService logger { get; set; }

		/// <summary>
		/// Shows a pad.  If it hasn't been shown before, it shows it
		/// docked to the right side.  Otherwise it restores it to the
		/// previous place that it was before hiding.  Doesn't work
		/// correctly for floating panes (yet).
		/// </summary>
		/// <param name="pad"></param>
		/// <param name="anchor"></param>
		public void ShowPad(IPad pad, AnchorStyle anchor = AnchorStyle.Right)
		{
			if (!m_padLookup.ContainsKey(pad))
			{
				DockableContent content = new DockableContent
				{
					Content = pad,
					Title = pad.Title,
					Name = pad.Name
				};
				m_padLookup.Add(pad, content);
				_dockablePanes[anchor].Items.Add(content);

				pad.PropertyChanged += pad_PropertyChanged;
				content.GotFocus += pad.OnGotFocus;
				content.LostFocus += pad.OnLostFocus;
			}
			m_padLookup[pad].Show(DockManager);
			//DockManager.Show(m_padLookup[pad]);
		}

		/// <summary>
		/// Hides the given pad, if it exists
		/// </summary>
		/// <param name="pad"></param>
		public void HidePad(IPad pad)
		{
			if (m_padLookup.ContainsKey(pad))
			{
				//DockManager.Hide(m_padLookup[pad]);
				m_padLookup[pad].Hide();
			}
		}

		public void HideAllPads()
		{
			foreach (var content in m_padLookup.Values)
			{
				//DockManager.Hide(content);
				content.Hide();
			}
		}

		/// <summary>
		/// Shows a document.  Puts it in the document pane.
		/// </summary>
		/// <param name="document"></param>
		public IDocument ShowDocument(IDocument document, string memento)
		{
			IDocument doc = document.CreateDocument(memento);
			if (doc != null)
			{
				if (!m_documentLookup.ContainsKey(doc))
				{
					DocumentContent content = new DocumentContent();
					content.Content = doc;
					content.Title = doc.Title;
					content.Name = doc.Name;
					m_documentLookup.Add(doc, content);
					_documentPane.Items.Add(content);
					// all these event handlers get unsubscribed in the content_Closing method
					doc.PropertyChanged += new PropertyChangedEventHandler(doc_PropertyChanged);
					content.Closing += new EventHandler<System.ComponentModel.CancelEventArgs>(doc.OnClosing);
					content.Closed += new EventHandler(content_Closed);
					content.GotFocus += new RoutedEventHandler(doc.OnGotFocus);
					content.LostFocus += new RoutedEventHandler(doc.OnLostFocus);
					document.OnOpened(content, new EventArgs());
				}
				//DockManager.Show(m_documentLookup[doc]);
				m_documentLookup[doc].Show(DockManager);
			}
			return doc;
		}

		// Keep the VM properties in sync with
		// the V properties (backwards data binding?)
		void pad_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			var pad = sender as IPad;
			if (pad != null)
			{
				var content = m_padLookup[pad];
				if (content != null)
				{
					if (e.PropertyName == m_TitleName)
					{
						content.Title = pad.Title;
					}
					else if (e.PropertyName == m_NameName)
					{
						content.Name = pad.Name;
					}
				}
			}
		}

		// Keep the VM properties in sync with
		// the V properties (backwards data binding?)
		void doc_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			var doc = sender as IDocument;
			if (doc != null)
			{
				var content = m_documentLookup[doc];
				if (content != null)
				{
					if (e.PropertyName == m_TitleName)
					{
						content.Title = doc.Title;
					}
					else if (e.PropertyName == m_NameName)
					{
						content.Name = doc.Name;
					}
				}
			}
		}
		private static string m_TitleName =
				NotifyPropertyChangedHelper.GetPropertyName<ILayoutItem>(o => o.Title);
		private static string m_NameName =
				NotifyPropertyChangedHelper.GetPropertyName<ILayoutItem>(o => o.Name);

		/// <summary>
		/// Closes the given instance of a document, if it exists
		/// </summary>
		/// <param name="document"></param>
		public void CloseDocument(IDocument document)
		{
			if (m_documentLookup.ContainsKey(document))
			{
				m_documentLookup[document].Close();
			}
		}

		/// <summary>
		/// Close all documents, if they are open
		/// </summary>
		public void CloseAllDocuments()
		{
			while (m_documentLookup.Count > 0)
			{
				IDocument doc = m_documentLookup.Keys.First();
				m_documentLookup[doc].Close();
			}
		}

		// Handles removing documents from the data structure when closed
		void content_Closed(object sender, EventArgs e)
		{
			DocumentContent content = sender as DocumentContent;
			IDocument document = content.Content as IDocument;

			document.PropertyChanged -= new PropertyChangedEventHandler(doc_PropertyChanged);
			content.Closing -= new EventHandler<System.ComponentModel.CancelEventArgs>(document.OnClosing);
			content.Closed -= new EventHandler(content_Closed);
			content.GotFocus -= new RoutedEventHandler(document.OnGotFocus);
			content.LostFocus -= new RoutedEventHandler(document.OnLostFocus);

			m_documentLookup.Remove(document);
			document.OnClosed(sender, e);
		}

		/// <summary>
		/// The View binds to this property
		/// </summary>
		public DockingManager DockManager
		{
			get
			{
				return m_Content;
			}
		}
		private readonly DockingManager m_Content = new DockingManager();

		#region " ILayoutManager Members "

		public event EventHandler Loaded;
		public event EventHandler Unloading;
		public event EventHandler LayoutUpdated;

		/// <summary>
		/// Pass through the LayoutUpdated Event
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void DockManager_LayoutUpdated(object sender, EventArgs e)
		{
			if (LayoutUpdated != null)
			{
				LayoutUpdated(sender, e);
			}
		}

		/// <summary>
		/// Pass through the Loaded Event
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void DockManager_Loaded(object sender, System.Windows.RoutedEventArgs e)
		{
			logger.Info("Layout manager firing Loaded event.");
			if (Loaded != null)
			{
				Loaded(sender, new EventArgs());
			}
		}

		/// <summary>
		/// The workbench is notifying us that we're unloading, so
		/// fire the Unloading event
		/// </summary>
		public void UnloadingWorkbench()
		{
			logger.Info("Layout manager firing Unloading event.");
			if (Unloading != null)
			{
				Unloading(this, new EventArgs());
			}
		}

		/// <summary>
		/// A collection of tool pads, etc., from the workbench
		/// </summary>
		public ReadOnlyCollection<IPad> Pads
		{
			get
			{
				return new ReadOnlyCollection<IPad>(m_padLookup.Keys.ToList());
			}
		}
		private readonly Dictionary<IPad, DockableContent> m_padLookup = new Dictionary<IPad, DockableContent>();

		/// <summary>
		/// A collection of documents from the document manager
		/// </summary>
		public ReadOnlyCollection<IDocument> Documents
		{
			get
			{
				return new ReadOnlyCollection<IDocument>(m_documentLookup.Keys.ToList());
			}
		}
		private readonly Dictionary<IDocument, DocumentContent> m_documentLookup = new Dictionary<IDocument, DocumentContent>();

		/// <summary>
		/// Returns true if the given pad is visible.
		/// </summary>
		/// <param name="pad"></param>
		/// <returns></returns>
		public bool IsVisible(IPad pad)
		{
			if (m_padLookup.ContainsKey(pad))
			{
				DockableContent content = m_padLookup[pad];
				return (content.State != DockableContentState.Hidden);
			}
			else
			{
				return false;
			}
		}

		/// <summary>
		/// Returns true if the given document is visible.
		/// </summary>
		/// <param name="document"></param>
		/// <returns></returns>
		public bool IsVisible(IDocument document)
		{
			if (m_documentLookup.ContainsKey(document))
			{
				DocumentContent content = m_documentLookup[document];
				return content.IsActiveDocument;
			}
			else
			{
				return false;
			}
		}

		// Set this to the list of all available pads and docs so that RestoreLayout
		// can use them to build new ones as needed.
		public void SetAllPadsDocuments(
				IEnumerable<Lazy<IPad, IPadMeta>> AllPads,
				IEnumerable<Lazy<IDocument, IDocumentMeta>> AllDocuments)
		{
			allPads = AllPads;
			allDocuments = AllDocuments;
		}
		private IEnumerable<Lazy<IPad, IPadMeta>> allPads = new Collection<Lazy<IPad, IPadMeta>>();
		private IEnumerable<Lazy<IDocument, IDocumentMeta>> allDocuments = new Collection<Lazy<IDocument, IDocumentMeta>>();

		/// <summary>
		/// Call this method to persist the current layout to disk.
		/// </summary>
		public string SaveLayout()
		{
			if (DockManager.IsLoaded)
			{
				// Save pads
				List<string> padNamesList = new List<string>();
				foreach (IPad pad in Pads)
				{
					// We have to save all the pad names that have ever been
					// shown even if they're hidden now or else the layout
					// manager won't remember where they are when shown again.
					padNamesList.Add(pad.Name);
				}

				string padNames = String.Join(",", padNamesList.ToArray());

				// Save documents
				DocumentList docNamesList = new DocumentList();
				foreach (IDocument doc in Documents)
				{
					docNamesList.AddItem(
							new DocumentItem(doc.Name, doc.Memento)
							);
				}

				XmlSerializer s = new XmlSerializer(typeof(DocumentList));
				StringWriter sw = new StringWriter();
				s.Serialize(sw, docNamesList);
				string docNames = sw.ToString();
				sw.Close();

				// Save layout
				StringWriter swLayout = new StringWriter();
				DockManager.SaveLayout(swLayout);
				string layout = swLayout.ToString();
				swLayout.Close();

				// encode it to base 64 so we don't have to worry about control codes
				byte[] encbuf;

				encbuf = System.Text.Encoding.Unicode.GetBytes(padNames);
				string padNamesEncoded = Convert.ToBase64String(encbuf);

				encbuf = System.Text.Encoding.Unicode.GetBytes(docNames);
				string docNamesEncoded = Convert.ToBase64String(encbuf);

				encbuf = System.Text.Encoding.Unicode.GetBytes(layout);
				string layoutEncoded = Convert.ToBase64String(encbuf);

				// "The base-64 digits in ascending order from zero are the uppercase 
				// characters "A" to "Z", the lowercase characters "a" to "z", 
				// the numerals "0" to "9", and the symbols "+" and "/". 
				// The valueless character, "=", is used for trailing padding."
				return padNamesEncoded + "." + docNamesEncoded + "." + layoutEncoded;
			}
			else
			{
				throw new InvalidOperationException("The DockManager isn't loaded yet.");
			}
		}

		/// <summary>
		/// Call this method to restore the existing layout from disk.
		/// </summary>
		/// <param name="pads">A collection of all possible Pads</param>
		/// <param name="docs">A collection of all possible Documents</param>
		public void RestoreLayout(string blob)
		{
			if (blob == null)
			{
				throw new ArgumentNullException("blob");
			}
			if (blob == string.Empty)
			{
				return;
			}

			// Decode the blob (base64)
			char[] charSeparators = new char[] { '.' };
			string[] sections = blob.Split(charSeparators, 3, StringSplitOptions.None);

			if (sections.Length != 3)
			{
				throw new ArgumentOutOfRangeException("blob");
			}

			byte[] decbuff;

			decbuff = Convert.FromBase64String(sections[0]);
			string padNames = System.Text.Encoding.Unicode.GetString(decbuff);

			decbuff = Convert.FromBase64String(sections[1]);
			string docNames = System.Text.Encoding.Unicode.GetString(decbuff);

			decbuff = Convert.FromBase64String(sections[2]);
			string layout = System.Text.Encoding.Unicode.GetString(decbuff);

			// PADS
			// In case someone tried to show a pad or document before we were loaded
			Collection<IPad> oldPads = new Collection<IPad>();
			foreach (IPad p in Pads)
			{
				oldPads.Add(p);
			}
			Collection<IDocument> oldDocuments = new Collection<IDocument>();
			foreach (IDocument d in Documents)
			{
				oldDocuments.Add(d);
			}

			List<string> padNamesList = padNames.Split(new char[] { ',' }).ToList();
			foreach (var p in allPads)
			{
				if (padNamesList.Contains(p.Metadata.Name))
				{
					ShowPad(p.Value);
				}
			}

			// DOCUMENTS
			DocumentList newList;
			XmlSerializer s = new XmlSerializer(typeof(DocumentList));
			TextReader r = new StringReader(docNames);
			try
			{
				newList = (DocumentList) s.Deserialize(r);
			}
			catch (InvalidOperationException)
			{
				newList = null;
			}
			finally
			{
				r.Close();
			}
			if (newList != null)
			{
				Dictionary<string, Collection<string>> docNamesList = new Dictionary<string, Collection<string>>();
				foreach (DocumentItem item in newList.Items)
				{
					if (!docNamesList.ContainsKey(item.name))
					{
						docNamesList.Add(item.name, new Collection<string>());
					}
					docNamesList[item.name].Add(item.memento);
				}
				foreach (var d in allDocuments)
				{
					if (docNamesList.ContainsKey(d.Metadata.Name))
					{
						foreach (var memento in docNamesList[d.Metadata.Name])
						{
							ShowDocument(d.Value, memento);
						}
					}
				}
			}

			// LAYOUT
			TextReader rLayout = new StringReader(layout);
			DockManager.RestoreLayout(rLayout);

			// now restore the pads and documents that we already wanted to show
			foreach (IPad p in oldPads)
			{
				ShowPad(p);
			}
			foreach (IDocument d in oldDocuments)
			{
				ShowDocument(d, d.Memento);
			}
		}
		#endregion
	}
}