# Projekt lokal aufsetzen

Dieses Repo enthält die Ordnerstruktur, Skripte und Doku für Battlelords,
aber noch kein von Unity generiertes Projekt (`ProjectSettings/`,
`Packages/manifest.json`). Das muss einmalig lokal mit dem Unity-Editor
erzeugt werden, da diese Dateien editor-versionsabhängig sind.

## 1. Unity-Projekt erzeugen

1. Unity Hub installieren, Unity **6000.0 LTS** (oder aktuellste 2022 LTS,
   falls 6 noch nicht verfügbar) installieren.
2. Neues Projekt mit dem **URP (3D)**-Template anlegen, **im Root dieses
   Repos** (also dort, wo diese `docs/`-Datei liegt).
3. Unity fragt beim Anlegen ggf., ob vorhandene Dateien (README, .gitignore,
   Assets/_Project/...) übernommen werden sollen — bestätigen. Der von
   Unity generierte `Assets/Scenes`-Ordner mit der Default-Szene kann in
   `Assets/_Project/Scenes` verschoben oder gelöscht werden.

## 2. Pakete installieren (Window → Package Manager)

- **Cinemachine** (für die Third-Person-Kamera)
- **Input System** (neues Input-System statt des alten)
- **Photon Fusion SDK**: über den [Photon Dashboard](https://dashboard.photonengine.com)
  einen kostenlosen App-ID für "Fusion" anlegen, SDK als `.unitypackage`
  herunterladen und importieren (nicht über den normalen UPM-Registry
  verfügbar).

## 3. Third-Person-Basis

- Über Package Manager → Samples das **"Starter Assets – Third Person
  Character Controller"** Paket importieren als Referenz/Ausgangspunkt für
  `Assets/_Project/Scripts/Player/ThirdPersonController.cs`.

## 4. Committen

Nach dem Setup:

```
git add ProjectSettings Packages Assets
git commit -m "Unity-Projekt initialisieren"
```

`Library/`, `Temp/`, `obj/` etc. werden über `.gitignore` bereits
ausgeschlossen.

## Hinweis zu großen Binärdateien

Für Art-/Audio-Assets ist [Git LFS](https://git-lfs.com/) vorkonfiguriert
(`.gitattributes`). Vor dem ersten Import größerer Assets lokal einmalig
`git lfs install` ausführen.
