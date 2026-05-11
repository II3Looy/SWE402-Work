# Knockout Arena — Creative Core Polish (Assignment 3)

This document covers each of the six required sections plus the written report.
For every section it lists: **what was implemented**, **which GameObject(s) /
script(s) it lives on**, and **how to see it in action while playing**.

> Engine: Unity 6 (6000.4.3f1), URP. Scene: `Assets/Scenes/Prototype 4.unity`.
> All new scripts live under `Assets/Scripts/`.

---

## Section 1 — Shaders and Materials

**What:** Runtime material control on enemies via `MaterialPropertyBlock`
(no shared-material edits, no extra material assets needed). Enemies are
tinted progressively redder as the wave number grows, and they **flash white
for ~0.12s** when the powered-up player slams into them.

**Where:**
- `Assets/Scripts/EnemyVisuals.cs` — drives `_BaseColor` (URP/Lit) and `_Color`
  (legacy Standard) via a `MaterialPropertyBlock`.
- Subscribed to `GameManager.OnWaveChanged` for the tint, and called from
  `PlayerController.OnCollisionEnter` for the hit-flash.

**How to see it:** Start the game and survive several waves — enemies become
visibly more red. Pick up the powerup and ram an enemy — it flashes white on
impact.

---

## Section 2 — Lighting

**What:** Two scripted lighting behaviors that work with any Light component:
- `WaveLightIntensity` — smoothly raises the Directional Light's intensity and
  shifts its color slightly warmer as the wave number rises, then resets when a
  new game starts.
- `LightFlicker` — Perlin-noise-driven flicker for atmospheric Point/Spot
  lights (e.g., arena-edge lamps or a beacon at the powerup spawn).

**Where:**
- `Assets/Scripts/WaveLightIntensity.cs` (attach to the Directional Light).
- `Assets/Scripts/LightFlicker.cs` (attach to any Point/Spot Light).
- Both subscribe to `GameManager` events; no external wiring needed beyond
  putting the component on a Light.

**How to see it:** Play and progress through waves — the directional light
slowly brightens and warms. With a flickering point light added at the arena
edge, you'll see a gentle, natural intensity variation.

---

## Section 3 — Animation

**What:** Two improvements beyond the original worksheet's powerup animator:
- **Score punch (UI animation from code):** When `UpdateScore` runs, the score
  text scales up to 1.3x and lerps back to 1.0 over 0.2s.
- **Wave-driven event broadcasting:** `GameManager.UpdateWaveText` now also
  raises `OnWaveChanged`, so wave changes can trigger animation/visual
  responses across the whole game (see Sections 1, 2, 5, 6).

**Where:**
- `Assets/Scripts/GameManager.cs` — `ScorePunch` coroutine + `OnWaveChanged`
  event.

**How to see it:** Knock an enemy off — the score number visibly pulses.
Advance through waves — every system tuned to `OnWaveChanged` reacts in sync.

---

## Section 4 — VFX

**What:** Two additional ParticleSystem-driven effects on top of the existing
`KnockoutEffect`, both controlled from code with `.Play()` / `.Stop()`:
- **Player trail** (`PlayerTrailVFX`) — Plays a looping particle trail on the
  player while moving (`rb.linearVelocity` over a threshold) and stops when the
  player is idle or the game ends.
- **Powerup pickup burst** (`PickupVFX`) — A static `PickupVFX.PlayAt(position)`
  helper spawns a one-shot burst particle prefab at the pickup location, plays
  it, and destroys it after its lifetime.

**Where:**
- `Assets/Scripts/PlayerTrailVFX.cs` (attach to Player; drag in trail PS).
- `Assets/Scripts/PickupVFX.cs` (attach to a scene manager GameObject; drag in
  burst PS prefab).
- `PlayerController.OnTriggerEnter` calls `PickupVFX.PlayAt(...)` on collect.

**How to see it:** Move the player — the trail particle plays as long as
you're moving. Grab a powerup — a burst plays at the pickup location.

---

## Section 5 — Cameras

**What:** Two camera enhancements beyond the basic focal-point orbit:
- **Camera shake** (`CameraShake`) — Singleton with a `Shake(duration, magnitude)`
  API. Subscribes to `OnPlayerHitEnemy` (small jolt) and `OnGameOver` (bigger
  shake). Falls off smoothly back to the camera's rest position.
- **Dynamic FOV** (`CameraEffects`) — Smoothly widens the camera FOV by ~8°
  while the powerup is active (a "feels fast" pulse), and adds a small
  per-wave FOV bump so the arena visibly pulls out as waves progress.

**Where:**
- `Assets/Scripts/CameraShake.cs` (attach to Main Camera).
- `Assets/Scripts/CameraEffects.cs` (attach to Main Camera; auto-requires `Camera`).

**How to see it:** Pick up the powerup and ram an enemy — the camera jolts and
the FOV widens. Wait for game over — a stronger one-time shake. Survive
through several waves — the camera gradually pulls back.

