using System.Windows.Media;

namespace Gemini.Modules.Inspector.Inspectors
{
    public class ColorEditorViewModel : SelectiveUndoEditorBase<Color>, ILabelledInspector
    {
        private bool _usingAlphaChannel = true;

        public bool UsingAlphaChannel
        {
            get { return _usingAlphaChannel; }

            set
            {
                if (_usingAlphaChannel == value)
                    return;

                _usingAlphaChannel = value;

                NotifyOfPropertyChange(() => UsingAlphaChannel);
            }
        }

        public void Opened()
        {
            OnBeginEdit();
        }

        public void Closed()
        {
            OnEndEdit();
        }
    }
}