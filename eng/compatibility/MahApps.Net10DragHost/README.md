# MahApps .NET 10 drag compatibility host

This small WPF executable records the reason Gemini updated MahApps.Metro from
2.4.9 to 2.4.11. It is compatibility evidence for one vendor/runtime defect,
not a Gemini application target, sample application, general UI test project,
or commitment to maintain a separate test architecture.

## Provenance

Gemini's main window is a `MahApps.Metro.Controls.MetroWindow`. MahApps.Metro
2.4.9 implements title dragging by reflecting WPF's private
`Window.CriticalHandle` property and using that value for the native
non-client drag message. That private member is not available under .NET 10,
so a MetroWindow can start normally while title dragging does nothing.

[MahApps.Metro issue #4554](https://github.com/MahApps/MahApps.Metro/issues/4554)
tracks the .NET 10 failure. MahApps.Metro 2.4.11 replaces the private reflection
with `WindowInteropHelper.EnsureHandle()` in the same drag path. The package
update in this branch deliberately does not include MahApps 3, a ControlzEx
major update, a Gemini library target change, or a broader shell redesign.

The host targets `net10.0-windows` and constructs a real `MetroWindow` with
normal, maximized, minimized, caption-button, resize, and glow behavior visible.
Its `MahAppsVersion` MSBuild property defaults to 2.4.11 so the same source and
runtime can also reproduce the 2.4.9 behavior without maintaining two fixtures.

## Running the comparison

From the repository root:

```powershell
dotnet run --project eng\compatibility\MahApps.Net10DragHost\MahApps.Net10DragHost.csproj `
  --configuration Release `
  -p:MahAppsVersion=2.4.9
```

Verify that dragging the normal window by its title bar does not move it, then
maximize it and verify that dragging the title bar does not restore and move it.

Run the identical host with the patched package:

```powershell
dotnet run --project eng\compatibility\MahApps.Net10DragHost\MahApps.Net10DragHost.csproj `
  --configuration Release `
  -p:MahAppsVersion=2.4.11
```

Repeat both drag checks. The expected result is that normal title dragging and
maximized drag-to-restore both work. The host displays its requested package
version, loaded MahApps assembly version, runtime, window state, and observed
location-change count so results are attributable to the intended comparison.

For a noninteractive startup check:

```powershell
dotnet run --project eng\compatibility\MahApps.Net10DragHost\MahApps.Net10DragHost.csproj `
  --configuration Release `
  -- `
  --smoke
```

Smoke mode verifies that the MetroWindow starts and obtains a native handle. It
does not claim that title dragging passed; the interactive checks above remain
separate evidence.

## Lifetime and removal

Do not remove the host merely because the MahApps 2.4.11 package update merges.
It remains useful while subsequent .NET 10 targeting work needs to distinguish
the already-fixed window-chrome prerequisite from new Gemini regressions.

The host has served its purpose and can be removed in a focused cleanup when
all of the following are true:

1. MahApps.Metro 2.4.11 or a later version containing the same handle fix is the
   established minimum on every actively maintained Gemini line that exercises
   a .NET 10 host.
2. The planned .NET 10 host/target work has completed its normal and maximized
   title-drag validation against Gemini's actual main window.
3. Equivalent regression coverage exists in a durable Gemini WPF
   integration/UI fixture, or maintainers explicitly decide that the accepted
   upstream fix and recorded PR evidence are sufficient.
4. No open dependent branch still uses this project to separate the MahApps
   prerequisite from its own changes.

Removal should delete this directory and its `Compatibility` solution entry
together. If Gemini stops using `MetroWindow`, the host can also be removed as
obsolete after the replacement window path has its own coverage.
