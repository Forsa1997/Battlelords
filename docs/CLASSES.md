# MVP-Klassen

Zwei bewusst gegensätzliche Klassen: ein Fernkampf-Caster mit
Cast-Zeiten (verwundbar gegen Interrupts) und ein mobiler Nahkämpfer.
Eine Klasse ist technisch nur ein `ClassDefinition`-Asset mit 4
`AbilityDefinition`-Assets (Slots = Tasten 1–4).

Anlegen im Editor: *Create → Battlelords → Ability* bzw. *→ Class*,
Werte aus den Tabellen eintragen, Class-Asset beim `AbilityCaster`
in `Class Definition` ziehen. Alle Werte sind Startpunkte fürs
Balancing — im Playtest anpassen.

## Sturmwirker (Fernkampf-Caster)

Spielgefühl: hoher Schaden auf Distanz, muss für den großen Burst
stehen bleiben und casten — Gegner können das unterbrechen.

| Slot | Name | Cast Type | Werte |
|---|---|---|---|
| 1 | Sturmpfeil | Projectile | Damage 120, Cooldown 1.5, Cost 12, Cast Time 0, Speed 30, Range 30 |
| 2 | Blitzschlag | GroundTelegraph | Damage 350, Cooldown 8, Cost 30, **Cast Time 1.0**, Range 20, Radius 4, Warn Duration 1.2 |
| 3 | Arkanbarriere | SelfShield | Cooldown 14, Cost 25, Shield 300, Duration 4 |
| 4 | Störimpuls | Interrupt | Damage 50, Cooldown 12, Cost 15, Range 20, Lockout 2.0 |

## Klingentänzer (Nahkämpfer)

Spielgefühl: muss Distanz überbrücken, dafür alles instant und hohe
Mobilität. Gewinnt, wenn er am Gegner klebt.

| Slot | Name | Cast Type | Werte |
|---|---|---|---|
| 1 | Klingenwirbel | MeleeCone | Damage 160, Cooldown 1.5, Cost 12, Range 4, Angle 100 |
| 2 | Vorstoß | Dash | Cooldown 8, Cost 20, Distance 10 |
| 3 | Erdspalter | GroundTelegraph | Damage 250, Cooldown 10, Cost 25, Range 8, Radius 3, Warn Duration 0.8 |
| 4 | Fußfeger | Interrupt | Damage 80, Cooldown 12, Cost 15, **Range 5**, Lockout 2.0 |

## Design-Regeln fürs Balancing

- Interrupts lohnen sich nur gegen Cast-Zeit-Abilities; der Sturmwirker
  hat genau eine (Blitzschlag) — das ist das zentrale Mindgame.
- Kosten/Cooldown einer gecasteten Ability fallen erst bei Abschluss an;
  ein Interrupt kostet das Opfer also nur Zeit plus die Lockout-Dauer.
- Telegraph-Warndauer ≥ 0.8 s, damit Dodge-Roll (0.35 s) reaktiv möglich
  bleibt.
- Time-to-Kill-Ziel im 2v2: 15–30 s pro Kill, damit Fokus-Wechsel und
  Peeling eine Rolle spielen.

## Interrupt solo testen

Der Trainings-Dummy castet nicht — das Lockout-Gefühl lässt sich solo
nur indirekt prüfen. Vollständig testbar wird der Interrupt im
2-Spieler-Test nach Phase 3 (Netzwerk).
