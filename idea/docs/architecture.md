# Architecture

## The split

Everything hinges on one decision: **the rules engine has no engine dependency.**

```
┌─────────────────────────────────────┐
│ Warhammer.Rules (.NET class lib)    │   pure, deterministic, headless
│   GameState + Command → ResultLog   │
└─────────────────────────────────────┘
                  ↕
┌─────────────────────────────────────┐
│ Godot 4 (.NET)                      │   renders state, plays back the log
└─────────────────────────────────────┘
```

`Warhammer.Rules` never references Godot, never touches the filesystem, never
reads a clock, and never calls `System.Random`. Given the same starting state,
the same seed, and the same command sequence, it produces byte-identical output
on any machine.

This buys three things at once:

1. **Testability.** The entire ruleset is exercised from `dotnet test` without
   launching a game. Charges, break tests and pursuit are verified as pure
   functions.
2. **Multiplayer for free.** Two clients running identical cores exchange
   *commands*, not state. A turn is a handful of bytes. No authoritative server,
   no state reconciliation.
3. **Replays and undo for free.** The command list *is* the save file. Replay it
   from the seed to reach any point in the game.

## Commands and the result log

The presentation layer never computes an outcome. It submits a command and
receives a log of what happened:

```
DeclareCharge(unit: 3, target: 7)
  → ChargeDeclared, ChargeRangeMeasured(9.5"), FearTestRequired,
    LeadershipTest(rolled [3,4] = 7 vs Ld 9, passed), ChargeSucceeded
```

Each entry carries the dice that produced it. The UI can surface them the way
Baldur's Gate 3 does — resolved instantly in the background, expandable if the
player wants to see the arithmetic.

## Determinism

`System.Random` is explicitly **not** stable across .NET versions, which would
silently break replays and desync multiplayer. `DeterministicRng` implements
xoshiro256\*\* with SplitMix64 seeding and Lemire rejection sampling for unbiased
dice. Its state is serialisable, so a game can be saved mid-turn and resumed
bit-identically.

## Units

Internal canonical unit is the **millimetre**, because base sizes (20mm, 25mm,
25×50mm) are the awkward numbers and movement values are not. Movement in inches
converts at the API edge: `1" = 25.4mm`.

Positions are continuous, not grid-snapped. Wheeling in 6th edition pivots on a
front corner through an arbitrary angle, so any grid would misrepresent legal
movement.

## Phase order (6th edition)

```
Movement → Magic → Shooting → Close Combat
```

Close combat is fought by Initiative order across *both* armies, which is why
combat resolution cannot be modelled as "active player acts."
