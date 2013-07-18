using System.Collections.Generic;
using System.Linq;
using System.Xml;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Highlighting.Xshd;

namespace Gemini.Modules.CodeEditor
{
    internal class DefaultLanguageDefinition : ILanguageDefinition
    {
        private readonly string _name;
        private IHighlightingDefinition _highlightingDefinition;
        private XshdSyntaxDefinition _syntaxDefinition;

        public DefaultLanguageDefinition(string name, IEnumerable<string> fileExtensions)
        {
            _name = name;
            FileExtensions = fileExtensions;
        }

        public DefaultLanguageDefinition(XshdSyntaxDefinition syntaxDefinition)
            : this(syntaxDefinition.Name, syntaxDefinition.Extensions)
        {
            _syntaxDefinition = syntaxDefinition;
        }

        public string Name
        {
            get { return _name; }
        }

        public IEnumerable<string> FileExtensions { get; set; }

        public IHighlightingDefinition SyntaxHighlighting
        {
            get
            {
                if (_highlightingDefinition == null)
                {
                    _highlightingDefinition = LoadHighlightingDefinition();
                }

                return _highlightingDefinition;
            }
        }

        public string CustomSyntaxHighlightingFileName { get; set; }

        private IHighlightingDefinition LoadHighlightingDefinition()
        {
            HighlightingManager highlightingManager = HighlightingManager.Instance;

            if (!string.IsNullOrEmpty(CustomSyntaxHighlightingFileName))
            {
                using (var reader = new XmlTextReader(CustomSyntaxHighlightingFileName))
                    _syntaxDefinition = HighlightingLoader.LoadXshd(reader);
            }

            if (_syntaxDefinition != null)
            {
                IHighlightingDefinition highlightingDefinition =
                    HighlightingLoader.Load(_syntaxDefinition, highlightingManager);

                highlightingManager.RegisterHighlighting(_syntaxDefinition.Name,
                                                         _syntaxDefinition.Extensions.ToArray(),
                                                         highlightingDefinition);
            }

            return highlightingManager.GetDefinition(_name);
        }
    }
}