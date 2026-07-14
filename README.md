# Battlelords

2vs2 Arena-Kampfspiel, Third Person, mit actionbasierter Kampfsteuerung im Stil von
WildStar (freies Zielen, Strafing, Dodge-Roll mit i-Frames, sichtbare Boden-Telegraphs
statt reiner Tab-Target-Autoattacke).

- Engine: Unity (LTS) + URP
- Netzwerk: Photon Fusion (server-autoritativ, Client-Prediction)
- Ziel: spielbarer MVP für interne Tests mit 4 Spielern (2v2)

## Setup

Siehe [docs/PROJECT_SETUP.md](docs/PROJECT_SETUP.md) für die Schritte, um das
Unity-Projekt lokal aus diesem Repo zu öffnen.

## Roadmap

Der vollständige Phasenplan steht in [docs/ROADMAP.md](docs/ROADMAP.md).

## Projektstruktur

```
Assets/_Project/
  Scripts/
    Player/       Movement, Kamera-Anbindung
    Combat/        Abilities, Telegraphs, Health/Resource
    Networking/    Photon-Fusion-Anbindung, NetworkBehaviours
    UI/            HUD, Menüs
  Prefabs/
  Scenes/
  Art/
  Audio/
```
