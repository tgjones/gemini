# Caliburn Micro 5 migration

Gemini's Caliburn Micro 5 package line targets `net462` and
`net10.0-windows`. Applications that require the previous net6/net7 assets
must remain on the earlier Gemini package line.

Caliburn.Micro 5.0.258 does not publish a native net6 or net7 WPF Platform
asset group. NuGet can select its net462 Platform fallback for those targets,
but reports `NU1701`; the Core package's netstandard2.0 asset does not replace
the WPF Platform assembly. Gemini therefore does not publish new supported
net6/net7 targets on that warning-bearing fallback.

## Package and extension compatibility

The package family uses Caliburn.Micro 5.0.258. Extensions should reference the
same released Caliburn package family and must be rebuilt before loading into
this Gemini line.

Caliburn 5 changes the inherited `IActivate.Activated` event from
`EventHandler<ActivationEventArgs>` to
`AsyncEventHandler<ActivationEventArgs>`. Source handlers must return `Task`,
for example:

```csharp
viewModel.Activated += async (sender, args) =>
{
    await InitializeForActivationAsync();
};
```

An extension compiled against the Caliburn 4 event accessor is not considered
binary compatible merely because Gemini still provides a `net462` asset.
Rebuild and test extensions against the new package before deployment.

Caliburn 5 also replaces the obsolete `OnInitializeAsync` extension point with
`OnInitializedAsync`. The new hook runs after `IsInitialized` becomes `true`.
Code that retries initialization after a failure must recreate the failed
screen rather than changing Caliburn's lifecycle flags.

## Asynchronous shell and startup readiness

`IShell.ShowTool` has been replaced by the awaitable `ShowToolAsync` overloads,
and parentless docking tools route close requests through `CloseToolAsync`.
Document open, document close, tool show/close, and active-layout binding
transitions are serialized in arrival order. Callers should await these methods
so lifecycle failures are observed; a failed transition does not prevent later
queued transitions from running. The shell evaluates a tool guard once, closes
the tool without unregistering it, and preserves a vetoed tool as visible and
active.

Gemini tools are registered by the shell rather than parented by a Caliburn
conductor. `Tool.TryCloseAsync` therefore routes through the shell-owned close
operation, which awaits the guard once and updates visibility and selection
only after successful deactivation. The tool remains registered for reopening.

`IShell.InitializationTask` represents view-dependent shell readiness. It
completes after global resources, module pre-initialization and initialization,
theme selection, layout restore or default item creation, and every module's
`PostInitializeAsync` have completed. It faults when any of those startup steps
fails. `IMainWindow.ShellActivationTask` similarly exposes activation of the
shell conductor. The default `AppBootstrapper` awaits root display, shell
activation, and shell initialization in that order and treats a failure as a
fatal startup error.

## Close guards and lifecycle state

Dirty document outcomes remain distinct. **Save** closes only after the save
completes; a canceled Save As remains a veto. **Discard** closes without
clearing the dirty state first, while **Cancel** leaves the document active.
Save faults and task cancellation remain observable to the caller. Undo history
is retained when close is vetoed or saving does not complete, and is disposed
when the document closes.

Shell state is saved only for an actual `close=true` deactivation. Ordinary
deactivation neither latches the shutdown guard nor writes state, so the shell
can reactivate and accept later document activation. A failed close clears the
shutdown guard; successful close keeps it latched for the last-nonactive-
document AvalonDock workaround. Partial or failed transactional state saves
continue through the existing warning hook.

Applications that add an exit confirmation after deriving from the shell must
await `base.CanCloseAsync` first. Only after every child guard allows close
should application confirmation run. Coroutine bridges should complete task
continuations asynchronously, preserve user cancellation, and surface
`ResultCompletionEventArgs.Error` as a task fault.

## Persisted layout recovery

Layout loading now returns an awaitable, explicit outcome instead of a Boolean.
`NotFound` opens the module defaults. `Failed` and `Corrupt` preserve the
unreadable state file in place, report the failure through the shell warning
hook, and then open defaults without making startup fatal. `Partial` reports the
skipped item, retains every usable document and tool, and does not replace the
recovered layout with defaults. Missing plug-in types and invalid item payloads
are partial recovery; malformed envelopes and AvalonDock deserialization errors
are corrupt recovery.

`ILayoutItem.LoadState(BinaryReader)` and AvalonDock's serialization callbacks
remain synchronous and stream-scoped. Layout items that need asynchronous
content loading can additionally implement `IAsyncLayoutItemStateRestorer`;
that work runs after parsing and before restored items become ready.

Shell readiness now waits for asynchronous item content restoration, ordered
document presentation, visible-tool activation, and final persisted selection
before module `PostInitializeAsync` and `IShell.InitializationTask` complete.
Hidden tools are registered without activation; visible tools are activated
once. Serializer callbacks never start background work and never outlive the
state stream.

Layout saves are transactional. The complete envelope and AvalonDock layout are
written and flushed to `<state-file>.tmp` in the destination directory before
the destination is replaced or moved. When a destination already exists, its
previous contents are copied to the deterministic `<state-file>.bak` rollback
file before replacement. A file-level failure leaves the destination intact.
An individual item state failure remains a partial save with a skippable
zero-length payload.

## Truthful editor opening

`IEditorOpeningService` is the shared MEF service for opening existing files and
creating new editor documents. Its task completes only after both shell
presentation and the provider's `Open` or `New` task complete. Views that are
already loaded are recognized immediately.

Callers must await the returned task. Shell and provider faults or cancellation
remain observable. `OpenFileAsync` returns a faulted, non-null task with
`NotSupportedException` when no editor provider handles the path. Path-based
`OpenDocumentResult` operations use the same service.

## Coroutine result completion

Each open/show coroutine result execution publishes at most one terminal
outcome, even when task completion and close events race. Completion returns to
the captured synchronization context. Successful completion has no error,
canceled tasks set `WasCancelled`, and faulted tasks preserve the original
exception. Synchronous location and configuration failures follow the same
completion path.

The result lifetimes remain intentionally distinct:

- `OpenDocumentResult` completes after document presentation and editor opening
  finish. Its one-shot close handler remains only to invoke `OnShutDown`.
- `ShowToolResult` and `ShowWindowResult` complete when the shown item closes.
  A faulted or canceled show task completes them immediately instead.
- `ShowDialogResult` completes when the dialog task returns. `false` and `null`
  are user cancellation; a canceled task is cancellation, while a faulted task
  remains an error.

Exceptions from `OnConfigure` are result errors. `OnShutDown` errors are result
errors while the result is awaiting close or dialog return; for a document whose
result has already completed, they fault the later close lifecycle task.

## Retained MEF and Inspector conventions

MEF priority catalogs remain ahead of main catalogs so the first single-service
resolution is the application override. Document exports that represent a new
editor instance retain `PartCreationPolicy(CreationPolicy.NonShared)`, and
missing required imports remain visible as composition failures rather than
being converted into fallback instances.

## Release policy

This package family starts the Gemini 1.1 beta line. Removing net6/net7 and
changing an inherited public lifecycle event are breaking package-contract
changes. The previous 1.0 beta four-target family remains the rollback path.
