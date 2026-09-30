using System;
using System.ComponentModel.Composition;
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
            if (!string.IsNullOrEmpty(_path))
            {
                _ = ExecutePathAsync();
                return;
            }

            var editor = _editor ?? (IDocument)IoC.GetInstance(_editorType, null);
            if (editor == null)
            {
                OnCompleted(null, true);
                return;
            }

            ConfigureEditor(editor);

            _shell
                .OpenDocumentAsync(editor)
                .ContinueWith(t =>
                {
                    OnCompleted(null, false);
                });
        }

        private async Task ExecutePathAsync()
        {
            try
            {
                await _editorOpeningService.OpenFileAsync(_path, ConfigureEditor);
                OnCompleted(null, false);
            }
            catch (OperationCanceledException)
            {
                OnCompleted(null, true);
            }
            catch (Exception exception)
            {
                OnCompleted(exception, false);
            }
        }

        private void ConfigureEditor(IDocument editor)
        {
            _setData?.Invoke(editor);
            _onConfigure?.Invoke(editor);

            editor.Deactivated += (sender, eventArgs) =>
            {
                if (eventArgs.WasClosed)
                    _onShutDown?.Invoke(editor);

                return Task.CompletedTask;
            };
        }
    }
}
