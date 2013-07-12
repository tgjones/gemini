using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Xml;
using Gemini.Framework;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Highlighting.Xshd;

namespace Gemini.Modules.CodeEditor
{
    [Export(typeof (IModule))]
    public class Module : ModuleBase
    {
        private bool _isHighlightingManagerInitialized;

        [Export]
        public HighlightingManager HighlightingManager
        {
            get
            {
                if (!_isHighlightingManagerInitialized)
                {
                    LoadHighlightingFiles();

                    _isHighlightingManagerInitialized = true;
                }

                return HighlightingManager.Instance;
            }
        }

        private static void LoadHighlightingFiles()
        {
            string path = Path.Combine(Directory.GetCurrentDirectory(), "SyntaxHighlighting");

            if (!Directory.Exists(path))
            {
                return;
            }

            HighlightingManager highlightingManager = HighlightingManager.Instance;
            IEnumerable<string> highlightingFiles = Directory.GetFiles(path, "*.xshd");

            foreach (string file in highlightingFiles)
            {
                try
                {
                    XshdSyntaxDefinition syntaxDefinition;

                    using (var reader = new XmlTextReader(file))
                        syntaxDefinition = HighlightingLoader.LoadXshd(reader);

                    IHighlightingDefinition highlightingDefinition =
                        HighlightingLoader.Load(syntaxDefinition, highlightingManager);

                    highlightingManager.RegisterHighlighting(syntaxDefinition.Name,
                                                             syntaxDefinition.Extensions.ToArray(),
                                                             highlightingDefinition);
                }
                catch
                {
                    continue;
                }
            }
        }
    }
}