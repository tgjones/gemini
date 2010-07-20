using System.Collections.Generic;
using System.ComponentModel.Composition;
using Gemini.Contracts.Services.LoggingService;

namespace Gemini.Contracts.Services.ExtensionService
{
	/// <summary>
	/// Utility methods and functions to work with IExtension objects.
	/// </summary>
	[Export((ContractNames.Services.Host.ExtensionService), typeof(IExtensionService))]
	public class ExtensionService : IExtensionService
	{
		[Import(ContractNames.Services.Logging.LoggingService, typeof(ILoggingService))]
		private ILoggingService logger { get; set; }

		/// <summary>
		/// Takes a collection of extensions and returns a sorted list
		/// of those extensions based on the InsertBeforeID  
		/// property of each extension.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="extensions"></param>
		/// <returns></returns>
		public IList<T> Sort<T>(IEnumerable<T> extensionCollection) where T : IExtension
		{
			List<T> extensions = new List<T>(extensionCollection);
			List<T> sortedExtensions = new List<T>();
			List<T> unsortedExtensions = new List<T>();
			foreach (T newExtension in extensions)
			{
				if (newExtension.InsertRelativeToID == null)
				{
					sortedExtensions.Add(newExtension);
				}
				else if (FindByID(newExtension.InsertRelativeToID, extensions) == -1)
				{
					// found a configuration error
					logger.ErrorWithFormat("Configuration error with extension ID {0}, InsertBeforeID of {1} doesn't exist.",
							newExtension.ID, newExtension.InsertRelativeToID);
					sortedExtensions.Add(newExtension);
				}
				else
				{
					unsortedExtensions.Add(newExtension);
				}
			}
			while (unsortedExtensions.Count > 0)
			{
				List<T> stillUnsortedExtensions = new List<T>();
				int startingCount = unsortedExtensions.Count;
				foreach (T newExtension in unsortedExtensions)
				{
					int index = FindByID(newExtension.InsertRelativeToID, sortedExtensions);
					if (index > -1)
					{
						if (newExtension.BeforeOrAfter == RelativeDirection.Before)
						{
							sortedExtensions.Insert(index, newExtension);
						}
						else
						{
							if (index == sortedExtensions.Count - 1)
							{
								//it's to be inserted after the last item in the list
								sortedExtensions.Add(newExtension);
							}
							else
							{
								sortedExtensions.Insert(index + 1, newExtension);
							}
						}
					}
					else
					{
						stillUnsortedExtensions.Add(newExtension);
					}
				}
				if (startingCount == stillUnsortedExtensions.Count)
				{
					// We didn't make any progress
					logger.Error("Configuration error with one of these extensions:");
					foreach (IExtension ext in stillUnsortedExtensions)
					{
						logger.ErrorWithFormat("ID = {0}, InsertBeforeID = {1}", ext.ID, ext.InsertRelativeToID);
					}
					// Pick one and add it at the end.
					sortedExtensions.Add(stillUnsortedExtensions[0]);
					stillUnsortedExtensions.RemoveAt(0);
				}
				unsortedExtensions = stillUnsortedExtensions;
			}
			return sortedExtensions;
		}

		/// <summary>
		/// Returns the index of the extension with the given ID,
		/// or -1 if not found.
		/// </summary>
		/// <param name="ID"></param>
		/// <param name="extensions"></param>
		/// <returns></returns>
		private int FindByID<T>(string ID, IList<T> extensions) where T : IExtension
		{
			for (int i = 0; i < extensions.Count; i++)
			{
				if (extensions[i].ID == ID)
				{
					return i;
				}
			}
			return -1;
		}

	}
}