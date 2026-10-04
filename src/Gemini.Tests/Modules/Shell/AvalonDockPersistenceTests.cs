using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Xml;
using AvalonDock;
using AvalonDock.Layout;
using AvalonDock.Serializer.Xml;
using Caliburn.Micro;
using Gemini.Framework;
using Gemini.Framework.Services;
using Gemini.Modules.Shell.Services;
using Gemini.Modules.Shell.Views;
using Gemini.Tests.TestInfrastructure;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Gemini.Tests.Modules.Shell
{
    [STATestClass]
    [DoNotParallelize]
    public sealed class AvalonDockPersistenceTests
    {
        private const string Complete460FixtureHash =
            "7697172D53DC8F9ADFDE8DF4ADC6F52AE4BA323A7E45DC336ADCFB7AFC2CCA13";

        private const string Complete4741StateFixtureHash =
            "58FF3ED46BD9E545C15D8A9B81CD9475CC404B3F690D7EE326FD80C40DBBB7D4";

        private const string Complete501StateFixtureHash =
            "3218BBB44DC60EB8AFEA2C6BD8E1B768CD6DEF54F246F5C964E2DCFCED6D527F";

        private static readonly string[] FixtureDocumentIds =
        {
            "fixture-document-primary",
            "fixture-document-secondary"
        };

        private static readonly string[] FixtureToolIds =
        {
            "fixture-tool-left",
            "fixture-tool-right",
            "fixture-tool-bottom",
            "fixture-tool-floating",
            "fixture-tool-hidden"
        };

        // Intent: make this compatibility input immutable; later writers require new fixtures.
        [TestMethod]
        public void Complete460Fixture_NormalizedBytesMatchCapturedImmutableBaseline()
        {
            var normalizedFixture = File.ReadAllText(Get460FixturePath())
                .Replace("\r\n", "\n");

            string actualHash;
            using (var algorithm = SHA256.Create())
            {
                actualHash = BitConverter.ToString(
                        algorithm.ComputeHash(Encoding.UTF8.GetBytes(normalizedFixture)))
                    .Replace("-", string.Empty);
            }

            Assert.AreEqual(Complete460FixtureHash, actualHash);
            Assert.Contains(
                "fixture-tool-floating",
                normalizedFixture);
            Assert.Contains(
                "<Hidden>",
                normalizedFixture);
        }

        // Intent: make the whole pre-v5 Gemini envelope an immutable compatibility input.
        [TestMethod]
        public void Complete4741StateFixture_BytesMatchCapturedImmutableBaseline()
        {
            AssertCompleteStateFixtureHash(
                Get4741StateFixturePath(),
                Complete4741StateFixtureHash);
        }

        // Intent: freeze v5 writer output separately instead of overwriting rollback evidence.
        [TestMethod]
        public void Complete501StateFixture_BytesMatchCapturedImmutableWriterOutput()
        {
            AssertCompleteStateFixtureHash(
                Get501StateFixturePath(),
                Complete501StateFixtureHash);
        }

        // Intent: prove a complete pre-v5 state restores payloads and layout semantics together.
        [TestMethod]
        public async Task Complete4741StateFixture_LoadStateAsync_RestoresWholeState()
        {
            await AssertCompleteStateFixtureRestoresWholeState(
                Get4741StateFixturePath());
        }

        // Intent: prove v5 writer output is a complete, readable Gemini state envelope.
        [TestMethod]
        public async Task Complete501StateFixture_LoadStateAsync_RestoresWholeState()
        {
            await AssertCompleteStateFixtureRestoresWholeState(
                Get501StateFixturePath());
        }

        // Intent: prove legacy ContentIds still reconnect every document and tool model.
        [TestMethod]
        public void Complete460Fixture_Deserialize_RestoresDocumentsAndToolsByContentId()
        {
            var loaded = LoadFixture();
            var documents = loaded.Manager.Layout.Descendents()
                .OfType<LayoutDocument>()
                .ToArray();
            var tools = loaded.Manager.Layout.Descendents()
                .OfType<LayoutAnchorable>()
                .ToArray();

            CollectionAssert.AreEquivalent(
                FixtureDocumentIds,
                documents.Select(x => x.ContentId).ToArray());
            CollectionAssert.AreEquivalent(
                FixtureToolIds,
                tools.Select(x => x.ContentId).ToArray());
            CollectionAssert.AreEquivalent(
                FixtureDocumentIds.Concat(FixtureToolIds).ToArray(),
                loaded.CallbackContentIds.ToArray());
            foreach (var content in documents.Cast<LayoutContent>().Concat(tools))
            {
                Assert.AreEqual(
                    "restored:" + content.ContentId,
                    content.Content);
            }
        }

        // Intent: protect user-visible legacy layout semantics beyond successful XML parsing.
        [TestMethod]
        public void Complete460Fixture_Deserialize_PreservesVisibilitySelectionAndTopology()
        {
            var loaded = LoadFixture();
            var root = loaded.Manager.Layout;
            var vertical = Assert.IsInstanceOfType<LayoutPanel>(root.RootPanel);
            var horizontal = Assert.IsInstanceOfType<LayoutPanel>(vertical.Children[0]);
            var left = Assert.IsInstanceOfType<LayoutAnchorablePane>(
                horizontal.Children[0]);
            var documents = Assert.IsInstanceOfType<LayoutDocumentPane>(
                horizontal.Children[1]);
            var right = Assert.IsInstanceOfType<LayoutAnchorablePane>(
                horizontal.Children[2]);
            var bottom = Assert.IsInstanceOfType<LayoutAnchorablePane>(
                vertical.Children[1]);
            var hidden = Assert.ContainsSingle(root.Hidden);
            var floatingWindow = Assert.ContainsSingle(root.FloatingWindows);
            var floatingPane = Assert.IsInstanceOfType<LayoutAnchorablePane>(
                floatingWindow.Descendents().Single(x => x is LayoutAnchorablePane));
            var floating = Assert.ContainsSingle(floatingPane.Children);

            Assert.AreEqual(Orientation.Vertical, vertical.Orientation);
            Assert.AreEqual(2, vertical.ChildrenCount);
            Assert.AreEqual(Orientation.Horizontal, horizontal.Orientation);
            Assert.AreEqual(3, horizontal.ChildrenCount);
            Assert.AreEqual("LeftPane", left.Name);
            Assert.AreEqual(240d, left.DockWidth.Value);
            Assert.AreEqual(GridUnitType.Pixel, left.DockWidth.GridUnitType);
            Assert.AreEqual("RightPane", right.Name);
            Assert.AreEqual(310d, right.DockWidth.Value);
            Assert.AreEqual(GridUnitType.Pixel, right.DockWidth.GridUnitType);
            Assert.AreEqual("BottomPane", bottom.Name);
            Assert.AreEqual(180d, bottom.DockHeight.Value);
            Assert.AreEqual(GridUnitType.Pixel, bottom.DockHeight.GridUnitType);
            CollectionAssert.AreEqual(
                FixtureDocumentIds,
                documents.Children.Select(x => x.ContentId).ToArray());
            Assert.AreEqual(
                "fixture-document-secondary",
                documents.SelectedContent.ContentId);
            Assert.IsTrue(documents.Children[1].IsSelected);
            Assert.IsTrue(documents.Children[1].IsLastFocusedDocument);
            Assert.AreEqual("fixture-tool-hidden", hidden.ContentId);
            Assert.IsFalse(hidden.IsVisible);
            Assert.AreEqual("FloatingPane", floatingPane.Name);
            Assert.AreEqual("fixture-tool-floating", floating.ContentId);
            Assert.IsTrue(floating.IsFloating);
            Assert.AreEqual(120d, floating.FloatingLeft);
            Assert.AreEqual(80d, floating.FloatingTop);
            Assert.AreEqual(420d, floating.FloatingWidth);
            Assert.AreEqual(260d, floating.FloatingHeight);
        }

        // Intent: verify Gemini's ShellView callbacks restore model identity at the real boundary.
        [TestMethod]
        public void ShellViewLayoutBoundary_SerializeDeserialize_RestoresDocumentsAndToolsByContentId()
        {
            var sourceItems = RuntimeItems.CreateSource();
            var sourceView = CreateCompleteRuntimeView(sourceItems);
            var layoutBytes = SaveLayout(sourceView);
            var restored = RestoreLayout(layoutBytes, sourceItems);

            Assert.AreEqual(2, restored.DocumentCallbacks.Count);
            Assert.AreEqual(6, restored.ToolCallbacks.Count);
            CollectionAssert.AreEquivalent(
                restored.Items.Documents.Cast<IDocument>().ToArray(),
                restored.DocumentCallbacks.ToArray());
            CollectionAssert.AreEquivalent(
                restored.Items.Tools.Cast<ITool>().ToArray(),
                restored.ToolCallbacks.ToArray());

            foreach (var pair in restored.Items.SerializedContentMap)
            {
                var model = restored.Manager.Layout.Descendents()
                    .OfType<LayoutContent>()
                    .Single(x => ReferenceEquals(x.Content, pair.Value));
                Assert.AreSame(pair.Value, model.Content);
                Assert.AreEqual(pair.Value.ContentId, model.ContentId);
            }
        }

        // Intent: preserve visibility, selection, dimensions, and topology across the ShellView API.
        [TestMethod]
        public void ShellViewLayoutBoundary_Deserialize_PreservesVisibilitySelectionAndLayoutSemantics()
        {
            var sourceItems = RuntimeItems.CreateSource();
            var sourceView = CreateCompleteRuntimeView(sourceItems);
            var restored = RestoreLayout(SaveLayout(sourceView), sourceItems);
            var root = restored.Manager.Layout;
            var horizontal = root.Descendents()
                .OfType<LayoutPanel>()
                .Single(x => x.Orientation == Orientation.Horizontal);
            var documentPane = root.Descendents()
                .OfType<LayoutDocumentPane>()
                .Single();
            var leftPane = root.Descendents()
                .OfType<LayoutAnchorablePane>()
                .Single(x => x.Name == "LeftPane");
            var hidden = Assert.ContainsSingle(root.Hidden);

            Assert.AreEqual(3, horizontal.ChildrenCount);
            Assert.AreEqual(2, documentPane.ChildrenCount);
            Assert.AreEqual(2, leftPane.ChildrenCount);
            Assert.AreEqual(
                restored.Items.SecondaryDocument.ContentId,
                documentPane.SelectedContent.ContentId);
            Assert.AreEqual(
                restored.Items.SecondaryLeftTool.ContentId,
                leftPane.SelectedContent.ContentId);
            Assert.IsFalse(restored.Items.PrimaryDocument.IsSelected);
            Assert.IsTrue(restored.Items.SecondaryDocument.IsSelected);
            Assert.IsFalse(restored.Items.PrimaryLeftTool.IsSelected);
            Assert.IsTrue(restored.Items.SecondaryLeftTool.IsSelected);
            Assert.IsFalse(restored.Items.HiddenTool.IsVisible);
            Assert.AreSame(
                restored.Items.HiddenTool,
                hidden.Content);
            Assert.AreEqual(1, root.FloatingWindows.Count);
            var floating = root.FloatingWindows[0].Descendents()
                .OfType<LayoutAnchorable>()
                .Single();
            Assert.AreSame(restored.Items.FloatingTool, floating.Content);
            Assert.IsTrue(floating.IsFloating);
            Assert.AreEqual(420d, floating.FloatingWidth);
        }

        // Intent: ensure a restored layout remains editable and serializable after migration.
        [TestMethod]
        public void RestoredLayout_AddAndSelectDocumentThenSerialize_RemainsOperational()
        {
            var sourceItems = RuntimeItems.CreateSource();
            var restored = RestoreLayout(
                SaveLayout(CreateCompleteRuntimeView(sourceItems)),
                sourceItems);
            var documentPane = restored.Manager.Layout.Descendents()
                .OfType<LayoutDocumentPane>()
                .Single();
            var addedDocument = new PersistentTestDocument(
                "Post-restore document",
                "post-restore-state");
            var addedModel = CreateDocument(addedDocument);

            documentPane.Children.Add(addedModel);
            addedModel.IsSelected = true;
            addedModel.IsActive = true;
            var serializedAgain = SaveLayout(restored.View);
            var serializedXml = Encoding.UTF8.GetString(serializedAgain);

            Assert.AreEqual(3, documentPane.ChildrenCount);
            Assert.AreSame(addedModel, documentPane.SelectedContent);
            Assert.AreSame(addedModel, restored.Manager.Layout.ActiveContent);
            Assert.Contains(addedDocument.ContentId, serializedXml);
            Assert.Contains("Post-restore document", serializedXml);
            Assert.Contains(
                restored.Items.PrimaryDocument.ContentId,
                serializedXml);
            Assert.Contains(
                restored.Items.SecondaryDocument.ContentId,
                serializedXml);
        }

        // Intent: catch semantic drift or ghost entries that appear only after repeated saves.
        [TestMethod]
        public void CompleteLayout_RepeatedRoundTrips_PreserveSemanticSnapshot()
        {
            var sourceItems = RuntimeItems.CreateSource();
            var sourceView = CreateCompleteRuntimeView(sourceItems);
            var expectedSnapshot = CreateSemanticSnapshot(GetManager(sourceView).Layout);

            var firstRestore = RestoreLayout(SaveLayout(sourceView), sourceItems);
            var firstSnapshot = CreateSemanticSnapshot(firstRestore.Manager.Layout);
            var secondRestore = RestoreLayout(
                SaveLayout(firstRestore.View),
                firstRestore.Items);
            var secondSnapshot = CreateSemanticSnapshot(secondRestore.Manager.Layout);

            Assert.AreEqual(expectedSnapshot, firstSnapshot);
            Assert.AreEqual(expectedSnapshot, secondSnapshot);
            Assert.AreEqual(2, secondRestore.DocumentCallbacks.Count);
            Assert.AreEqual(6, secondRestore.ToolCallbacks.Count);
        }

        // Intent: tolerate missing extension content without discarding models that still resolve.
        [TestMethod]
        public void LoadLayout_UnresolvedContentId_CancelsMissingModelAndRestoresKnownContent()
        {
            var sourceView = new ShellView();
            var sourceManager = GetManager(sourceView);
            var knownSource = new PersistentTestDocument("Known", "known");
            var missingSource = new PersistentTestDocument("Missing", "missing");
            var pane = new LayoutDocumentPane();
            pane.Children.Add(CreateDocument(knownSource));
            pane.Children.Add(CreateDocument(missingSource));
            sourceManager.Layout = new LayoutRoot
            {
                RootPanel = new LayoutPanel(pane)
            };
            var bytes = SaveLayout(sourceView);
            var restoredKnown = new PersistentTestDocument("Restored known", "restored");
            var targetView = new ShellView();
            var callbacks = new List<IDocument>();

            using (var stream = new MemoryStream(bytes, false))
            {
                targetView.LoadLayout(
                    stream,
                    tool => Assert.Fail("No tool callback was expected."),
                    callbacks.Add,
                    new Dictionary<string, ILayoutItem>
                    {
                        { knownSource.ContentId, restoredKnown }
                    });
            }

            var targetManager = GetManager(targetView);
            var restoredPane = targetManager.Layout.Descendents()
                .OfType<LayoutDocumentPane>()
                .Single();
            var restoredModel = Assert.ContainsSingle(restoredPane.Children);
            Assert.AreEqual(restoredKnown.ContentId, restoredModel.ContentId);
            Assert.AreSame(restoredKnown, restoredModel.Content);
            Assert.AreEqual(1, callbacks.Count);
            Assert.AreSame(restoredKnown, callbacks[0]);
            Assert.DoesNotContain(
                missingSource.ContentId,
                targetManager.Layout.Descendents()
                    .OfType<LayoutContent>()
                    .Select(x => x.ContentId)
                    .ToArray());
        }

        // Intent: reject corrupt layouts before callbacks or partial replacement expose bad state.
        [TestMethod]
        public void LoadLayout_MalformedXml_ThrowsInvalidOperationWithXmlCauseAndNoCallbacks()
        {
            var view = new ShellView();
            var manager = GetManager(view);
            var originalLayout = manager.Layout;
            var documentCallbacks = 0;
            var toolCallbacks = 0;
            var malformedXml = Encoding.UTF8.GetBytes(
                "<?xml version=\"1.0\"?><LayoutRoot><RootPanel>");

            using (var stream = new MemoryStream(malformedXml, false))
            {
                var exception = Assert.ThrowsExactly<InvalidOperationException>(
                    () => view.LoadLayout(
                    stream,
                    tool => toolCallbacks++,
                    document => documentCallbacks++,
                    new Dictionary<string, ILayoutItem>()));
                Assert.IsInstanceOfType<XmlException>(
                    exception.GetBaseException());
            }

            Assert.AreEqual(0, documentCallbacks);
            Assert.AreEqual(0, toolCallbacks);
            Assert.AreSame(originalLayout, manager.Layout);
        }

        // Intent: exercise the complete Gemini envelope because AvalonDock XML alone is not state.
        [TestMethod]
        public async Task WholeFileBoundary_RealLayoutPayload_RoundTripsWithoutUsingApplicationState()
        {
            using (var directory = new TestDirectory())
            {
                var statePath = directory.GetPath("avalondock-460-state.bin");
                var sourceItems = RuntimeItems.CreateSource();
                var sourceView = CreateCompleteRuntimeView(sourceItems);
                var sourceShell = new TestShell(
                    sourceItems.Documents,
                    sourceItems.Tools);
                var persister = new LayoutItemStatePersister();

                var saveResult = persister.SaveState(
                    sourceShell,
                    sourceView,
                    statePath);

                Assert.AreEqual(LayoutItemStateSaveStatus.Success, saveResult.Status);
                Assert.IsTrue(File.Exists(statePath));
                using (var reader = new BinaryReader(File.OpenRead(statePath)))
                    Assert.AreEqual(8, reader.ReadInt32());

                var restoredItems = RuntimeItems.CreateForRestore(sourceItems);
                var documentQueue = new Queue<PersistentTestDocument>(
                    restoredItems.Documents);
                var toolQueue = new Queue<PersistentTestTool>(
                    restoredItems.Tools);
                var restoredShell = new TestShell();
                var restoredView = new ShellView();
                LayoutItemStateLoadResult loadResult;

                using (new IoCOverrideScope(
                    (type, key) =>
                    {
                        if (type == typeof(PersistentTestDocument))
                            return documentQueue.Dequeue();
                        if (type == typeof(PersistentTestTool))
                            return toolQueue.Dequeue();
                        return null;
                    },
                    type => Enumerable.Empty<object>(),
                    instance => { }))
                {
                    loadResult = await persister.LoadStateAsync(
                        restoredShell,
                        restoredView,
                        statePath);
                }

                Assert.AreEqual(LayoutItemStateLoadStatus.Success, loadResult.Status);
                Assert.AreEqual(2, loadResult.RestorePlan.Documents.Count);
                Assert.AreEqual(5, loadResult.RestorePlan.VisibleTools.Count);
                Assert.AreSame(
                    restoredItems.HiddenTool,
                    loadResult.RestorePlan.SelectedItem);
                Assert.AreEqual(2, restoredShell.Documents.Count);
                Assert.AreEqual(6, restoredShell.Tools.Count);
                Assert.AreEqual(
                    sourceItems.PrimaryDocument.StateMarker,
                    restoredItems.PrimaryDocument.StateMarker);
                Assert.AreEqual(
                    sourceItems.FloatingTool.StateMarker,
                    restoredItems.FloatingTool.StateMarker);
                Assert.IsFalse(restoredItems.HiddenTool.IsVisible);
                Assert.AreNotEqual(
                    Path.GetFullPath("ApplicationState.bin"),
                    Path.GetFullPath(statePath));
            }
        }

        private static FixtureLoadResult LoadFixture()
        {
            _ = Application.ResourceAssembly;
            var manager = new DockingManager();
            var callbackIds = new List<string>();
            var serializer = new XmlLayoutSerializer(manager);
            serializer.LayoutSerializationCallback += (sender, args) =>
            {
                callbackIds.Add(args.Model.ContentId);
                args.Content = "restored:" + args.Model.ContentId;
            };
            using (var stream = new FileStream(
                Get460FixturePath(),
                FileMode.Open,
                FileAccess.Read,
                FileShare.Read))
            {
                serializer.Deserialize(stream);
            }

            return new FixtureLoadResult(manager, callbackIds);
        }

        private static string Get460FixturePath()
        {
            return Path.Combine(
                AppContext.BaseDirectory,
                "TestData",
                "AvalonDock460",
                "CompleteLayout.xml");
        }

        private static string Get4741StateFixturePath()
        {
            return Path.Combine(
                AppContext.BaseDirectory,
                "TestData",
                "AvalonDock4741",
                "ApplicationState.bin");
        }

        private static string Get501StateFixturePath()
        {
            return Path.Combine(
                AppContext.BaseDirectory,
                "TestData",
                "AvalonDock501",
                "ApplicationState.bin");
        }

        private static void AssertCompleteStateFixtureHash(
            string fixturePath,
            string expectedHash)
        {
            byte[] fixture = File.ReadAllBytes(fixturePath);
            string actualHash;
            using (var algorithm = SHA256.Create())
            {
                actualHash = BitConverter.ToString(algorithm.ComputeHash(fixture))
                    .Replace("-", string.Empty);
            }

            Assert.AreEqual(expectedHash, actualHash);
            using (var reader = new BinaryReader(new MemoryStream(fixture, false)))
            {
                Assert.AreEqual(8, reader.ReadInt32());
            }
        }

        private static async Task AssertCompleteStateFixtureRestoresWholeState(
            string fixturePath)
        {
            var expectedView = CreateCompleteRuntimeView(RuntimeItems.CreateSource());
            var expectedSnapshot = CreateSemanticSnapshot(GetManager(expectedView).Layout);
            var restoredItems = RuntimeItems.CreateSource();
            var documentQueue = new Queue<PersistentTestDocument>(
                restoredItems.Documents);
            var toolQueue = new Queue<PersistentTestTool>(
                restoredItems.Tools);
            var shell = new TestShell();
            var view = new ShellView();
            LayoutItemStateLoadResult result;

            using (new IoCOverrideScope(
                (type, key) =>
                {
                    if (type == typeof(PersistentTestDocument))
                        return documentQueue.Dequeue();
                    if (type == typeof(PersistentTestTool))
                        return toolQueue.Dequeue();
                    return null;
                },
                type => Enumerable.Empty<object>(),
                instance => { }))
            {
                result = await new LayoutItemStatePersister().LoadStateAsync(
                    shell,
                    view,
                    fixturePath);
            }

            Assert.AreEqual(LayoutItemStateLoadStatus.Success, result.Status);
            Assert.AreEqual(2, result.RestorePlan.Documents.Count);
            Assert.AreEqual(5, result.RestorePlan.VisibleTools.Count);
            Assert.AreEqual(2, shell.Documents.Count);
            Assert.AreEqual(6, shell.Tools.Count);
            Assert.AreSame(restoredItems.HiddenTool, result.RestorePlan.SelectedItem);
            Assert.IsFalse(restoredItems.HiddenTool.IsVisible);
            Assert.AreEqual("document-primary", restoredItems.PrimaryDocument.StateMarker);
            Assert.AreEqual("document-secondary", restoredItems.SecondaryDocument.StateMarker);
            Assert.AreEqual("tool-floating", restoredItems.FloatingTool.StateMarker);
            Assert.AreEqual("tool-hidden", restoredItems.HiddenTool.StateMarker);
            Assert.AreEqual(
                expectedSnapshot,
                CreateSemanticSnapshot(GetManager(view).Layout));
        }

        private static ShellView CreateCompleteRuntimeView(RuntimeItems items)
        {
            _ = Application.ResourceAssembly;
            var view = new ShellView();
            var manager = GetManager(view);
            var primaryDocument = CreateDocument(items.PrimaryDocument);
            var secondaryDocument = CreateDocument(items.SecondaryDocument);
            var documents = new LayoutDocumentPane();
            documents.Children.Add(primaryDocument);
            documents.Children.Add(secondaryDocument);

            var primaryLeft = CreateAnchorable(items.PrimaryLeftTool);
            var secondaryLeft = CreateAnchorable(items.SecondaryLeftTool);
            var left = new LayoutAnchorablePane(primaryLeft)
            {
                Name = "LeftPane",
                DockWidth = new GridLength(240, GridUnitType.Pixel)
            };
            left.Children.Add(secondaryLeft);

            var rightModel = CreateAnchorable(items.RightTool);
            var right = new LayoutAnchorablePane(rightModel)
            {
                Name = "RightPane",
                DockWidth = new GridLength(310, GridUnitType.Pixel)
            };
            var bottomModel = CreateAnchorable(items.BottomTool);
            var bottom = new LayoutAnchorablePane(bottomModel)
            {
                Name = "BottomPane",
                DockHeight = new GridLength(180, GridUnitType.Pixel)
            };
            var horizontal = new LayoutPanel
            {
                Orientation = Orientation.Horizontal
            };
            horizontal.Children.Add(left);
            horizontal.Children.Add(documents);
            horizontal.Children.Add(right);
            var vertical = new LayoutPanel
            {
                Orientation = Orientation.Vertical
            };
            vertical.Children.Add(horizontal);
            vertical.Children.Add(bottom);
            var root = new LayoutRoot
            {
                RootPanel = vertical
            };

            var floatingModel = CreateAnchorable(items.FloatingTool);
            floatingModel.FloatingLeft = 120;
            floatingModel.FloatingTop = 80;
            floatingModel.FloatingWidth = 420;
            floatingModel.FloatingHeight = 260;
            var floatingPane = new LayoutAnchorablePane(floatingModel)
            {
                Name = "FloatingPane"
            };
            root.FloatingWindows.Add(new LayoutAnchorableFloatingWindow
            {
                RootPanel = new LayoutAnchorablePaneGroup(floatingPane)
            });

            var hiddenModel = CreateAnchorable(items.HiddenTool);
            root.Hidden.Add(hiddenModel);
            items.HiddenTool.IsVisible = false;

            manager.Layout = root;
            primaryDocument.IsSelected = false;
            secondaryDocument.IsSelected = true;
            primaryLeft.IsSelected = false;
            secondaryLeft.IsSelected = true;
            rightModel.IsSelected = true;
            bottomModel.IsSelected = true;
            floatingModel.IsSelected = true;
            hiddenModel.IsSelected = true;
            secondaryDocument.IsActive = true;
            return view;
        }

        private static RestoredRuntime RestoreLayout(
            byte[] bytes,
            RuntimeItems sourceItems)
        {
            var restoredItems = RuntimeItems.CreateForRestore(sourceItems);
            var view = new ShellView();
            var documentCallbacks = new List<IDocument>();
            var toolCallbacks = new List<ITool>();
            using (var stream = new MemoryStream(bytes, false))
            {
                view.LoadLayout(
                    stream,
                    toolCallbacks.Add,
                    documentCallbacks.Add,
                    restoredItems.SerializedContentMap);
            }

            return new RestoredRuntime(
                view,
                GetManager(view),
                restoredItems,
                documentCallbacks,
                toolCallbacks);
        }

        private static byte[] SaveLayout(ShellView view)
        {
            using (var stream = new MemoryStream())
            {
                view.SaveLayout(stream);
                return stream.ToArray();
            }
        }

        private static DockingManager GetManager(ShellView view)
        {
            return Assert.IsInstanceOfType<DockingManager>(view.FindName("Manager"));
        }

        private static LayoutDocument CreateDocument(PersistentTestDocument document)
        {
            return new LayoutDocument
            {
                Content = document,
                ContentId = document.ContentId,
                Title = document.DisplayName
            };
        }

        private static LayoutAnchorable CreateAnchorable(PersistentTestTool tool)
        {
            return new LayoutAnchorable
            {
                Content = tool,
                ContentId = tool.ContentId,
                Title = tool.DisplayName
            };
        }

        private static string CreateSemanticSnapshot(LayoutRoot root)
        {
            var builder = new StringBuilder();
            AppendElement(builder, root.RootPanel);
            builder.Append("|hidden:");
            foreach (var item in root.Hidden.OrderBy(x => x.ContentId))
                AppendContent(builder, item);
            builder.Append("|floating:");
            foreach (var window in root.FloatingWindows)
            {
                AppendElement(
                    builder,
                    Assert.IsInstanceOfType<LayoutAnchorableFloatingWindow>(
                        window).RootPanel);
            }
            return builder.ToString();
        }

        private static void AppendElement(
            StringBuilder builder,
            ILayoutElement element)
        {
            var panel = element as LayoutPanel;
            if (panel != null)
            {
                builder.Append("panel(").Append(panel.Orientation).Append(")[");
                foreach (var child in panel.Children)
                    AppendElement(builder, child);
                builder.Append(']');
                return;
            }

            var documentPane = element as LayoutDocumentPane;
            if (documentPane != null)
            {
                builder.Append("documents[");
                foreach (var child in documentPane.Children)
                    AppendContent(builder, child);
                builder.Append(']');
                return;
            }

            var anchorablePane = element as LayoutAnchorablePane;
            if (anchorablePane != null)
            {
                builder.Append("tools(")
                    .Append(anchorablePane.Name)
                    .Append(',')
                    .Append(anchorablePane.DockWidth.Value)
                    .Append(',')
                    .Append(anchorablePane.DockWidth.GridUnitType)
                    .Append(',')
                    .Append(anchorablePane.DockHeight.Value)
                    .Append(',')
                    .Append(anchorablePane.DockHeight.GridUnitType)
                    .Append(")[");
                foreach (var child in anchorablePane.Children)
                    AppendContent(builder, child);
                builder.Append(']');
                return;
            }

            var group = element as LayoutAnchorablePaneGroup;
            if (group != null)
            {
                builder.Append("group(").Append(group.Orientation).Append(")[");
                foreach (var child in group.Children)
                    AppendElement(builder, child);
                builder.Append(']');
                return;
            }

            Assert.Fail("Unexpected layout element type: " + element.GetType().FullName);
        }

        private static void AppendContent(
            StringBuilder builder,
            LayoutContent content)
        {
            builder.Append(content.GetType().Name)
                .Append("(title=")
                .Append(content.Title)
                .Append(",selected=")
                .Append(content.IsSelected)
                .Append(",floating=")
                .Append(content.IsFloating);
            var anchorable = content as LayoutAnchorable;
            if (anchorable != null)
            {
                builder.Append(",visible=")
                    .Append(anchorable.IsVisible)
                    .Append(",bounds=")
                    .Append(anchorable.FloatingLeft)
                    .Append(',')
                    .Append(anchorable.FloatingTop)
                    .Append(',')
                    .Append(anchorable.FloatingWidth)
                    .Append(',')
                    .Append(anchorable.FloatingHeight);
            }
            builder.Append(')');
        }

        private sealed class FixtureLoadResult
        {
            public FixtureLoadResult(
                DockingManager manager,
                List<string> callbackContentIds)
            {
                Manager = manager;
                CallbackContentIds = callbackContentIds;
            }

            public DockingManager Manager { get; }

            public List<string> CallbackContentIds { get; }
        }

        private sealed class RestoredRuntime
        {
            public RestoredRuntime(
                ShellView view,
                DockingManager manager,
                RuntimeItems items,
                List<IDocument> documentCallbacks,
                List<ITool> toolCallbacks)
            {
                View = view;
                Manager = manager;
                Items = items;
                DocumentCallbacks = documentCallbacks;
                ToolCallbacks = toolCallbacks;
            }

            public ShellView View { get; }

            public DockingManager Manager { get; }

            public RuntimeItems Items { get; }

            public List<IDocument> DocumentCallbacks { get; }

            public List<ITool> ToolCallbacks { get; }
        }

        private sealed class RuntimeItems
        {
            private RuntimeItems(
                PersistentTestDocument primaryDocument,
                PersistentTestDocument secondaryDocument,
                PersistentTestTool primaryLeftTool,
                PersistentTestTool secondaryLeftTool,
                PersistentTestTool rightTool,
                PersistentTestTool bottomTool,
                PersistentTestTool floatingTool,
                PersistentTestTool hiddenTool)
            {
                PrimaryDocument = primaryDocument;
                SecondaryDocument = secondaryDocument;
                PrimaryLeftTool = primaryLeftTool;
                SecondaryLeftTool = secondaryLeftTool;
                RightTool = rightTool;
                BottomTool = bottomTool;
                FloatingTool = floatingTool;
                HiddenTool = hiddenTool;
                Documents = new[] { PrimaryDocument, SecondaryDocument };
                Tools = new[]
                {
                    PrimaryLeftTool,
                    SecondaryLeftTool,
                    RightTool,
                    BottomTool,
                    FloatingTool,
                    HiddenTool
                };
                SerializedContentMap = new Dictionary<string, ILayoutItem>();
            }

            public PersistentTestDocument PrimaryDocument { get; }

            public PersistentTestDocument SecondaryDocument { get; }

            public PersistentTestTool PrimaryLeftTool { get; }

            public PersistentTestTool SecondaryLeftTool { get; }

            public PersistentTestTool RightTool { get; }

            public PersistentTestTool BottomTool { get; }

            public PersistentTestTool FloatingTool { get; }

            public PersistentTestTool HiddenTool { get; }

            public PersistentTestDocument[] Documents { get; }

            public PersistentTestTool[] Tools { get; }

            public Dictionary<string, ILayoutItem> SerializedContentMap { get; }

            public static RuntimeItems CreateSource()
            {
                return new RuntimeItems(
                    new PersistentTestDocument("Primary document", "document-primary"),
                    new PersistentTestDocument("Secondary document", "document-secondary"),
                    new PersistentTestTool(
                        "Primary left tool",
                        "tool-left-primary",
                        PaneLocation.Left),
                    new PersistentTestTool(
                        "Secondary left tool",
                        "tool-left-secondary",
                        PaneLocation.Left),
                    new PersistentTestTool(
                        "Right tool",
                        "tool-right",
                        PaneLocation.Right),
                    new PersistentTestTool(
                        "Bottom tool",
                        "tool-bottom",
                        PaneLocation.Bottom),
                    new PersistentTestTool(
                        "Floating tool",
                        "tool-floating",
                        PaneLocation.Right),
                    new PersistentTestTool(
                        "Hidden tool",
                        "tool-hidden",
                        PaneLocation.Left));
            }

            public static RuntimeItems CreateForRestore(RuntimeItems source)
            {
                var restored = new RuntimeItems(
                    new PersistentTestDocument("Primary document", "unrestored"),
                    new PersistentTestDocument("Secondary document", "unrestored"),
                    new PersistentTestTool(
                        "Primary left tool",
                        "unrestored",
                        PaneLocation.Left),
                    new PersistentTestTool(
                        "Secondary left tool",
                        "unrestored",
                        PaneLocation.Left),
                    new PersistentTestTool(
                        "Right tool",
                        "unrestored",
                        PaneLocation.Right),
                    new PersistentTestTool(
                        "Bottom tool",
                        "unrestored",
                        PaneLocation.Bottom),
                    new PersistentTestTool(
                        "Floating tool",
                        "unrestored",
                        PaneLocation.Right),
                    new PersistentTestTool(
                        "Hidden tool",
                        "unrestored",
                        PaneLocation.Left));

                for (var i = 0; i < source.Documents.Length; i++)
                {
                    restored.SerializedContentMap.Add(
                        source.Documents[i].ContentId,
                        restored.Documents[i]);
                }
                for (var i = 0; i < source.Tools.Length; i++)
                {
                    restored.SerializedContentMap.Add(
                        source.Tools[i].ContentId,
                        restored.Tools[i]);
                }
                return restored;
            }
        }

        private sealed class PersistentTestDocument : Document
        {
            public PersistentTestDocument(string displayName, string stateMarker)
            {
                DisplayName = displayName;
                StateMarker = stateMarker;
            }

            public string StateMarker { get; private set; }

            public override bool ShouldReopenOnStart => true;

            public override void LoadState(BinaryReader reader)
            {
                StateMarker = reader.ReadString();
            }

            public override void SaveState(BinaryWriter writer)
            {
                writer.Write(StateMarker);
            }
        }

        private sealed class PersistentTestTool : Tool
        {
            public PersistentTestTool(
                string displayName,
                string stateMarker,
                PaneLocation preferredLocation)
            {
                DisplayName = displayName;
                StateMarker = stateMarker;
                PreferredLocation = preferredLocation;
            }

            public string StateMarker { get; private set; }

            public override PaneLocation PreferredLocation { get; }

            public override void LoadState(BinaryReader reader)
            {
                StateMarker = reader.ReadString();
            }

            public override void SaveState(BinaryWriter writer)
            {
                writer.Write(StateMarker);
            }
        }

        private sealed class TestDirectory : IDisposable
        {
            public TestDirectory()
            {
                Path = System.IO.Path.Combine(
                    System.IO.Path.GetTempPath(),
                    "gemini-avalondock-tests-" + Guid.NewGuid().ToString("N"));
                Directory.CreateDirectory(Path);
            }

            public string Path { get; }

            public string GetPath(string fileName)
            {
                return System.IO.Path.Combine(Path, fileName);
            }

            public void Dispose()
            {
                if (Directory.Exists(Path))
                    Directory.Delete(Path, true);
            }
        }

        private sealed class TestShell : Screen, IShell
        {
            public TestShell(
                IEnumerable<IDocument> documents = null,
                IEnumerable<ITool> tools = null)
            {
                Documents = new BindableCollection<IDocument>(
                    documents ?? Enumerable.Empty<IDocument>());
                Tools = new BindableCollection<ITool>(
                    tools ?? Enumerable.Empty<ITool>());
            }

            public event EventHandler ActiveDocumentChanging
            {
                add { }
                remove { }
            }

            public event EventHandler ActiveDocumentChanged
            {
                add { }
                remove { }
            }

            public bool ShowFloatingWindowsInTaskbar { get; set; }

            public Gemini.Modules.MainMenu.IMenu MainMenu => null;

            public Gemini.Modules.ToolBars.IToolBars ToolBars => null;

            public Gemini.Modules.StatusBar.IStatusBar StatusBar => null;

            public ILayoutItem ActiveLayoutItem { get; set; }

            public IDocument ActiveItem => ActiveLayoutItem as IDocument;

            public IObservableCollection<IDocument> Documents { get; }

            public IObservableCollection<ITool> Tools { get; }

            public Task InitializationTask => Task.CompletedTask;

            public bool RegisterTool(ITool tool)
            {
                if (Tools.Contains(tool))
                    return false;
                Tools.Add(tool);
                return true;
            }

            public Task ShowToolAsync<TTool>() where TTool : ITool
            {
                return Task.CompletedTask;
            }

            public Task ShowToolAsync(ITool model)
            {
                return Task.CompletedTask;
            }

            public Task CloseToolAsync(ITool tool)
            {
                return Task.CompletedTask;
            }

            public Task OpenDocumentAsync(IDocument model)
            {
                return Task.CompletedTask;
            }

            public Task CloseDocumentAsync(IDocument document)
            {
                return Task.CompletedTask;
            }

            public void Close()
            {
            }
        }
    }
}
