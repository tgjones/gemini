using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Gemini.Framework;

namespace Gemini.Modules.Shell.Services
{
    /// <summary>Describes whether persisted layout state can be used.</summary>
    public enum LayoutItemStateLoadStatus
    {
        /// <summary>No persisted state exists; module defaults should be opened.</summary>
        NotFound,
        /// <summary>All persisted state was restored.</summary>
        Success,
        /// <summary>Usable state was restored after one or more items were skipped.</summary>
        Partial,
        /// <summary>The state file could not be opened or otherwise read.</summary>
        Failed,
        /// <summary>The state envelope or AvalonDock layout could not be parsed.</summary>
        Corrupt
    }

    /// <summary>Describes the outcome of a transactional layout-state write.</summary>
    public enum LayoutItemStateSaveStatus
    {
        /// <summary>The complete destination was committed.</summary>
        Success,
        /// <summary>The destination was committed with one or more omitted item payloads.</summary>
        Partial,
        /// <summary>The destination was not replaced.</summary>
        Failed
    }

    /// <summary>Identifies restored items that require post-parse shell lifecycle work.</summary>
    public sealed class LayoutItemStateRestorePlan
    {
        public static readonly LayoutItemStateRestorePlan Empty = new LayoutItemStateRestorePlan(
            new IDocument[0],
            new ITool[0],
            null);

        public LayoutItemStateRestorePlan(
            IEnumerable<IDocument> documents,
            IEnumerable<ITool> visibleTools,
            ILayoutItem selectedItem)
        {
            if (documents == null)
                throw new ArgumentNullException(nameof(documents));
            if (visibleTools == null)
                throw new ArgumentNullException(nameof(visibleTools));

            Documents = new ReadOnlyCollection<IDocument>(new List<IDocument>(documents));
            VisibleTools = new ReadOnlyCollection<ITool>(new List<ITool>(visibleTools));
            SelectedItem = selectedItem;
        }

        public IReadOnlyList<IDocument> Documents { get; }

        public IReadOnlyList<ITool> VisibleTools { get; }

        public ILayoutItem SelectedItem { get; }
    }

    /// <summary>Returns the usable restore plan and any recovery details.</summary>
    public sealed class LayoutItemStateLoadResult
    {
        private LayoutItemStateLoadResult(
            LayoutItemStateLoadStatus status,
            LayoutItemStateRestorePlan restorePlan,
            string details,
            Exception exception)
        {
            Status = status;
            RestorePlan = restorePlan ?? LayoutItemStateRestorePlan.Empty;
            Details = details;
            Exception = exception;
        }

        public LayoutItemStateLoadStatus Status { get; }

        public LayoutItemStateRestorePlan RestorePlan { get; }

        public string Details { get; }

        public Exception Exception { get; }

        public static LayoutItemStateLoadResult NotFound(string details = null)
        {
            return new LayoutItemStateLoadResult(
                LayoutItemStateLoadStatus.NotFound,
                LayoutItemStateRestorePlan.Empty,
                details,
                null);
        }

        public static LayoutItemStateLoadResult Success(
            LayoutItemStateRestorePlan restorePlan = null)
        {
            return new LayoutItemStateLoadResult(
                LayoutItemStateLoadStatus.Success,
                restorePlan,
                null,
                null);
        }

        public static LayoutItemStateLoadResult Partial(
            LayoutItemStateRestorePlan restorePlan,
            string details,
            Exception exception = null)
        {
            return new LayoutItemStateLoadResult(
                LayoutItemStateLoadStatus.Partial,
                restorePlan,
                details,
                exception);
        }

        public static LayoutItemStateLoadResult Failed(
            Exception exception,
            string details = null)
        {
            if (exception == null)
                throw new ArgumentNullException(nameof(exception));

            return new LayoutItemStateLoadResult(
                LayoutItemStateLoadStatus.Failed,
                LayoutItemStateRestorePlan.Empty,
                details,
                exception);
        }

        public static LayoutItemStateLoadResult Corrupt(
            Exception exception,
            string details = null)
        {
            if (exception == null)
                throw new ArgumentNullException(nameof(exception));

            return new LayoutItemStateLoadResult(
                LayoutItemStateLoadStatus.Corrupt,
                LayoutItemStateRestorePlan.Empty,
                details,
                exception);
        }
    }

    /// <summary>Returns the transactional save outcome and any omitted-item details.</summary>
    public sealed class LayoutItemStateSaveResult
    {
        private LayoutItemStateSaveResult(
            LayoutItemStateSaveStatus status,
            string details,
            Exception exception)
        {
            Status = status;
            Details = details;
            Exception = exception;
        }

        public LayoutItemStateSaveStatus Status { get; }

        public string Details { get; }

        public Exception Exception { get; }

        public static LayoutItemStateSaveResult Success()
        {
            return new LayoutItemStateSaveResult(
                LayoutItemStateSaveStatus.Success,
                null,
                null);
        }

        public static LayoutItemStateSaveResult Partial(
            string details,
            Exception exception = null)
        {
            return new LayoutItemStateSaveResult(
                LayoutItemStateSaveStatus.Partial,
                details,
                exception);
        }

        public static LayoutItemStateSaveResult Failed(
            Exception exception,
            string details = null)
        {
            if (exception == null)
                throw new ArgumentNullException(nameof(exception));

            return new LayoutItemStateSaveResult(
                LayoutItemStateSaveStatus.Failed,
                details,
                exception);
        }
    }
}
