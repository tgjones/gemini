# Gemini package-only compatibility host

This is durable package-validation infrastructure, not a transient migration
probe. Project-reference builds cannot prove that a packed `GeminiWpf` package
contains usable assets and dependency groups. `eng\validate-artifacts.ps1`
therefore restores this host from the locally packed package family, then builds
and runs it for every retained target framework.

The project intentionally has no project reference to Gemini. Its package
version is supplied by the validator after the coherent family has been
inspected. This verifies the consumer boundary that a normal solution build
cannot cover:

- NuGet selects Gemini and the complete external dependency graph declared for
  each target in `eng\package-contract.json`;
- the validator rejects framework fallback by requiring the exact native
  compile and runtime assets declared by that contract;
- an external extension compiles against the public lifecycle event contract;
  and
- representative framework, docking, serialization, and theme APIs load and
  execute from the resolved package graph.

The host can be removed only when another package-only consumer fixture covers
the same restore, compile, public-contract, dependency-graph, and runtime
assembly checks.
