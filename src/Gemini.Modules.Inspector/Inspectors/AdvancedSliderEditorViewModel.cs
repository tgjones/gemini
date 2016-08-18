using System;

using Caliburn.Micro;

using Gemini.Framework.Controls;
using Gemini.Framework.Util;

namespace Gemini.Modules.Inspector.Inspectors
{
    public class AdvancedSliderEditorViewModel<TValue> : SelectiveUndoEditorBase<TValue>, ILabelledInspector, IViewAware where TValue : IComparable<TValue>
    {
        private static readonly string _defaultValueFormat = "{0:0.#####}";

        public AdvancedSliderEditorViewModel()
        {
            _valueFormat = _defaultValueFormat;
            _valueType = typeof(TValue);
            _type = AdvancedSliderBase.DisplayType.Number;
        }

        public AdvancedSliderEditorViewModel(TValue min, TValue max)
        {
            _minimum = min;
            _maximum = max;
            _valueFormat = _defaultValueFormat;
            _valueType = typeof(TValue);
            _type = AdvancedSliderBase.DisplayType.Bar;
        }

        private TValue _minimum;

        public TValue Minimum
        {
            get { return _minimum; }
            set
            {
                _minimum = value;
                NotifyOfPropertyChange(() => Minimum);
            }
        }

        private TValue _maximum;

        public TValue Maximum
        {
            get { return _maximum; }
            set
            {
                _maximum = value;
                NotifyOfPropertyChange(() => Maximum);
            }
        }

        private TValue _speed;

        public TValue Speed
        {
            get { return _speed; }
            set
            {
                _speed = value;
                NotifyOfPropertyChange(() => Speed);
            }
        }

        private bool _mouseCaptured;

        public bool MouseCaptured
        {
            get { return _mouseCaptured; }
            set
            {
                if (_mouseCaptured == value)
                    return;

                _mouseCaptured = value;

                if (value)
                    OnBeginEdit();
                else
                    OnEndEdit();
            }
        }

        private string _valueFormat;

        public string ValueFormat
        {
            get { return _valueFormat; }
            set
            {
                _valueFormat = value;
                NotifyOfPropertyChange(() => ValueFormat);
            }
        }

        private Type _valueType;

        public Type ValueType
        {
            get { return _valueType; }
            set
            {
                _valueType = value;
                NotifyOfPropertyChange(() => ValueType);
            }
        }

        private AdvancedSliderBase.DisplayType _type;

        public AdvancedSliderBase.DisplayType Type
        {
            get { return _type; }

            set
            {
                if (Equals(_type, value))
                    return;

                _type = value;

                NotifyOfPropertyChange(() => Type);
            }
        }

        public void Up()
        {
            _view.slider.ApplyValueChange(SpeedMultiplier.Get());
        }

        public void Down()
        {
            _view.slider.ApplyValueChange(-SpeedMultiplier.Get());
        }

        private AdvancedSliderEditorView _view;

        public event EventHandler<ViewAttachedEventArgs> ViewAttached;

        public void AttachView(object view, object context = null)
        {
            _view = (AdvancedSliderEditorView) view;
            ViewAttached?.Invoke(this, new ViewAttachedEventArgs() { View = view, Context = context });
        }

        public object GetView(object context = null)
        {
            return _view;
        }
    }
}
