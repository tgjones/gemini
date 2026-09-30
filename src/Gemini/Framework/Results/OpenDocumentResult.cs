using System;
using System.ComponentModel.Composition;
using System.Threading;
using System.Threading.Tasks;
using Caliburn.Micro;
using Gemini.Framework.Services;

namespace Gemini.Framework.Results
{
    public class OpenDocumentResult : OpenResultBase<IDocument>
    {
        private readonly IDocument _editor;
        private readonly Type _editorType;
        private readonly string _path;
        private System.Action _detachShutdownHandler;

#pragma warning disable 649
#pragma warning disable IDE0044 // Add readonly modifier
        [Import] private IShell _shell;
        [Import] private IEditorOpeningService _editorOpeningService;
#pragma warning restore IDE0044 // Add readonly modifier
#pragma warning restore 649

        public OpenDocumentResult(IDocument editor)
        {
            _editor = editor;
        }

        public OpenDocumentResult(string path)
        {
            _path = path;
        }

        public OpenDocumentResult(Type editorType)
        {
            _editorType = editorType;
        }

        public override void Execute(CoroutineExecutionContext context)
        {
            var execution = BeginExecution();
            DetachShutdownHandler();
            System.Action detachShutdownHandler = null;

            try
            {
                Task openTask;
                if (!string.IsNullOrEmpty(_path))
                {
                    openTask = _editorOpeningService.OpenFileAsync(
                        _path,
                        delegate(IDocument editor)
                        {
                            ConfigureEditor(editor);
                            detachShutdownHandler = AttachShutdownHandler(editor);
                        });
                }
                else
                {
                    var editor = _editor ?? (IDocument)IoC.GetInstance(_editorType, null);
                    if (editor == null)
                    {
                        execution.TryComplete(null, true);
                        return;
                    }

                    ConfigureEditor(editor);
                    detachShutdownHandler = AttachShutdownHandler(editor);
                    openTask = _shell.OpenDocumentAsync(editor);
                }

                _ = ObserveTaskAsync(
                    execution,
                    openTask,
                    true,
                    onFailure: delegate { detachShutdownHandler?.Invoke(); });
            }
            catch (OperationCanceledException)
            {
                detachShutdownHandler?.Invoke();
                execution.TryComplete(null, true);
            }
            catch (Exception exception)
            {
                detachShutdownHandler?.Invoke();
                execution.TryComplete(exception, false);
            }
        }

        private void ConfigureEditor(IDocument editor)
        {
            _setData?.Invoke(editor);
            _onConfigure?.Invoke(editor);
        }

        private System.Action AttachShutdownHandler(IDocument editor)
        {
            var detach = SubscribeToClosed(editor, editor, null);
            var previous = Interlocked.Exchange(ref _detachShutdownHandler, detach);
            previous?.Invoke();
            return detach;
        }

        private void DetachShutdownHandler()
        {
            Interlocked.Exchange(ref _detachShutdownHandler, null)?.Invoke();
        }
    }
}
