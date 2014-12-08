using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace Gemini.Modules.Inspector.Controls
{
    // From http://blog.onedevjob.com/2011/12/11/fixing-wpf-gridsplitter/.
    // Using a custom splitter because the built-in GridSplitter has a bug
    // when using *-widths:
    // https://connect.microsoft.com/VisualStudio/feedback/details/483010/wpf-gridsplitter-randomly-jumps-when-resizing
    public class SimpleGridSplitter : Thumb
    {
        static SimpleGridSplitter()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof (SimpleGridSplitter),
                new FrameworkPropertyMetadata(typeof (SimpleGridSplitter)));

            EventManager.RegisterClassHandler(typeof(SimpleGridSplitter), DragStartedEvent, new DragStartedEventHandler(OnDragStarted));
            EventManager.RegisterClassHandler(typeof(SimpleGridSplitter), DragDeltaEvent, new DragDeltaEventHandler(OnDragDelta));
            EventManager.RegisterClassHandler(typeof(SimpleGridSplitter), DragCompletedEvent, new DragCompletedEventHandler(OnDragCompleted));
        }

        private const double DefaultKeyboardIncrement = 1d;
        private bool dragging;

        #region ResizeBehavior
        /// <summary>
        /// ResizeBehavior Dependency Property
        /// </summary>
        public static readonly DependencyProperty ResizeBehaviorProperty =
            DependencyProperty.Register(
                "ResizeBehavior",
                typeof(GridResizeBehavior),
                typeof(SimpleGridSplitter),
                new PropertyMetadata(GridResizeBehavior.BasedOnAlignment));

        /// <summary>
        /// Gets or sets the ResizeBehavior property. This dependency property 
        /// indicates which columns or rows are resized relative
        /// to the column or row for which the GridSplitter control is defined.
        /// </summary>
        public GridResizeBehavior ResizeBehavior
        {
            get { return (GridResizeBehavior) GetValue(ResizeBehaviorProperty); }
            set { SetValue(ResizeBehaviorProperty, value); }
        }
        #endregion

        #region ResizeDirection
        /// <summary>
        /// ResizeDirection Dependency Property
        /// </summary>
        public static readonly DependencyProperty ResizeDirectionProperty =
            DependencyProperty.Register(
                "ResizeDirection",
                typeof(GridResizeDirection),
                typeof(SimpleGridSplitter),
                new PropertyMetadata(GridResizeDirection.Auto, OnResizeDirectionChanged));

        /// <summary>
        /// Gets or sets the ResizeDirection property. This dependency property 
        /// indicates whether the SimpleGridSplitter control resizes rows or columns.
        /// </summary>
        public GridResizeDirection ResizeDirection
        {
            get { return (GridResizeDirection) GetValue(ResizeDirectionProperty); }
            set { SetValue(ResizeDirectionProperty, value); }
        }

        /// <summary>
        /// Handles changes to the ResizeDirection property.
        /// </summary>
        /// <param name="d">
        /// The <see cref="DependencyObject"/> on which
        /// the property has changed value.
        /// </param>
        /// <param name="e">
        /// Event data that is issued by any event that
        /// tracks changes to the effective value of this property.
        /// </param>
        private static void OnResizeDirectionChanged(
            DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var target = (SimpleGridSplitter) d;
            GridResizeDirection oldResizeDirection = (GridResizeDirection) e.OldValue;
            GridResizeDirection newResizeDirection = target.ResizeDirection;
            target.OnResizeDirectionChanged(oldResizeDirection, newResizeDirection);
        }

        /// <summary>
        /// Provides derived classes an opportunity to handle changes
        /// to the ResizeDirection property.
        /// </summary>
        /// <param name="oldResizeDirection">The old ResizeDirection value</param>
        /// <param name="newResizeDirection">The new ResizeDirection value</param>
        protected virtual void OnResizeDirectionChanged(
            GridResizeDirection oldResizeDirection, GridResizeDirection newResizeDirection)
        {
            this.DetermineResizeCursor();
        }
        #endregion

        #region KeyboardIncrement
        /// <summary>
        /// KeyboardIncrement Dependency Property
        /// </summary>
        public static readonly DependencyProperty KeyboardIncrementProperty =
            DependencyProperty.Register(
                "KeyboardIncrement",
                typeof(double),
                typeof(SimpleGridSplitter),
                new FrameworkPropertyMetadata(DefaultKeyboardIncrement));

        /// <summary>
        /// Gets or sets the KeyboardIncrement property. This dependency property 
        /// indicates the distance that each press of an arrow key moves
        /// a SimpleGridSplitter control.
        /// </summary>
        public double KeyboardIncrement
        {
            get { return (double) GetValue(KeyboardIncrementProperty); }
            set { SetValue(KeyboardIncrementProperty, value); }
        }
        #endregion

        #region DetermineEffectiveResizeDirection()
        private GridResizeDirection DetermineEffectiveResizeDirection()
        {
            if (ResizeDirection == GridResizeDirection.Columns)
            {
                return GridResizeDirection.Columns;
            }

            if (ResizeDirection == GridResizeDirection.Rows)
            {
                return GridResizeDirection.Rows;
            }

            // Based on GridResizeDirection Enumeration documentation from
            // http://msdn.microsoft.com/en-us/library/system.windows.controls.gridresizedirection(v=VS.110).aspx

            // Space is redistributed based on the values of the HorizontalAlignment, VerticalAlignment, ActualWidth, and ActualHeight properties of the SimpleGridSplitter.

            // * If the HorizontalAlignment is not set to Stretch, space is redistributed between columns.
            if (HorizontalAlignment != HorizontalAlignment.Stretch)
            {
                return GridResizeDirection.Columns;
            }

            // * If the HorizontalAlignment is set to Stretch and the VerticalAlignment is not set to Stretch, space is redistributed between rows.
            if (this.HorizontalAlignment == HorizontalAlignment.Stretch &&
                this.VerticalAlignment != VerticalAlignment.Stretch)
            {
                return GridResizeDirection.Rows;
            }

            // * If the following conditions are true, space is redistributed between columns:
            //   * The HorizontalAlignment is set to Stretch.
            //   * The VerticalAlignment is set to Stretch.
            //   * The ActualWidth is less than or equal to the ActualHeight.
            if (this.HorizontalAlignment == HorizontalAlignment.Stretch &&
                this.VerticalAlignment == VerticalAlignment.Stretch &&
                this.ActualWidth <= this.ActualHeight)
            {
                return GridResizeDirection.Columns;
            }

            // * If the following conditions are true, space is redistributed between rows:
            //   * HorizontalAlignment is set to Stretch.
            //   * VerticalAlignment is set to Stretch.
            //   * ActualWidth is greater than the ActualHeight.
            //if (this.HorizontalAlignment == HorizontalAlignment.Stretch &&
            //    this.VerticalAlignment == VerticalAlignment.Stretch &&
            //    this.ActualWidth > this.ActualHeight)
            {
                return GridResizeDirection.Rows;
            }
        }
        #endregion

        #region DetermineEffectiveResizeBehavior()
        private GridResizeBehavior DetermineEffectiveResizeBehavior()
        {
            if (ResizeBehavior == GridResizeBehavior.CurrentAndNext)
            {
                return GridResizeBehavior.CurrentAndNext;
            }

            if (ResizeBehavior == GridResizeBehavior.PreviousAndCurrent)
            {
                return GridResizeBehavior.PreviousAndCurrent;
            }

            if (ResizeBehavior == GridResizeBehavior.PreviousAndNext)
            {
                return GridResizeBehavior.PreviousAndNext;
            }

            // Based on GridResizeBehavior Enumeration documentation from
            // http://msdn.microsoft.com/en-us/library/system.windows.controls.gridresizebehavior(v=VS.110).aspx

            // Space is redistributed based on the value of the
            // HorizontalAlignment and VerticalAlignment properties.

            var effectiveResizeDirection =
                DetermineEffectiveResizeDirection();

            // If the value of the ResizeDirection property specifies
            // that space is redistributed between rows,
            // the redistribution follows these guidelines:

            if (effectiveResizeDirection == GridResizeDirection.Rows)
            {
                // * When the VerticalAlignment property is set to Top,
                //   space is redistributed between the row that is specified
                //   for the GridSplitter and the row that is above that row.
                if (this.VerticalAlignment == VerticalAlignment.Top)
                {
                    return GridResizeBehavior.PreviousAndCurrent;
                }

                // * When the VerticalAlignment property is set to Bottom,
                //   space is redistributed between the row that is specified
                //   for the GridSplitter and the row that is below that row.
                if (this.VerticalAlignment == VerticalAlignment.Bottom)
                {
                    return GridResizeBehavior.CurrentAndNext;
                }

                // * When the VerticalAlignment property is set to Center,
                //   space is redistributed between the row that is above and
                //   the row that is below the row that is specified
                //   for the GridSplitter.
                // * When the VerticalAlignment property is set to Stretch,
                //   space is redistributed between the row that is above
                //   and the row that is below the row that is specified
                //   for the GridSplitter.
                return GridResizeBehavior.PreviousAndNext;
            }

            // If the value of the ResizeDirection property specifies
            // that space is redistributed between columns,
            // the redistribution follows these guidelines:

            // * When the HorizontalAlignment property is set to Left,
            //   space is redistributed between the column that is specified
            //   for the GridSplitter and the column that is to the left.
            if (this.HorizontalAlignment == HorizontalAlignment.Left)
            {
                return GridResizeBehavior.PreviousAndCurrent;
            }

            // * When the HorizontalAlignment property is set to Right,
            //   space is redistributed between the column that is specified
            //   for the GridSplitter and the column that is to the right.
            if (this.HorizontalAlignment == HorizontalAlignment.Right)
            {
                return GridResizeBehavior.CurrentAndNext;
            }

            // * When the HorizontalAlignment property is set to Center,
            //   space is redistributed between the columns that are to the left
            //   and right of the column that is specified for the GridSplitter.
            // * When the HorizontalAlignment property is set to Stretch,
            //   space is redistributed between the columns that are to the left
            //   and right of the column that is specified for the GridSplitter.
            return GridResizeBehavior.PreviousAndNext;
        }
        #endregion

        #region DetermineResizeCursor()
        private void DetermineResizeCursor()
        {
            var effectiveResizeDirection =
                this.DetermineEffectiveResizeDirection();

            if (effectiveResizeDirection == GridResizeDirection.Columns)
            {
                this.Cursor = Cursors.SizeWE;
            }
            else
            {
                this.Cursor = Cursors.SizeNS;
            }
        }
        #endregion

        #region CTOR - SimpleGridSplitter()
        // The below code throws for some reason, so the focus properties 
        // for keyboard support had to be moved to the constructor.
        // 
        //static SimpleGridSplitter()
        //{
        //    FocusableProperty.OverrideMetadata(
        //        typeof(SimpleGridSplitter),
        //        new UIPropertyMetadata(true));
        //    IsTabStopProperty.OverrideMetadata(
        //        typeof(SimpleGridSplitter),
        //        new UIPropertyMetadata(true));
        //}

        public SimpleGridSplitter()
        {
            //FocusManager.SetIsFocusScope(this, true);
            this.DetermineResizeCursor();
        }
        #endregion

        #region Mouse event handlers

        protected override void OnDragEnter(DragEventArgs e)
        {
            base.OnDragEnter(e);
        }

        protected override void OnMouseEnter(MouseEventArgs e)
        {
            this.DetermineResizeCursor();
            base.OnMouseEnter(e);
        }

        private static void OnDragStarted(object sender, DragStartedEventArgs e)
        {
            ((SimpleGridSplitter) sender).OnDragStarted(e);
        }

        private void OnDragStarted(DragStartedEventArgs e)
        {
            //this.CaptureMouse();
            //var grid = GetGrid();
            //this.lastPosition = e.GetPosition(grid);
            this.dragging = true;
            //this.Focus();
        }

        private static void OnDragDelta(object sender, DragDeltaEventArgs e)
        {
            ((SimpleGridSplitter) sender).OnDragDelta(e);
        }

        private void OnDragDelta(DragDeltaEventArgs e)
        {
            if (!dragging)
            {
                return;
            }

            GridResizeDirection effectiveResizeDirection =
                this.DetermineEffectiveResizeDirection();

            var grid = GetGrid();

            if (effectiveResizeDirection == GridResizeDirection.Columns)
            {
                var deltaX = e.HorizontalChange;
                this.ResizeColumns(grid, deltaX);
            }
            else
            {
                var deltaY = e.VerticalChange;
                this.ResizeRows(grid, deltaY);
            }
        }

        private static void OnDragCompleted(object sender, DragCompletedEventArgs e)
        {
            ((SimpleGridSplitter) sender).OnDragCompleted(e);
        }

        private void OnDragCompleted(DragCompletedEventArgs e)
        {
            this.dragging = false;
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);

            GridResizeDirection effectiveResizeDirection =
                this.DetermineEffectiveResizeDirection();

            if (effectiveResizeDirection == GridResizeDirection.Columns)
            {
                if (e.Key == Key.Left)
                {
                    this.ResizeColumns(this.GetGrid(), -KeyboardIncrement);
                    e.Handled = true;
                }
                else if (e.Key == Key.Right)
                {
                    this.ResizeColumns(this.GetGrid(), KeyboardIncrement);
                    e.Handled = true;
                }
            }
            else
            {
                if (e.Key == Key.Up)
                {
                    this.ResizeRows(this.GetGrid(), -KeyboardIncrement);
                    e.Handled = true;
                }
                else if (e.Key == Key.Down)
                {
                    this.ResizeRows(this.GetGrid(), KeyboardIncrement);
                    e.Handled = true;
                }
            }
        }
        #endregion

        #region ResizeColumns()
        private void ResizeColumns(Grid grid, double deltaX)
        {
            GridResizeBehavior effectiveGridResizeBehavior =
                this.DetermineEffectiveResizeBehavior();

            int column = Grid.GetColumn(this);
            int leftColumn;
            int rightColumn;

            switch (effectiveGridResizeBehavior)
            {
                case GridResizeBehavior.PreviousAndCurrent:
                    leftColumn = column - 1;
                    rightColumn = column;
                    break;
                case GridResizeBehavior.PreviousAndNext:
                    leftColumn = column - 1;
                    rightColumn = column + 1;
                    break;
                default:
                    leftColumn = column;
                    rightColumn = column + 1;
                    break;
            }

            if (rightColumn >= grid.ColumnDefinitions.Count)
            {
                return;
            }

            var leftColumnDefinition = grid.ColumnDefinitions[leftColumn];
            var rightColumnDefinition = grid.ColumnDefinitions[rightColumn];
            var leftColumnGridUnitType = leftColumnDefinition.Width.GridUnitType;
            var rightColumnGridUnitType = rightColumnDefinition.Width.GridUnitType;
            var leftColumnActualWidth = leftColumnDefinition.ActualWidth;
            var rightColumnActualWidth = rightColumnDefinition.ActualWidth;
            var leftColumnMaxWidth = leftColumnDefinition.MaxWidth;
            var rightColumnMaxWidth = rightColumnDefinition.MaxWidth;
            var leftColumnMinWidth = leftColumnDefinition.MinWidth;
            var rightColumnMinWidth = rightColumnDefinition.MinWidth;

            //deltaX = 200;
            if (leftColumnActualWidth + deltaX > leftColumnMaxWidth)
            {
                deltaX = Math.Max(
                    0,
                    leftColumnDefinition.MaxWidth - leftColumnActualWidth);
            }

            if (leftColumnActualWidth + deltaX < leftColumnMinWidth)
            {
                deltaX = Math.Min(
                    0,
                    leftColumnDefinition.MinWidth - leftColumnActualWidth);
            }

            if (rightColumnActualWidth - deltaX > rightColumnMaxWidth)
            {
                deltaX = -Math.Max(
                    0,
                    rightColumnDefinition.MaxWidth - rightColumnActualWidth);
            }

            if (rightColumnActualWidth - deltaX < rightColumnMinWidth)
            {
                deltaX = -Math.Min(
                    0,
                    rightColumnDefinition.MinWidth - rightColumnActualWidth);
            }

            var newLeftColumnActualWidth = leftColumnActualWidth + deltaX;
            var newRightColumnActualWidth = rightColumnActualWidth - deltaX;

            grid.BeginInit();

            double totalStarColumnsWidth = 0;
            double starColumnsAvailableWidth = grid.ActualWidth;

            if (leftColumnGridUnitType ==
                    GridUnitType.Star ||
                rightColumnGridUnitType ==
                    GridUnitType.Star)
            {
                foreach (var columnDefinition in grid.ColumnDefinitions)
                {
                    if (columnDefinition.Width.GridUnitType ==
                        GridUnitType.Star)
                    {
                        totalStarColumnsWidth +=
                            columnDefinition.Width.Value;
                    }
                    else
                    {
                        starColumnsAvailableWidth -=
                            columnDefinition.ActualWidth;
                    }
                }
            }

            if (leftColumnGridUnitType == GridUnitType.Star)
            {
                if (rightColumnGridUnitType == GridUnitType.Star)
                {
                    // If both columns are star columns
                    // - totalStarColumnsWidth won't change and
                    // as much as one of the columns grows
                    // - the other column will shrink by the same value.

                    // If there is no width available to star columns
                    // - we can't resize two of them.
                    if (starColumnsAvailableWidth < 1)
                    {
                        return;
                    }

                    var oldStarWidth = leftColumnDefinition.Width.Value;
                    var newStarWidth = Math.Max(
                        0,
                        totalStarColumnsWidth * newLeftColumnActualWidth /
                            starColumnsAvailableWidth);
                    leftColumnDefinition.Width =
                        new GridLength(newStarWidth, GridUnitType.Star);

                    rightColumnDefinition.Width =
                        new GridLength(
                            Math.Max(
                                0,
                                rightColumnDefinition.Width.Value -
                                    newStarWidth + oldStarWidth),
                            GridUnitType.Star);
                }
                else
                {
                    var newStarColumnsAvailableWidth =
                        starColumnsAvailableWidth +
                        rightColumnActualWidth -
                        newRightColumnActualWidth;

                    if (newStarColumnsAvailableWidth - newLeftColumnActualWidth >= 1)
                    {
                        var newStarWidth = Math.Max(
                            0,
                            (totalStarColumnsWidth -
                             leftColumnDefinition.Width.Value) *
                            newLeftColumnActualWidth /
                            (newStarColumnsAvailableWidth - newLeftColumnActualWidth));

                        leftColumnDefinition.Width =
                            new GridLength(newStarWidth, GridUnitType.Star);
                    }
                }
            }
            else
            {
                leftColumnDefinition.Width =
                    new GridLength(
                        newLeftColumnActualWidth, GridUnitType.Pixel);
            }

            if (rightColumnGridUnitType ==
                GridUnitType.Star)
            {
                if (leftColumnGridUnitType !=
                    GridUnitType.Star)
                {
                    var newStarColumnsAvailableWidth =
                        starColumnsAvailableWidth +
                        leftColumnActualWidth -
                        newLeftColumnActualWidth;

                    if (newStarColumnsAvailableWidth - newRightColumnActualWidth >= 1)
                    {
                        var newStarWidth = Math.Max(
                            0,
                            (totalStarColumnsWidth -
                             rightColumnDefinition.Width.Value) *
                            newRightColumnActualWidth /
                            (newStarColumnsAvailableWidth - newRightColumnActualWidth));
                        rightColumnDefinition.Width =
                            new GridLength(newStarWidth, GridUnitType.Star);
                    }
                }
                // else handled in the left column width calculation block
            }
            else
            {
                rightColumnDefinition.Width =
                    new GridLength(
                        newRightColumnActualWidth, GridUnitType.Pixel);
            }

            grid.EndInit();
        }
        #endregion

        #region ResizeRows()
        private void ResizeRows(Grid grid, double deltaX)
        {
            GridResizeBehavior effectiveGridResizeBehavior =
                this.DetermineEffectiveResizeBehavior();

            int row = Grid.GetRow(this);
            int upperRow;
            int lowerRow;

            switch (effectiveGridResizeBehavior)
            {
                case GridResizeBehavior.PreviousAndCurrent:
                    upperRow = row - 1;
                    lowerRow = row;
                    break;
                case GridResizeBehavior.PreviousAndNext:
                    upperRow = row - 1;
                    lowerRow = row + 1;
                    break;
                default:
                    upperRow = row;
                    lowerRow = row + 1;
                    break;
            }

            if (lowerRow >= grid.RowDefinitions.Count)
            {
                return;
            }

            var upperRowDefinition = grid.RowDefinitions[upperRow];
            var lowerRowDefinition = grid.RowDefinitions[lowerRow];
            var upperRowGridUnitType = upperRowDefinition.Height.GridUnitType;
            var lowerRowGridUnitType = lowerRowDefinition.Height.GridUnitType;
            var upperRowActualHeight = upperRowDefinition.ActualHeight;
            var lowerRowActualHeight = lowerRowDefinition.ActualHeight;
            var upperRowMaxHeight = upperRowDefinition.MaxHeight;
            var lowerRowMaxHeight = lowerRowDefinition.MaxHeight;
            var upperRowMinHeight = upperRowDefinition.MinHeight;
            var lowerRowMinHeight = lowerRowDefinition.MinHeight;

            //deltaX = 200;
            if (upperRowActualHeight + deltaX > upperRowMaxHeight)
            {
                deltaX = Math.Max(
                    0,
                    upperRowDefinition.MaxHeight - upperRowActualHeight);
            }

            if (upperRowActualHeight + deltaX < upperRowMinHeight)
            {
                deltaX = Math.Min(
                    0,
                    upperRowDefinition.MinHeight - upperRowActualHeight);
            }

            if (lowerRowActualHeight - deltaX > lowerRowMaxHeight)
            {
                deltaX = -Math.Max(
                    0,
                    lowerRowDefinition.MaxHeight - lowerRowActualHeight);
            }

            if (lowerRowActualHeight - deltaX < lowerRowMinHeight)
            {
                deltaX = -Math.Min(
                    0,
                    lowerRowDefinition.MinHeight - lowerRowActualHeight);
            }

            var newUpperRowActualHeight = upperRowActualHeight + deltaX;
            var newLowerRowActualHeight = lowerRowActualHeight - deltaX;

            grid.BeginInit();

            double totalStarRowsHeight = 0;
            double starRowsAvailableHeight = grid.ActualHeight;

            if (upperRowGridUnitType ==
                    GridUnitType.Star ||
                lowerRowGridUnitType ==
                    GridUnitType.Star)
            {
                foreach (var rowDefinition in grid.RowDefinitions)
                {
                    if (rowDefinition.Height.GridUnitType ==
                        GridUnitType.Star)
                    {
                        totalStarRowsHeight +=
                            rowDefinition.Height.Value;
                    }
                    else
                    {
                        starRowsAvailableHeight -=
                            rowDefinition.ActualHeight;
                    }
                }
            }

            if (upperRowGridUnitType == GridUnitType.Star)
            {
                if (lowerRowGridUnitType == GridUnitType.Star)
                {
                    // If both rows are star rows
                    // - totalStarRowsHeight won't change and
                    // as much as one of the rows grows
                    // - the other row will shrink by the same value.

                    // If there is no width available to star rows
                    // - we can't resize two of them.
                    if (starRowsAvailableHeight < 1)
                    {
                        return;
                    }

                    var oldStarHeight = upperRowDefinition.Height.Value;
                    var newStarHeight = Math.Max(
                        0,
                        totalStarRowsHeight * newUpperRowActualHeight /
                            starRowsAvailableHeight);
                    upperRowDefinition.Height =
                        new GridLength(newStarHeight, GridUnitType.Star);

                    lowerRowDefinition.Height =
                        new GridLength(
                            Math.Max(
                                0,
                                lowerRowDefinition.Height.Value -
                                    newStarHeight + oldStarHeight),
                            GridUnitType.Star);
                }
                else
                {
                    var newStarRowsAvailableHeight =
                        starRowsAvailableHeight +
                        lowerRowActualHeight -
                        newLowerRowActualHeight;

                    if (newStarRowsAvailableHeight - newUpperRowActualHeight >= 1)
                    {
                        var newStarHeight = Math.Max(
                            0,
                            (totalStarRowsHeight -
                             upperRowDefinition.Height.Value) *
                            newUpperRowActualHeight /
                            (newStarRowsAvailableHeight - newUpperRowActualHeight));

                        upperRowDefinition.Height =
                            new GridLength(newStarHeight, GridUnitType.Star);
                    }
                }
            }
            else
            {
                upperRowDefinition.Height =
                    new GridLength(
                        newUpperRowActualHeight, GridUnitType.Pixel);
            }

            if (lowerRowGridUnitType ==
                GridUnitType.Star)
            {
                if (upperRowGridUnitType !=
                    GridUnitType.Star)
                {
                    var newStarRowsAvailableHeight =
                        starRowsAvailableHeight +
                        upperRowActualHeight -
                        newUpperRowActualHeight;

                    if (newStarRowsAvailableHeight - newLowerRowActualHeight >= 1)
                    {
                        var newStarHeight = Math.Max(
                            0,
                            (totalStarRowsHeight -
                             lowerRowDefinition.Height.Value) *
                            newLowerRowActualHeight /
                            (newStarRowsAvailableHeight - newLowerRowActualHeight));
                        lowerRowDefinition.Height =
                            new GridLength(newStarHeight, GridUnitType.Star);
                    }
                }
                // else handled in the upper row width calculation block
            }
            else
            {
                lowerRowDefinition.Height =
                    new GridLength(
                        newLowerRowActualHeight, GridUnitType.Pixel);
            }

            grid.EndInit();
        }
        #endregion

        #region GetGrid()
        private Grid GetGrid()
        {
            var grid = this.Parent as Grid;

            if (grid == null)
            {
                throw new InvalidOperationException(
                    "SimpleGridSplitter only works when hosted in a Grid.");
            }
            return grid;
        }
        #endregion
    }
}