using System;
using System.ComponentModel;
using System.Linq;
using Caliburn.Micro;
using Gemini.Framework;
using Gemini.Framework.Services;
using Gemini.Modules.Inspector;
using Gemini.Modules.Inspector.Inspectors;
using Gemini.Tests.Framework.Results;
using Gemini.Tests.TestInfrastructure;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Gemini.Tests.Modules.Inspector
{
    [STATestClass]
    [DoNotParallelize]
    public class InspectorActionTests
    {
        [TestMethod]
        public void CanReset_RepeatedGuardEvaluationDoesNotResetOrCreateUndoActions()
        {
            var document = new TestDocument();
            var shell = new ControlledShell { ActiveLayoutItem = document };
            var descriptor = new CountingPropertyDescriptor("changed", "default");

            using (CreateIoCScope(shell))
            using (var editor = new TestEditor
            {
                BoundPropertyDescriptor = new BoundPropertyDescriptor(new object(), descriptor)
            })
            {
                Assert.IsTrue(editor.CanReset);
                Assert.IsTrue(editor.CanReset);
                Assert.IsTrue(editor.CanReset);

                Assert.AreEqual(3, descriptor.CanResetCallCount);
                Assert.AreEqual(0, descriptor.ResetCallCount);
                Assert.AreEqual(0, document.UndoRedoManager.ActionStack.Count);
                Assert.AreEqual("changed", descriptor.CurrentValue);
            }
        }

        [TestMethod]
        public void Reset_WhenGuardAllows_CreatesExactlyOneUndoActionAndOneReset()
        {
            var document = new TestDocument();
            var shell = new ControlledShell { ActiveLayoutItem = document };
            var descriptor = new CountingPropertyDescriptor("changed", "default");

            using (CreateIoCScope(shell))
            using (var editor = new TestEditor
            {
                BoundPropertyDescriptor = new BoundPropertyDescriptor(new object(), descriptor)
            })
            {
                Assert.IsTrue(editor.CanReset);
                editor.Reset();
                editor.Reset();

                Assert.AreEqual(1, descriptor.ResetCallCount);
                Assert.AreEqual(1, document.UndoRedoManager.ActionStack.Count);
                Assert.AreEqual(1, document.UndoRedoManager.UndoActionCount);
                Assert.AreEqual("default", descriptor.CurrentValue);

                document.UndoRedoManager.Undo(1);

                Assert.AreEqual("changed", descriptor.CurrentValue);
            }
        }

        [TestMethod]
        public void SelectiveUndo_DuplicateBeginEndWithNullOriginal_GroupsOneUndoAction()
        {
            var document = new TestDocument();
            var shell = new ControlledShell { ActiveLayoutItem = document };
            var descriptor = new CountingPropertyDescriptor(null, null);
            var batchBeginCount = 0;
            var batchEndCount = 0;
            document.UndoRedoManager.BatchBegin += delegate { batchBeginCount++; };
            document.UndoRedoManager.BatchEnd += delegate { batchEndCount++; };

            using (CreateIoCScope(shell))
            using (var editor = new TestSelectiveEditor
            {
                BoundPropertyDescriptor = new BoundPropertyDescriptor(new object(), descriptor)
            })
            {
                editor.BeginEdit();
                editor.BeginEdit();
                editor.Value = "after";
                editor.EndEdit();
                editor.EndEdit();

                Assert.IsTrue(editor.IsUndoEnabled);
                Assert.AreEqual(1, document.UndoRedoManager.ActionStack.Count);
                Assert.AreEqual(1, document.UndoRedoManager.UndoActionCount);
                Assert.AreEqual(3, descriptor.GetValueCallCount);
                Assert.AreEqual(2, descriptor.SetValueCallCount);
                Assert.AreEqual("after", descriptor.CurrentValue);

                document.UndoRedoManager.Undo(1);

                Assert.AreEqual(1, batchBeginCount);
                Assert.AreEqual(1, batchEndCount);
                Assert.IsNull(descriptor.CurrentValue);
            }
        }

        private static IoCOverrideScope CreateIoCScope(IShell shell)
        {
            return new IoCOverrideScope(
                delegate(Type type, string key)
                {
                    if (type == typeof(IShell))
                        return shell;

                    throw new InvalidOperationException("Unexpected IoC request for " + type.FullName + ".");
                },
                type => Enumerable.Empty<object>(),
                instance => { });
        }

        private sealed class TestDocument : Document
        {
        }

        private sealed class TestEditor : EditorBase<object>
        {
        }

        private sealed class TestSelectiveEditor : SelectiveUndoEditorBase<object>
        {
            public void BeginEdit()
            {
                OnBeginEdit();
            }

            public void EndEdit()
            {
                OnEndEdit();
            }
        }

        private sealed class CountingPropertyDescriptor : PropertyDescriptor
        {
            private readonly object _defaultValue;
            private object _value;

            public CountingPropertyDescriptor(object value, object defaultValue)
                : base("Value", new Attribute[0])
            {
                _value = value;
                _defaultValue = defaultValue;
            }

            public int CanResetCallCount { get; private set; }

            public int GetValueCallCount { get; private set; }

            public int ResetCallCount { get; private set; }

            public int SetValueCallCount { get; private set; }

            public object CurrentValue => _value;

            public override Type ComponentType => typeof(object);

            public override bool IsReadOnly => false;

            public override Type PropertyType => typeof(object);

            public override bool CanResetValue(object component)
            {
                CanResetCallCount++;
                return !Equals(_value, _defaultValue);
            }

            public override object GetValue(object component)
            {
                GetValueCallCount++;
                return _value;
            }

            public override void ResetValue(object component)
            {
                ResetCallCount++;
                _value = _defaultValue;
            }

            public override void SetValue(object component, object value)
            {
                SetValueCallCount++;
                _value = value;
            }

            public override bool ShouldSerializeValue(object component)
            {
                return false;
            }
        }
    }
}