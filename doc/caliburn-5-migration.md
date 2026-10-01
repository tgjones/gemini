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

## Release policy

This package family starts the Gemini 1.1 beta line. Removing net6/net7 and
changing an inherited public lifecycle event are breaking package-contract
changes. The previous 1.0 beta four-target family remains the rollback path.
