using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;
using Gemini.Framework;
using Gemini.Framework.Services;
using Gemini.Modules.Shell.Views;

namespace Gemini.Modules.Shell.Services
{
    [Export(typeof(ILayoutItemStatePersister))]
    public class LayoutItemStatePersister : ILayoutItemStatePersister
    {
        public const string BackupFileSuffix = ".bak";
        public const string TemporaryFileSuffix = ".tmp";

        private static readonly Type LayoutBaseType = typeof(ILayoutItem);

        public LayoutItemStateSaveResult SaveState(IShell shell, IShellView shellView, string fileName)
        {
            // Build and durable-flush the complete candidate beside the destination
            // before preserving the prior destination and replacing it atomically.
            var temporaryFileName = fileName + TemporaryFileSuffix;
            var backupFileName = fileName + BackupFileSuffix;
            var details = new List<string>();
            Exception firstItemException = null;

            try
            {
                using (var stream = new FileStream(
                    temporaryFileName,
                    FileMode.Create,
                    FileAccess.Write,
                    FileShare.None))
                using (var writer = new BinaryWriter(stream, Encoding.UTF8, true))
                {
                    var itemStates = shell.Documents.Concat(shell.Tools.Cast<ILayoutItem>());
                    var itemCount = 0;
                    writer.Write(itemCount);

                    foreach (var item in itemStates)
                    {
                        if (!item.ShouldReopenOnStart)
                            continue;

                        var selectedType = GetPersistedType(item);
                        var selectedTypeName = selectedType.AssemblyQualifiedName;
                        if (string.IsNullOrEmpty(selectedTypeName))
                        {
                            throw new InvalidOperationException(string.Format(
                                "Could not retrieve the assembly qualified type name for {0}, most likely because the type is generic.", selectedType));
                        }

                        writer.Write(selectedTypeName);
                        writer.Write(item.ContentId);

                        byte[] payload;
                        try
                        {
                            using (var payloadStream = new MemoryStream())
                            {
                                using (var payloadWriter = new BinaryWriter(
                                    payloadStream,
                                    Encoding.UTF8,
                                    true))
                                {
                                    item.SaveState(payloadWriter);
                                    payloadWriter.Flush();
                                }

                                payload = payloadStream.ToArray();
                            }
                        }
                        catch (Exception exception)
                        {
                            payload = new byte[0];
                            if (firstItemException == null)
                                firstItemException = exception;
                            details.Add(string.Format(
                                "Could not save state for layout item '{0}': {1}",
                                item.ContentId,
                                exception.Message));
                        }

                        writer.Write((long)payload.Length);
                        writer.Write(payload);
                        itemCount++;
                    }

                    writer.BaseStream.Seek(0, SeekOrigin.Begin);
                    writer.Write(itemCount);
                    writer.BaseStream.Seek(0, SeekOrigin.End);
                    shellView.SaveLayout(writer.BaseStream);
                    writer.Flush();
                    stream.Flush(true);
                }

                if (File.Exists(fileName))
                {
                    File.Copy(fileName, backupFileName, true);
                    File.Replace(temporaryFileName, fileName, null, true);
                }
                else
                {
                    File.Move(temporaryFileName, fileName);
                }
            }
            catch (Exception exception)
            {
                var failure = DeleteTemporaryFile(temporaryFileName, exception);
                return LayoutItemStateSaveResult.Failed(
                    failure,
                    "Could not transactionally write layout state to '" + fileName + "'.");
            }

            return details.Count == 0
                ? LayoutItemStateSaveResult.Success()
                : LayoutItemStateSaveResult.Partial(
                    string.Join(Environment.NewLine, details),
                    firstItemException);
        }

        public async Task<LayoutItemStateLoadResult> LoadStateAsync(
            IShell shell,
            IShellView shellView,
            string fileName)
        {
            FileStream stream;
            try
            {
                stream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read);
            }
            catch (FileNotFoundException)
            {
                return LayoutItemStateLoadResult.NotFound(
                    "Layout state file was not found at '" + fileName + "'.");
            }
            catch (DirectoryNotFoundException)
            {
                return LayoutItemStateLoadResult.NotFound(
                    "Layout state directory was not found for '" + fileName + "'.");
            }
            catch (Exception exception)
            {
                return LayoutItemStateLoadResult.Failed(
                    exception,
                    "Could not open layout state file '" + fileName + "'.");
            }

