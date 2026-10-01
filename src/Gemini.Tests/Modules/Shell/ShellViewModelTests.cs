using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Gemini.Framework;
using Gemini.Modules.Shell.ViewModels;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Gemini.Tests.Modules.Shell
{
    [STATestClass]
    public class ShellViewModelTests
    {
        [TestMethod]
        [Timeout(10000)]
        public async Task OpenDocumentAsync_NewDocument_RaisesOrderedEventsAndEchoesActiveLayoutItem()
        {
            var shell = new ShellViewModel();
            var document = new TestDocument();
            var markers = new List<string>();
            var changingCount = 0;
            var changedCount = 0;
            object changingSender = null;
            object changedSender = null;

            shell.ActiveDocumentChanging += delegate(object sender, EventArgs eventArgs)
            {
                changingCount++;
                changingSender = sender;
                markers.Add("ActiveDocumentChanging");
            };
            shell.ActiveDocumentChanged += delegate(object sender, EventArgs eventArgs)
            {
                changedCount++;
                changedSender = sender;
                markers.Add("ActiveDocumentChanged");
            };

            await shell.OpenDocumentAsync(document);

            CollectionAssert.AreEqual(
                new[] { "ActiveDocumentChanging", "ActiveDocumentChanged" },
                markers);
            Assert.AreEqual(1, changingCount);
            Assert.AreEqual(1, changedCount);
            Assert.AreSame(shell, changingSender);
            Assert.AreSame(shell, changedSender);
            Assert.AreSame(document, shell.ActiveItem);
            Assert.AreSame(document, shell.ActiveLayoutItem);
        }

        private sealed class TestDocument : Document
        {
            public TestDocument()
            {
                DisplayName = "Test document";
            }
        }
    }
}
