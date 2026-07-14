# Testszene zusammenbauen (Phase 1/2)

Anleitung, um mit den vorhandenen Skripten eine lokale Testszene ohne
Netzwerk zu bauen: Movement + Dodge, Skillshot, Boden-Telegraph, Dummy.
Dauert ca. 15 Minuten.

> **Wichtig:** Die Prototyp-Skripte nutzen noch den klassischen Input
> Manager (`Input.GetAxis` etc.). In *Project Settings → Player → Active
> Input Handling* auf **"Both"** stellen, falls das Input-System-Paket
> installiert ist.

## 1. Arena-Blockout

1. Neue Szene in `Assets/_Project/Scenes/` anlegen, z.B. `TestArena`.
2. Boden: Plane, Scale (5, 1, 5), auf Position (0, 0, 0).
3. Ein paar Cubes als Wände/Deckung verteilen (für Kamera-Kollision und
   Projectile-Blocking).

## 2. Spieler

1. Capsule anlegen, Name `Player`, Position (0, 1, 0).
2. Den **Capsule Collider entfernen** und stattdessen einen
   **Character Controller** hinzufügen (Center Y = 0, Height = 2).
3. Komponenten hinzufügen:
   - `ThirdPersonController` (Player)
   - `Health` — Team auf **TeamA** stellen
   - `AbilityCaster`
4. Es reicht, `AbilityCaster → Aim Camera` leer zu lassen (nimmt
   `Camera.main`).

## 3. Kamera

1. An der Main Camera die Komponente `OrbitCamera` hinzufügen.
2. `Target` auf den `Player` ziehen.
3. Beim `ThirdPersonController` des Players `Camera Transform` auf die
   Main Camera ziehen.

## 4. Abilities anlegen

> Alternativ: gleich die beiden MVP-Klassen aus
> [CLASSES.md](CLASSES.md) als Assets anlegen und das Class-Asset in
> `AbilityCaster → Class Definition` ziehen — das überschreibt die
> Slots automatisch.

1. Rechtsklick in `Assets/_Project/` → *Create → Battlelords → Ability*.
2. Zwei Assets anlegen:
   - **Feuerball**: Cast Type `Projectile`, Damage 150, Cooldown 2,
     Resource Cost 15.
   - **Einschlag**: Cast Type `GroundTelegraph`, Damage 300, Cooldown 6,
     Resource Cost 30, Warn Duration 1.2.
   - Optional **Sprung**: Cast Type `Dash`, Cooldown 8, Resource Cost 20.
3. Die Assets beim Player in `AbilityCaster → Abilities` in die Slots
   ziehen (Slot 0 = Taste 1, Slot 1 = Taste 2, …).

Prefabs für Projektil/Telegraph sind optional — ohne Prefab erzeugen die
Skripte automatisch Debug-Primitives.

## 5. Trainings-Dummy

1. Capsule anlegen, Name `Dummy`, ein Stück vor dem Spieler platzieren.
2. Komponenten: `Health` (Team **TeamB**) und `TrainingDummy`.
3. Der vorhandene Capsule Collider bleibt (wird für Treffer gebraucht).

## 6. Debug-HUD

1. Leeres GameObject `DebugHud` anlegen, Komponente `DebugHud` hinzufügen.
2. `Player Health` und `Player Caster` auf den Player ziehen.

## 7. Testen

Play drücken:

- **WASD** bewegen, **Shift** sprinten, **Space** Dodge-Roll (i-Frames)
- **Maus** Kamera, **1/2/3/4** Abilities
- Feuerball trifft, was im Fadenkreuz ist; Einschlag zündet nach der
  Warnzeit am anvisierten Bodenpunkt — Dummy blinkt rot bei Treffern
  und respawnt nach dem Tod automatisch.

Damit lässt sich das Kern-Gefühl (Zielen, Telegraph ausweichen per
Dodge) bereits solo gegen den Dummy prüfen, bevor Netzwerk dazukommt.

## 8. Optional: Match-Flow testen (Phase 4)

1. Leeres GameObject `Match` anlegen mit den Komponenten `MatchManager`
   und `MatchHud`.
2. Pro Team mindestens einen Spawnpunkt: leeres GameObject mit
   `SpawnPoint`-Komponente, Team einstellen, in der Arena platzieren
   (Blickrichtung = Spawn-Ausrichtung; Gizmo zeigt sie an).
3. Wichtig: Für Match-Tests am Dummy die `TrainingDummy`-Komponente
   **entfernen** (nur `Health` behalten) — ihr Auto-Respawn würde sonst
   mit dem Runden-Reset kollidieren.

Ablauf: Countdown → Runde → Team tot oder Zeit abgelaufen → Reset an
den Spawnpunkten. Erster mit 3 Rundensiegen gewinnt, `R` startet ein
Rematch. Solo killst du einfach den Dummy, um den Flow zu sehen.
