using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using Caliburn.Micro;
using Gemini.Framework;
using Gemini.Framework.Services;
using Gemini.Modules.Shell.Views;

namespace Gemini.Modules.Shell.Services
{
    [Export(typeof(ILayoutItemStatePersister))]
    public class LayoutItemStatePersister : ILayoutItemStatePersister
    {
        private static readonly Type LayoutBaseType = typeof(ILayoutItem);

        public bool SaveState(IShell shell, IShellView shellView, string fileName)
        {
            try
            {
                using (var writer = new BinaryWriter(new FileStream(fileName, FileMode.Create, FileAccess.Write)))
                {
                    IEnumerable<ILayoutItem> itemStates = shell.Documents.Concat(shell.Tools.Cast<ILayoutItem>());

                    int itemCount = 0;
                    // reserve some space for items count, it'll be updated later
                    writer.Write(itemCount);

                    foreach (var item in itemStates)
                    {
                        if (!item.ShouldReopenOnStart)
                            continue;

                        var itemType = item.GetType();
                        List<ExportAttribute> exportAttributes = itemType
                                .GetCustomAttributes(typeof(ExportAttribute), false)
                                .Cast<ExportAttribute>().ToList();

                        // get exports with explicit types or names that inherit from ILayoutItem
                        var exportTypes = new List<Type>();
                        var foundExportContract = false;
                        foreach (var att in exportAttributes)
                        {
                            // select the contract type if it is of type ILayoutitem.
                            var type = att.ContractType;
                            if (LayoutBaseType.IsAssignableFrom(type))
                            {
                                exportTypes.Add(type);
                                foundExportContract = true;
                                continue;
                            }

                            // select the contract name if it is of type ILayoutItem.
                            type = GetTypeFromContractNameAsILayoutItem(att);
                            if (LayoutBaseType.IsAssignableFrom(type))
                            {
                                exportTypes.Add(type);
                                foundExportContract = true;
                            }
                        }
                        // select the viewmodel type if it is of type ILayoutItem.
                        if (!foundExportContract && LayoutBaseType.IsAssignableFrom(itemType))
                            exportTypes.Add(itemType);

                        // throw exceptions here, instead of failing silently. These are design time errors.
                        var firstExport = exportTypes.FirstOrDefault();
                        if (firstExport == null)
                            throw new InvalidOperationException(string.Format(
                                "A ViewModel that participates in LayoutItem.ShouldReopenOnStart must be decorated with an ExportAttribute who's ContractType that inherits from ILayoutItem, infringing type is {0}.", itemType));
                        if (exportTypes.Count > 1)
                            throw new InvalidOperationException(string.Format(
                                "A ViewModel that participates in LayoutItem.ShouldReopenOnStart can't be decorated with more than one ExportAttribute which inherits from ILayoutItem. infringing type is {0}.", itemType));

                        var selectedTypeName = firstExport.AssemblyQualifiedName;

                        if (string.IsNullOrEmpty(selectedTypeName))
                            throw new InvalidOperationException(string.Format(
                                "Could not retrieve the assembly qualified type name for {0}, most likely because the type is generic.", firstExport));
                        // TODO: it is possible to save generic types. It requires that every generic parameter is saved, along with its position in the generic tree... A lot of work.

                        writer.Write(selectedTypeName);
                        writer.Write(item.ContentId);

                        // Here's the tricky part. Because some items might fail to save their state, or they might be removed (a plug-in assembly deleted and etc.)
                        // we need to save the item's state size to be able to skip the data during deserialization.
                        // Save current stream position. We'll need it later.
                        long stateSizePosition = writer.BaseStream.Position;

                        // Reserve some space for item state size
                        writer.Write(0L);

                        long stateSize;

                        try
                        {
                            long stateStartPosition = writer.BaseStream.Position;
                            item.SaveState(writer);
                            stateSize = writer.BaseStream.Position - stateStartPosition;
                        }
                        catch
                        {
                            stateSize = 0;
                        }

                        // Go back to the position before item's state and write the actual value.
                        writer.BaseStream.Seek(stateSizePosition, SeekOrigin.Begin);
                        writer.Write(stateSize);

                        if (stateSize > 0)
                        {
                            // Got to the end of the stream
                            writer.BaseStream.Seek(0, SeekOrigin.End);
                        }

                        itemCount++;
                    }

                    writer.BaseStream.Seek(0, SeekOrigin.Begin);
                    writer.Write(itemCount);
                    writer.BaseStream.Seek(0, SeekOrigin.End);

                    shellView.SaveLayout(writer.BaseStream);
                }
            }
            catch
            {
                return false;
            }

            return true;
        }

        private static Type GetTypeFromContractNameAsILayoutItem(ExportAttribute attribute)
        {
            string typeName;
            if ((typeName = attribute.ContractName) == null)
                return null;

            var type = Type.GetType(typeName);
            return typeof(ILayoutItem).IsAssignableFrom(type) ? type : null;
        }

        public bool LoadState(IShell shell, IShellView shellView, string fileName)
        {
            var layoutItems = new Dictionary<string, ILayoutItem>();

            if (!File.Exists(fileName))
            {
                return false;
            }

            try
            {
                using (var reader = new BinaryReader(new FileStream(fileName, FileMode.Open, FileAccess.Read)))
                {
                    int count = reader.ReadInt32();

                    for (int i = 0; i < count; i++)
                    {
                        string typeName = reader.ReadString();
                        string contentId = reader.ReadString();
                        long stateEndPosition = reader.ReadInt64();
                        stateEndPosition += reader.BaseStream.Position;

                        var contentType = Type.GetType(typeName);
                        bool skipStateData = true;

                        if (contentType != null)
                        {
                            var contentInstance = IoC.GetInstance(contentType, null) as ILayoutItem;

                            if (contentInstance != null)
                            {
                                layoutItems.Add(contentId, contentInstance);

                                try
                                {
                                    contentInstance.LoadState(reader);
                                    skipStateData = false;
                                }
                                catch
                                {
                                    skipStateData = true;
                                }
                            }
                        }

                        // Skip state data block if we couldn't read it.
                        if (skipStateData)
                        {
                            reader.BaseStream.Seek(stateEndPosition, SeekOrigin.Begin);
                        }
                    }

                    shellView.LoadLayout(reader.BaseStream, shell.Tools.Add, d => shell.OpenDocumentAsync(d).Wait(), layoutItems);
                }
            }
            catch
            {
                return false;
            }

            return true;
        }
    }
}