---

## Section 6 — Post-Processing

**What:** A URP Global Volume drives Bloom, Vignette, and Chromatic Aberration
based on gameplay state via `PostFXController`:
- While the **powerup** is active: Bloom and Chromatic Aberration ramp up,
  Vignette pulses slightly.
- On **game over**: Vignette intensifies for a darkened, oppressive feel.
- On **new game**: state resets back to base values.
All transitions are smoothed with `Mathf.Lerp`.

**Where:**
- `Assets/Scripts/PostFXController.cs` (attach to the Global Volume GameObject).
- Uses `volume.profile.TryGet(out Bloom/Vignette/ChromaticAberration)` so it
  works with any URP volume profile that has those overrides enabled.
- The project's `Assets/Settings/DefaultVolumeProfile.asset` already includes
  Bloom, Vignette, and ChromaticAberration components.

**How to see it:** Pick up the powerup — the screen glows brighter (Bloom) and
the edges fringe slightly (Chromatic Aberration). Die — the edges darken
(Vignette).

---

## Section 7 — Written Report

This document **is** the written report. Per-section summaries above describe
what was added, where the implementation lives, and how to observe it in play
mode. Screenshots can be added to `Docs/` if required for submission.

---

## File map (new + modified)

**New scripts (`Assets/Scripts/`):**
- `CameraShake.cs`
- `CameraEffects.cs`
- `PostFXController.cs`
- `EnemyVisuals.cs`
- `PlayerTrailVFX.cs`
- `PickupVFX.cs`
- `LightFlicker.cs`
- `WaveLightIntensity.cs`

**Modified scripts:**
- `GameManager.cs` — added `OnGameStarted`, `OnWaveChanged`, `OnPowerupActivated`,
  `OnPowerupDeactivated`, `OnPlayerHitEnemy` events + raisers. Existing
  `ScorePunch` is now documented here as Section 3 work.
- `PlayerController.cs` — raises `OnPowerupActivated/Deactivated`,
  `OnPlayerHitEnemy`, calls `PickupVFX.PlayAt`, and calls `EnemyVisuals.Flash`
  on the enemy it hits while powered up.

---

## Setup checklist (Unity Editor work required)

The following actions must be done inside the Unity Editor — once these are
wired, every system above starts functioning together:

### Scene-level setup
1. **Main Camera**
   - Add component `CameraShake`.
   - Add component `CameraEffects`.
   - Enable **Post Processing** in the camera's Inspector (Rendering section).
2. **Global Volume**
   - In the Hierarchy: `GameObject > Volume > Global Volume`.
   - Set its Profile to `Assets/Settings/DefaultVolumeProfile.asset`.
   - In the profile, make sure **Bloom**, **Vignette**, and **Chromatic Aberration**
     overrides exist and their checkboxes (and the `intensity` checkboxes
     inside them) are enabled.
   - Add component `PostFXController` to the Global Volume GameObject.
3. **Directional Light**
   - Add component `WaveLightIntensity`. Confirm `baseIntensity` matches the
     light's starting intensity.
4. **Arena edge lights (optional but recommended for Section 2)**
   - Add 1–2 `Point Lights` near the edges of the arena (or one over the
     powerup spawn).
   - Add component `LightFlicker` to them.
5. **PickupVFX manager**
   - Create an empty GameObject named `VFX Manager` and add `PickupVFX`.

### Player setup
6. **Player GameObject**
   - Add component `PlayerTrailVFX`.
   - Create a child ParticleSystem (looping, small, colored to match player) and
     drag it onto the `trail` slot of `PlayerTrailVFX`.

### Enemy setup
7. **Enemy prefab (`Assets/Prefabs/Enemy.prefab`)**
   - Add component `EnemyVisuals`.
   - Adjust `baseColor` and `maxWaveColor` in the inspector if desired.

### Particle prefabs to create (any quick stylized burst works)
8. **Powerup pickup burst**
   - Create a Particle System prefab (e.g., yellow burst with ~30 particles,
     0.5s lifetime, radial velocity). Save as
     `Assets/Prefabs/PowerupPickupBurst.prefab`.
   - Drag it onto `PickupVFX.burstPrefab` on the VFX Manager.

### Optional but recommended
9. **Powerup material**
   - Duplicate the existing Powerup material, switch it to URP/Lit, enable
     **Emission**, pick a glowing color. With Bloom enabled in the Volume, the
     powerup will visibly glow — this satisfies Section 1's
     materials-with-emission requirement on top of the scripted tinting.

Once steps 1–8 are done, hit Play. You should see:
- Camera shake on impact / game over
- FOV pulse on powerup + slow zoom-out across waves
- Bloom + Chromatic Aberration spike during powerup
- Vignette darken on game over
- Enemies redder per wave, flashing white when hit
- Particle trail while moving
- Particle burst on pickup
- Directional light slowly warming and brightening per wave
- Optional flicker on edge point lights