            ParsedLayoutState parsedState;
            try
            {
                using (stream)
                using (var reader = new BinaryReader(stream, Encoding.UTF8, true))
                {
                    parsedState = ParseState(reader, shellView, shell);
                }
            }
            catch (Exception exception)
            {
                return LayoutItemStateLoadResult.Corrupt(
                    exception,
                    "Layout state file '" + fileName + "' could not be parsed; the original file was preserved.");
            }

            var failedItems = new List<ILayoutItem>();
            foreach (var item in parsedState.Items)
            {
                var asyncRestorer = item as IAsyncLayoutItemStateRestorer;
                if (asyncRestorer == null)
                    continue;

                try
                {
                    await asyncRestorer.RestoreStateAsync();
                }
                catch (Exception exception)
                {
                    failedItems.Add(item);
                    parsedState.AddPartialFailure(
                        string.Format(
                            "Could not restore content for layout item '{0}': {1}",
                            item.ContentId,
                            exception.Message),
                        exception);
                }
            }

            var documents = parsedState.Documents
                .Where(x => !ContainsReference(failedItems, x))
                .ToArray();
            var tools = parsedState.Tools
                .Where(x => !ContainsReference(failedItems, x))
                .ToArray();

            foreach (var failedItem in failedItems)
            {
                var failedDocument = failedItem as IDocument;
                if (failedDocument != null)
                    shell.Documents.Remove(failedDocument);

                var failedTool = failedItem as ITool;
                if (failedTool != null)
                    shell.Tools.Remove(failedTool);
            }

            var selectedItem = ContainsReference(failedItems, parsedState.SelectedItem)
                ? null
                : parsedState.SelectedItem;
            var restorePlan = new LayoutItemStateRestorePlan(
                documents,
                tools.Where(x => x.IsVisible),
                selectedItem);

            return parsedState.HasPartialFailures
                ? LayoutItemStateLoadResult.Partial(
                    restorePlan,
                    string.Join(Environment.NewLine, parsedState.PartialFailureDetails),
                    parsedState.FirstPartialFailure)
                : LayoutItemStateLoadResult.Success(restorePlan);
        }

        private static ParsedLayoutState ParseState(
            BinaryReader reader,
            IShellView shellView,
            IShell shell)
        {
            var parsedState = new ParsedLayoutState();
            var layoutItems = new Dictionary<string, ILayoutItem>();
            var count = reader.ReadInt32();
            if (count < 0)
                throw new InvalidDataException("The persisted layout item count cannot be negative.");

            for (var i = 0; i < count; i++)
            {
                var typeName = reader.ReadString();
                var contentId = reader.ReadString();
                var payloadLength = reader.ReadInt64();
                if (payloadLength < 0 || payloadLength > reader.BaseStream.Length - reader.BaseStream.Position)
                {
                    throw new InvalidDataException(string.Format(
                        "The persisted payload length for layout item '{0}' is invalid.",
                        contentId));
                }

                var stateEndPosition = reader.BaseStream.Position + payloadLength;
                ILayoutItem contentInstance = null;
                try
                {
                    var contentType = Type.GetType(typeName, false);
                    if (contentType == null)
                    {
                        parsedState.AddPartialFailure(string.Format(
                            "The persisted layout item type '{0}' for '{1}' is unavailable.",
                            typeName,
                            contentId));
                    }
                    else
                    {
                        contentInstance = IoC.GetInstance(contentType, null) as ILayoutItem;
                        if (contentInstance == null)
                        {
                            parsedState.AddPartialFailure(string.Format(
                                "The persisted layout item type '{0}' for '{1}' could not be created.",
                                typeName,
                                contentId));
                        }
                    }
                }
                catch (Exception exception)
                {
                    parsedState.AddPartialFailure(
                        string.Format(
                            "The persisted layout item '{0}' could not be created: {1}",
                            contentId,
                            exception.Message),
                        exception);
                }

                var itemLoaded = false;
                if (contentInstance != null)
                {
                    try
                    {
                        contentInstance.LoadState(reader);
                        if (reader.BaseStream.Position > stateEndPosition)
                        {
                            throw new InvalidDataException(string.Format(
                                "Layout item '{0}' read beyond its persisted payload.",
                                contentId));
                        }

                        itemLoaded = true;
                    }
                    catch (Exception exception)
                    {
                        parsedState.AddPartialFailure(
                            string.Format(
                                "Could not load persisted payload for layout item '{0}': {1}",
                                contentId,
                                exception.Message),
                            exception);
                    }
                }

                reader.BaseStream.Seek(stateEndPosition, SeekOrigin.Begin);
                if (!itemLoaded)
                    continue;

                if (layoutItems.ContainsKey(contentId))
                {
                    parsedState.AddPartialFailure(
                        "The persisted layout contains duplicate content ID '" + contentId + "'.");
                    continue;
                }

                layoutItems.Add(contentId, contentInstance);
                parsedState.Items.Add(contentInstance);
            }

            // Register the already-created instances synchronously so ItemsSource binds
            // AvalonDock's deserialized nodes. Async content and activation run afterward.
            shellView.LoadLayout(
                reader.BaseStream,
                tool =>
                {
                    shell.RegisterTool(tool);
                    parsedState.AddTool(tool);
                },
                document =>
                {
                    if (!ContainsReference(shell.Documents, document))
                        shell.Documents.Add(document);
                    parsedState.AddDocument(document);
                },
                layoutItems);

            return parsedState;
        }

