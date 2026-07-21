# Warhammer Fantasy Battle — 6th Edition, PC

A turn-based PC adaptation of Warhammer Fantasy Battle 6th edition (2000). Units move
as ranked blocks under the tabletop movement rules; combat is resolved by dice and
then played back as animation.

**Personal use only. Not for distribution.**

## Status

Pre-alpha. Rules core scaffolding only — no engine layer yet. Builds clean,
35 tests passing (geometry, arcs, wheeling, RNG determinism and uniformity).

Stat lines and points values are **placeholders** until the 6th ed rulebook,
Dwarfs (2000) and High Elves (2002) army books are available to encode from.
See [docs/rules-to-verify.md](docs/rules-to-verify.md) for everything currently
resting on recall rather than a source.

## Architecture

Two layers, hard split — see [docs/architecture.md](docs/architecture.md).

```
Warhammer.Rules   pure C# class library, no engine dependency, deterministic
       ↓ command → result log
Godot 4 (.NET)    presentation only: renders state, plays back the log
```

The rules core never references Godot and can be exercised entirely from tests.

## Layout

```
src/Warhammer.Rules/        engine-agnostic rules engine
  Core/                     dice, RNG, stat profiles
  Geometry/                 facing, footprints, arcs, wheeling
tests/Warhammer.Rules.Tests/
docs/
```

## Prerequisites

- **.NET SDK 10** (10.0.302 verified)
- **Godot 4.7.1, .NET build** — needed only once presentation work starts

Godot 4.7 targets `net8.0` with `rollForward: LatestMajor`, so it runs on the
.NET 10 runtime. A separate .NET 8 runtime install is **not** required.

## Target frameworks

| Project | TFM | Why |
|---|---|---|
| `Warhammer.Rules` | `net8.0` | matches what Godot 4.7's project template generates |
| `Warhammer.Rules.Tests` | `net10.0` | the runtime actually installed; net10 referencing net8 is fine |

## Build

```
dotnet build
dotnet test
```

## Scope — first playable

Dwarfs vs High Elves, ~750 points, no magic phase (Dwarfs have no wizards; the
High Elf list omits a mage). Exercises movement and wheeling, declared charges,
close combat with ranks and flanks, shooting, break tests, and pursuit.

Deferred: magic, monsters, chariots, fliers, magic items, multiplayer.
