# Caliburn 5 package-only compatibility host

This is durable package-validation infrastructure, not a transient migration
probe. Project-reference builds cannot prove that a packed `GeminiWpf` package
contains usable assets and dependency groups. `eng\validate-artifacts.ps1`
therefore restores this host from the locally packed package family, then builds
and runs it for every retained target framework.

The project intentionally has no project reference to Gemini. Its package
version is supplied by the validator after the coherent family has been
inspected. This verifies the consumer boundary that a normal solution build
cannot cover:

- NuGet selects Gemini and the expected Caliburn.Micro Platform, Core, and
  Microsoft.Xaml.Behaviors.Wpf packages for each target;
- an external extension compiles against the Caliburn 5 asynchronous
  `Activated` event contract; and
- the expected Gemini, Caliburn Core, and Caliburn Platform assemblies load at
  runtime.

The host can be removed only when another package-only consumer fixture covers
the same restore, compile, public-contract, dependency-graph, and runtime
assembly checks.