        private static Type GetPersistedType(ILayoutItem item)
        {
            var itemType = item.GetType();
            var exportAttributes = itemType
                .GetCustomAttributes(typeof(ExportAttribute), false)
                .Cast<ExportAttribute>()
                .ToList();
            var exportTypes = new List<Type>();
            var foundExportContract = false;

            foreach (var attribute in exportAttributes)
            {
                var type = attribute.ContractType;
                if (LayoutBaseType.IsAssignableFrom(type))
                {
                    exportTypes.Add(type);
                    foundExportContract = true;
                    continue;
                }

                type = GetTypeFromContractNameAsILayoutItem(attribute);
                if (LayoutBaseType.IsAssignableFrom(type))
                {
                    exportTypes.Add(type);
                    foundExportContract = true;
                }
            }

            if (!foundExportContract && LayoutBaseType.IsAssignableFrom(itemType))
                exportTypes.Add(itemType);

            var firstExport = exportTypes.FirstOrDefault();
            if (firstExport == null)
            {
                throw new InvalidOperationException(string.Format(
                    "A ViewModel that participates in LayoutItem.ShouldReopenOnStart must be decorated with an ExportAttribute whose ContractType inherits from ILayoutItem; infringing type is {0}.",
                    itemType));
            }

            if (exportTypes.Count > 1)
            {
                throw new InvalidOperationException(string.Format(
                    "A ViewModel that participates in LayoutItem.ShouldReopenOnStart cannot be decorated with more than one ExportAttribute which inherits from ILayoutItem; infringing type is {0}.",
                    itemType));
            }

            return firstExport;
        }

        private static Type GetTypeFromContractNameAsILayoutItem(ExportAttribute attribute)
        {
            var typeName = attribute.ContractName;
            if (typeName == null)
                return null;

            var type = Type.GetType(typeName);
            return type != null && typeof(ILayoutItem).IsAssignableFrom(type) ? type : null;
        }

        private static Exception DeleteTemporaryFile(string temporaryFileName, Exception failure)
        {
            try
            {
                if (File.Exists(temporaryFileName))
                    File.Delete(temporaryFileName);
                return failure;
            }
            catch (Exception cleanupException)
            {
                return new AggregateException(failure, cleanupException);
            }
        }

        private static bool ContainsReference<T>(IEnumerable<T> items, T candidate)
            where T : class
        {
            if (candidate == null)
                return false;

            return items.Any(x => ReferenceEquals(x, candidate));
        }

        private sealed class ParsedLayoutState
        {
            public ParsedLayoutState()
            {
                Items = new List<ILayoutItem>();
                Documents = new List<IDocument>();
                Tools = new List<ITool>();
                PartialFailureDetails = new List<string>();
            }

            public List<ILayoutItem> Items { get; }

            public List<IDocument> Documents { get; }

            public List<ITool> Tools { get; }

            public List<string> PartialFailureDetails { get; }

            public Exception FirstPartialFailure { get; private set; }

            public ILayoutItem SelectedItem { get; private set; }

            public bool HasPartialFailures => PartialFailureDetails.Count != 0;

            public void AddDocument(IDocument document)
            {
                if (!ContainsReference(Documents, document))
                    Documents.Add(document);
                if (document.IsSelected)
                    SelectedItem = document;
            }

            public void AddTool(ITool tool)
            {
                if (!ContainsReference(Tools, tool))
                    Tools.Add(tool);
                if (tool.IsSelected)
                    SelectedItem = tool;
            }

            public void AddPartialFailure(string details, Exception exception = null)
            {
                PartialFailureDetails.Add(details);
                if (FirstPartialFailure == null && exception != null)
                    FirstPartialFailure = exception;
            }
        }
    }
}
