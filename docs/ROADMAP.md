# MVP-Roadmap

Ziel: spielbarer 2v2-Arena-MVP zum internen Testen mit 4 Spielern. Kein
Progression-System, keine Kosmetik, kein eigenes Matchmaking-Backend.

Engine: Unity (LTS) + URP. Netzwerk: Photon Fusion (server-autoritativ,
eingebaute Client-Prediction/Reconciliation, eingebautes Relay für Tests
über das Internet ohne Portfreigabe).

## Phase 0 – Setup (3–5 Tage)
- Unity-Projekt mit URP-Template anlegen
- Photon Fusion SDK einbinden, App-ID einrichten
- Unity "Third Person Controller" Starter Assets als Referenz/Basis
- Scope fix: 1 Arena, 2 Klassen, 2v2, keine Meta-Systeme

## Phase 1 – Movement & Kamera (1–2 Wochen)
- Charakter-Controller: Strafe, Sprint, Dodge-Roll mit i-Frames
- Cinemachine-Third-Person-Kamera, freies Maus-Look
- Erst lokal ohne Netzwerk testen, bis sich die Bewegung gut anfühlt

## Phase 2 – Combat-Kern (2–3 Wochen)
- Health/Resource-System
- 3–4 Skills pro Klasse: Skillshot, Boden-Telegraph-AoE, Gap-Closer,
  Defensive/Interrupt
- Telegraph-Visualisierung (Warnung → Zündung)
- Server-autoritative Trefferauswertung

## Phase 3 – Netzwerk (3–4 Wochen, kritischster Teil)
- Server-autoritativ, Client-Prediction + Reconciliation für Bewegung
- Fähigkeiten/Telegraphs für alle 4 Spieler synchron replizieren
- Session-Join per Raum-Code (Photon), kein eigenes Matchmaking-Backend

## Phase 4 – Arena & Matchflow (1 Woche)
- Eine kleine, symmetrische Blockout-Arena
- 2v2-Spawns, Rundentimer, Win-Condition, Rematch

## Phase 5 – Minimal-UI (3–5 Tage)
- HUD: HP/Resource, Cooldowns, Timer, Killfeed
- Menü: Host/Join per Code, Klassenwahl

## Phase 6 – Playtest-Loop (laufend)
- Mit 3 Freunden über echtes Internet testen (nicht nur localhost)
- Iterieren auf Movement-Feel und Ability-Balance

Geschätzte Gesamtzeit solo/Teilzeit: ~10–14 Wochen bis spielbarer MVP.

## Bewusst nicht im MVP
- Weitere Klassen, Progression/Ranking, eigenes Matchmaking-Backend,
  Voice-Chat, Anti-Cheat, finale Art/Animationen
