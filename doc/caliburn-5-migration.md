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

## Release policy

This package family starts the Gemini 1.1 beta line. Removing net6/net7 and
changing an inherited public lifecycle event are breaking package-contract
changes. The previous 1.0 beta four-target family remains the rollback path.
