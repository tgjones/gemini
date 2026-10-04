using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using AvalonDock.Layout;
using Gemini.Framework;
using Gemini.Framework.Services;
using Gemini.Modules.Shell.Controls;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Gemini.Tests.Modules.Shell
{
    [STATestClass]
    [DoNotParallelize]
    public sealed class AvalonDockDockingTests
    {
        // Intent: pin Gemini's left-tool placement contract independently of vendor internals.
        [TestMethod]
        public void BeforeInsertAnchorable_LeftTool_CreatesNamedPaneAtHorizontalStart()
        {
            var topology = CreateDefaultTopology();
            var anchorable = CreateAnchorable(new TestTool(PaneLocation.Left, 321, 123));

            var inserted = new LayoutInitializer().BeforeInsertAnchorable(
                topology.Root,
                anchorable,
                topology.DocumentPane);

            Assert.IsTrue(inserted);
            var pane = Assert.IsInstanceOfType<LayoutAnchorablePane>(
                topology.HorizontalPanel.Children[0]);
            Assert.AreEqual("LeftPane", pane.Name);
            Assert.AreSame(anchorable, pane.Children[0]);
            Assert.AreSame(topology.DocumentPane, topology.HorizontalPanel.Children[1]);
            Assert.AreEqual(2, topology.HorizontalPanel.ChildrenCount);
        }

        // Intent: preserve right-tool ordering relied on by saved layouts and shell conventions.
        [TestMethod]
        public void BeforeInsertAnchorable_RightTool_CreatesNamedPaneAtHorizontalEnd()
        {
            var topology = CreateDefaultTopology();
            var anchorable = CreateAnchorable(new TestTool(PaneLocation.Right, 654, 123));

            var inserted = new LayoutInitializer().BeforeInsertAnchorable(
                topology.Root,
                anchorable,
                topology.DocumentPane);

            Assert.IsTrue(inserted);
            Assert.AreSame(topology.DocumentPane, topology.HorizontalPanel.Children[0]);
            var pane = Assert.IsInstanceOfType<LayoutAnchorablePane>(
                topology.HorizontalPanel.Children[1]);
            Assert.AreEqual("RightPane", pane.Name);
            Assert.AreSame(anchorable, pane.Children[0]);
            Assert.AreEqual(2, topology.HorizontalPanel.ChildrenCount);
        }

        // Intent: keep bottom tools outside the horizontal document/tool row.
        [TestMethod]
        public void BeforeInsertAnchorable_BottomTool_CreatesNamedPaneAtVerticalEnd()
        {
            var topology = CreateDefaultTopology();
            var anchorable = CreateAnchorable(new TestTool(PaneLocation.Bottom, 321, 246));

            var inserted = new LayoutInitializer().BeforeInsertAnchorable(
                topology.Root,
                anchorable,
                topology.DocumentPane);

            Assert.IsTrue(inserted);
            Assert.AreSame(topology.HorizontalPanel, topology.VerticalPanel.Children[0]);
            var pane = Assert.IsInstanceOfType<LayoutAnchorablePane>(
                topology.VerticalPanel.Children[1]);
            Assert.AreEqual("BottomPane", pane.Name);
            Assert.AreSame(anchorable, pane.Children[0]);
            Assert.AreEqual(2, topology.VerticalPanel.ChildrenCount);
        }

        // Intent: leave foreign anchorables to AvalonDock instead of claiming their placement.
        [TestMethod]
        public void BeforeInsertAnchorable_NonToolContent_DoesNotInterceptOrChangeTopology()
        {
            var topology = CreateDefaultTopology();
            var anchorable = new LayoutAnchorable
            {
                Content = new object(),
                ContentId = "non-gemini-content"
            };

            var inserted = new LayoutInitializer().BeforeInsertAnchorable(
                topology.Root,
                anchorable,
                topology.DocumentPane);

            Assert.IsFalse(inserted);
            Assert.AreEqual(1, topology.HorizontalPanel.ChildrenCount);
            Assert.AreSame(topology.DocumentPane, topology.HorizontalPanel.Children[0]);
            Assert.AreEqual(1, topology.VerticalPanel.ChildrenCount);
            Assert.IsNull(anchorable.Parent);
        }

        // Intent: reject invalid Gemini placement metadata without partially mutating the layout.
        [TestMethod]
        public void BeforeInsertAnchorable_UnsupportedPreferredLocation_ThrowsWithoutChangingTopology()
        {
            var topology = CreateDefaultTopology();
            var anchorable = CreateAnchorable(
                new TestTool((PaneLocation)int.MaxValue, 200, 200));

            var exception = Assert.ThrowsExactly<ArgumentOutOfRangeException>(
                () => new LayoutInitializer().BeforeInsertAnchorable(
                    topology.Root,
                    anchorable,
                    topology.DocumentPane));

            Assert.AreEqual("location", exception.ParamName);
            Assert.AreEqual(1, topology.HorizontalPanel.ChildrenCount);
            Assert.AreEqual(1, topology.VerticalPanel.ChildrenCount);
            Assert.IsNull(anchorable.Parent);
        }

        // Intent: apply a left tool's preferred width only when its pane is first established.
        [TestMethod]
        public void AfterInsertAnchorable_FirstSideTool_AppliesPreferredPixelWidth()
        {
            var topology = CreateDefaultTopology();
            var tool = new TestTool(PaneLocation.Left, 321, 123);
            var anchorable = CreateAnchorable(tool);
            var initializer = new LayoutInitializer();
            initializer.BeforeInsertAnchorable(topology.Root, anchorable, topology.DocumentPane);

            initializer.AfterInsertAnchorable(topology.Root, anchorable);

            var pane = Assert.IsInstanceOfType<LayoutAnchorablePane>(anchorable.Parent);
            Assert.AreEqual(GridUnitType.Pixel, pane.DockWidth.GridUnitType);
            Assert.AreEqual(321d, pane.DockWidth.Value);
            Assert.AreEqual("LeftPane", pane.Name);
        }

        // Intent: apply the same first-tool sizing rule to the independently named right pane.
        [TestMethod]
        public void AfterInsertAnchorable_FirstRightTool_AppliesPreferredPixelWidth()
        {
            var topology = CreateDefaultTopology();
            var tool = new TestTool(PaneLocation.Right, 654, 123);
            var anchorable = CreateAnchorable(tool);
            var initializer = new LayoutInitializer();
            initializer.BeforeInsertAnchorable(topology.Root, anchorable, topology.DocumentPane);

            initializer.AfterInsertAnchorable(topology.Root, anchorable);

            var pane = Assert.IsInstanceOfType<LayoutAnchorablePane>(anchorable.Parent);
            Assert.AreEqual(GridUnitType.Pixel, pane.DockWidth.GridUnitType);
            Assert.AreEqual(654d, pane.DockWidth.Value);
            Assert.AreEqual("RightPane", pane.Name);
        }

        // Intent: map bottom-tool sizing to height rather than a side-pane width.
        [TestMethod]
        public void AfterInsertAnchorable_FirstBottomTool_AppliesPreferredPixelHeight()
        {
            var topology = CreateDefaultTopology();
            var tool = new TestTool(PaneLocation.Bottom, 321, 246);
            var anchorable = CreateAnchorable(tool);
            var initializer = new LayoutInitializer();
            initializer.BeforeInsertAnchorable(topology.Root, anchorable, topology.DocumentPane);

            initializer.AfterInsertAnchorable(topology.Root, anchorable);

            var pane = Assert.IsInstanceOfType<LayoutAnchorablePane>(anchorable.Parent);
            Assert.AreEqual(GridUnitType.Pixel, pane.DockHeight.GridUnitType);
            Assert.AreEqual(246d, pane.DockHeight.Value);
            Assert.AreEqual("BottomPane", pane.Name);
        }

        // Intent: prevent later tools from overriding a user's established side-pane width.
        [TestMethod]
        public void AfterInsertAnchorable_LaterTool_PreservesFirstToolPaneDimension()
        {
            var topology = CreateDefaultTopology();
            var initializer = new LayoutInitializer();
            var first = CreateAnchorable(new TestTool(PaneLocation.Right, 420, 100));
            initializer.BeforeInsertAnchorable(topology.Root, first, topology.DocumentPane);
            initializer.AfterInsertAnchorable(topology.Root, first);
            var pane = Assert.IsInstanceOfType<LayoutAnchorablePane>(first.Parent);

            var later = CreateAnchorable(new TestTool(PaneLocation.Right, 999, 100));
            initializer.BeforeInsertAnchorable(topology.Root, later, topology.DocumentPane);
            initializer.AfterInsertAnchorable(topology.Root, later);

            Assert.AreSame(pane, later.Parent);
            Assert.AreEqual(2, pane.ChildrenCount);
            Assert.AreEqual(420d, pane.DockWidth.Value);
            Assert.AreEqual(GridUnitType.Pixel, pane.DockWidth.GridUnitType);
        }

        // Intent: prevent later bottom tools from overriding the established pane height.
        [TestMethod]
        public void AfterInsertAnchorable_LaterBottomTool_PreservesFirstToolPaneDimension()
        {
            var topology = CreateDefaultTopology();
            var initializer = new LayoutInitializer();
            var first = CreateAnchorable(new TestTool(PaneLocation.Bottom, 100, 275));
            initializer.BeforeInsertAnchorable(topology.Root, first, topology.DocumentPane);
            initializer.AfterInsertAnchorable(topology.Root, first);
            var pane = Assert.IsInstanceOfType<LayoutAnchorablePane>(first.Parent);

            var later = CreateAnchorable(new TestTool(PaneLocation.Bottom, 100, 888));
            initializer.BeforeInsertAnchorable(topology.Root, later, topology.DocumentPane);
            initializer.AfterInsertAnchorable(topology.Root, later);

            Assert.AreSame(pane, later.Parent);
            Assert.AreEqual(2, pane.ChildrenCount);
            Assert.AreEqual(275d, pane.DockHeight.Value);
            Assert.AreEqual(GridUnitType.Pixel, pane.DockHeight.GridUnitType);
        }

        // Intent: make invalid post-insert metadata fail without a partial resize.
        [TestMethod]
        public void AfterInsertAnchorable_UnsupportedPreferredLocation_ThrowsWithoutChangingDimensions()
        {
            var topology = CreateDefaultTopology();
            var anchorable = CreateAnchorable(
                new TestTool((PaneLocation)int.MaxValue, 777, 888));
            var pane = new LayoutAnchorablePane(anchorable)
            {
                Name = "UnsupportedPane",
                DockWidth = new GridLength(321, GridUnitType.Pixel),
                DockHeight = new GridLength(123, GridUnitType.Pixel)
            };
            topology.HorizontalPanel.InsertChildAt(0, pane);

            Assert.ThrowsExactly<ArgumentOutOfRangeException>(
                () => new LayoutInitializer().AfterInsertAnchorable(
                    topology.Root,
                    anchorable));

            Assert.AreEqual(321d, pane.DockWidth.Value);
            Assert.AreEqual(123d, pane.DockHeight.Value);
            Assert.AreSame(anchorable, pane.Children[0]);
        }

        // Intent: preserve restored user topology by reusing its semantic pane identity.
        [TestMethod]
        public void BeforeInsertAnchorable_RestoredNamedPane_ReusesPaneWithoutChangingRestoredTopology()
        {
            var topology = CreateDefaultTopology();
            var restoredPane = new LayoutAnchorablePane
            {
                Name = "RightPane",
                DockWidth = new GridLength(515, GridUnitType.Pixel)
            };
            topology.HorizontalPanel.InsertChildAt(0, restoredPane);
            var originalChildren = topology.HorizontalPanel.Children.ToArray();
            var anchorable = CreateAnchorable(new TestTool(PaneLocation.Right, 999, 100));

            var inserted = new LayoutInitializer().BeforeInsertAnchorable(
                topology.Root,
                anchorable,
                topology.DocumentPane);

            Assert.IsTrue(inserted);
            CollectionAssert.AreEqual(originalChildren, topology.HorizontalPanel.Children.ToArray());
            Assert.AreEqual(1, topology.Root.Descendents()
                .OfType<LayoutAnchorablePane>()
                .Count(x => x.Name == "RightPane"));
            Assert.AreSame(restoredPane, anchorable.Parent);
            Assert.AreEqual(515d, restoredPane.DockWidth.Value);
        }

        // Intent: find semantic panes after restore even when their child indexes have changed.
        [TestMethod]
        public void BeforeInsertAnchorable_ChangedTopology_FindsNamedPaneIndependentOfChildIndex()
        {
            var topology = CreateDefaultTopology();
            var leftPane = new LayoutAnchorablePane { Name = "LeftPane" };
            var group = new LayoutAnchorablePaneGroup(leftPane);
            topology.HorizontalPanel.Children.Add(group);
            var trailingDocumentPane = new LayoutDocumentPane();
            topology.HorizontalPanel.Children.Add(trailingDocumentPane);
            var anchorable = CreateAnchorable(new TestTool(PaneLocation.Left, 333, 100));

            var inserted = new LayoutInitializer().BeforeInsertAnchorable(
                topology.Root,
                anchorable,
                trailingDocumentPane);

            Assert.IsTrue(inserted);
            Assert.AreEqual(3, topology.HorizontalPanel.ChildrenCount);
            Assert.AreSame(topology.DocumentPane, topology.HorizontalPanel.Children[0]);
            Assert.AreSame(group, topology.HorizontalPanel.Children[1]);
            Assert.AreSame(trailingDocumentPane, topology.HorizontalPanel.Children[2]);
            Assert.AreSame(leftPane, anchorable.Parent);
            Assert.AreSame(anchorable, leftPane.Children[0]);
        }

        // Intent: keep document placement under AvalonDock rather than the tool initializer.
        [TestMethod]
        public void BeforeInsertDocument_Document_DoesNotInterceptAvalonDockInsertion()
        {
            var topology = CreateDefaultTopology();
            var document = CreateDocument(new TestDocument());

            var inserted = new LayoutInitializer().BeforeInsertDocument(
                topology.Root,
                document,
                topology.DocumentPane);

            Assert.IsFalse(inserted);
            Assert.AreEqual(0, topology.DocumentPane.ChildrenCount);
            Assert.IsNull(document.Parent);
        }

        // Intent: protect the selection feedback Gemini consumes when focus moves across item kinds.
        [TestMethod]
        public void LayoutSelection_DocumentThenTool_TracksSelectedAndActiveContent()
        {
            var topology = CreateDefaultTopology();
            var document = CreateDocument(new TestDocument());
            var tool = CreateAnchorable(new TestTool(PaneLocation.Left, 200, 200));
            topology.DocumentPane.Children.Add(document);
            var toolPane = new LayoutAnchorablePane(tool) { Name = "LeftPane" };
            topology.HorizontalPanel.InsertChildAt(0, toolPane);

            document.IsSelected = true;
            document.IsActive = true;

            Assert.AreSame(document, topology.DocumentPane.SelectedContent);
            Assert.AreSame(document, topology.Root.ActiveContent);
            Assert.IsTrue(document.IsSelected);

            tool.IsSelected = true;
            tool.IsActive = true;

            Assert.AreSame(tool, toolPane.SelectedContent);
            Assert.AreSame(tool, topology.Root.ActiveContent);
            Assert.IsTrue(tool.IsSelected);
            Assert.IsFalse(document.IsActive);
        }

        // Intent: ensure float/dock cycles move the same model without accumulating layout debris.
        [TestMethod]
        public void LayoutTransitions_TabFloatDock_PreserveContentIdentityAndDeterministicTopology()
        {
            var topology = CreateDefaultTopology();
            var first = CreateAnchorable(new TestTool(PaneLocation.Left, 200, 200));
            var moving = CreateAnchorable(new TestTool(PaneLocation.Left, 200, 200));
            var dockedPane = new LayoutAnchorablePane(first) { Name = "LeftPane" };
            dockedPane.Children.Add(moving);
            topology.HorizontalPanel.InsertChildAt(0, dockedPane);

            moving.IsSelected = true;
            Assert.AreSame(moving, dockedPane.SelectedContent);
            CollectionAssert.AreEqual(
                new[] { first.ContentId, moving.ContentId },
                dockedPane.Children.Select(x => x.ContentId).ToArray());

            dockedPane.Children.Remove(moving);
            var floatingPane = new LayoutAnchorablePane(moving) { Name = "FloatingPane" };
            var floatingWindow = new LayoutAnchorableFloatingWindow
            {
                RootPanel = new LayoutAnchorablePaneGroup(floatingPane)
            };
            topology.Root.FloatingWindows.Add(floatingWindow);

            Assert.AreEqual(1, topology.Root.FloatingWindows.Count);
            Assert.IsTrue(moving.IsFloating);
            Assert.AreSame(floatingPane, moving.Parent);
            Assert.AreEqual(moving.ContentId, floatingPane.Children[0].ContentId);
            Assert.AreEqual(1, dockedPane.ChildrenCount);

            floatingPane.Children.Remove(moving);
            topology.Root.FloatingWindows.Remove(floatingWindow);
            dockedPane.Children.Add(moving);
            moving.IsSelected = true;

            Assert.AreEqual(0, topology.Root.FloatingWindows.Count);
            Assert.IsFalse(moving.IsFloating);
            Assert.AreSame(dockedPane, moving.Parent);
            Assert.AreSame(moving, dockedPane.SelectedContent);
            CollectionAssert.AreEqual(
                new[] { first.ContentId, moving.ContentId },
                dockedPane.Children.Select(x => x.ContentId).ToArray());
        }

        private static LayoutTopology CreateDefaultTopology()
        {
            var documentPane = new LayoutDocumentPane();
            var horizontalPanel = new LayoutPanel(documentPane)
            {
                Orientation = Orientation.Horizontal
            };
            var verticalPanel = new LayoutPanel(horizontalPanel)
            {
                Orientation = Orientation.Vertical
            };
            var root = new LayoutRoot
            {
                RootPanel = verticalPanel
            };
            return new LayoutTopology(root, verticalPanel, horizontalPanel, documentPane);
        }

        private static LayoutAnchorable CreateAnchorable(TestTool tool)
        {
            return new LayoutAnchorable
            {
                Content = tool,
                ContentId = tool.ContentId,
                Title = tool.DisplayName
            };
        }

        private static LayoutDocument CreateDocument(TestDocument document)
        {
            return new LayoutDocument
            {
                Content = document,
                ContentId = document.ContentId,
                Title = document.DisplayName
            };
        }

        private sealed class LayoutTopology
        {
            public LayoutTopology(
                LayoutRoot root,
                LayoutPanel verticalPanel,
                LayoutPanel horizontalPanel,
                LayoutDocumentPane documentPane)
            {
                Root = root;
                VerticalPanel = verticalPanel;
                HorizontalPanel = horizontalPanel;
                DocumentPane = documentPane;
            }

            public LayoutRoot Root { get; }

            public LayoutPanel VerticalPanel { get; }

            public LayoutPanel HorizontalPanel { get; }

            public LayoutDocumentPane DocumentPane { get; }
        }

        private sealed class TestTool : Tool
        {
            public TestTool(
                PaneLocation preferredLocation,
                double preferredWidth,
                double preferredHeight)
            {
                PreferredLocation = preferredLocation;
                PreferredWidth = preferredWidth;
                PreferredHeight = preferredHeight;
                DisplayName = preferredLocation + " tool";
            }

            public override PaneLocation PreferredLocation { get; }

            public override double PreferredWidth { get; }

            public override double PreferredHeight { get; }
        }

        private sealed class TestDocument : Document
        {
            public TestDocument()
            {
                DisplayName = "Document";
            }
        }
    }
}
