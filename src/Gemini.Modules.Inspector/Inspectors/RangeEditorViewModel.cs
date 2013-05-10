namespace Gemini.Modules.Inspector.Inspectors
{
    public class RangeEditorViewModel : EditorBase<double>
    {
        private readonly double _minimum;
        private readonly double _maximum;

        public double Minimum
        {
            get { return _minimum; }
        }

        public double Maximum
        {
            get { return _maximum; }
        }

        public RangeEditorViewModel(double minimum, double maximum)
        {
            _minimum = minimum;
            _maximum = maximum;
        }
    }
}