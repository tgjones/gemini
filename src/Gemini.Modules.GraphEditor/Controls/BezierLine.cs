using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Gemini.Modules.GraphEditor.Controls
{
    public class BezierLine : Shape
    {
        private const FrameworkPropertyMetadataOptions MetadataOptions =
            FrameworkPropertyMetadataOptions.AffectsMeasure | 
            FrameworkPropertyMetadataOptions.AffectsRender;

        private Geometry _geometry;

        public static readonly DependencyProperty X1Property = DependencyProperty.Register(
            "X1", typeof(double), typeof(BezierLine),
            new FrameworkPropertyMetadata(0.0, MetadataOptions));

        public double X1
        {
            get { return (double) GetValue(X1Property); }
            set { SetValue(X1Property, value); }
        }

        public static readonly DependencyProperty X2Property = DependencyProperty.Register(
            "X2", typeof(double), typeof(BezierLine),
            new FrameworkPropertyMetadata(0.0, MetadataOptions));

        public double X2
        {
            get { return (double) GetValue(X2Property); }
            set { SetValue(X2Property, value); }
        }

        public static readonly DependencyProperty Y1Property = DependencyProperty.Register(
            "Y1", typeof(double), typeof(BezierLine),
            new FrameworkPropertyMetadata(0.0, MetadataOptions));

        public double Y1
        {
            get { return (double) GetValue(Y1Property); }
            set { SetValue(Y1Property, value); }
        }

        public static readonly DependencyProperty Y2Property = DependencyProperty.Register(
            "Y2", typeof(double), typeof(BezierLine),
            new FrameworkPropertyMetadata(0.0, MetadataOptions));

        public double Y2
        {
            get { return (double) GetValue(Y2Property); }
            set { SetValue(Y2Property, value); }
        }

        protected override Geometry DefiningGeometry
        {
            get { return _geometry; }
        }

        protected override Size MeasureOverride(Size constraint)
        {
            var midX = X1 + ((X2 - X1) / 2);

            _geometry = new PathGeometry
            {
                Figures =
                {
                    new PathFigure
                    {
                        IsFilled = false,
                        StartPoint = new Point(X1, Y1),
                        Segments =
                        {
                            new BezierSegment
                            {
                                Point1 = new Point(midX, Y1),
                                Point2 = new Point(midX, Y2),
                                Point3 = new Point(X2, Y2),
                                IsStroked = true
                            }
                        }
                    }
                }
            };

            return base.MeasureOverride(constraint);
        }
    }
}