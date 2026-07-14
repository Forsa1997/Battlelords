using UnityEngine;
using Battlelords.Combat;

namespace Battlelords.UI
{
    /// <summary>
    /// IMGUI-Anzeige für den Match-Zustand: Score, Rundentimer,
    /// Countdown, Rundenergebnis und Killfeed. Wie das DebugHud ein
    /// Wegwerf-HUD für die Testphase (Phase 5 ersetzt beides).
    /// </summary>
    [RequireComponent(typeof(MatchManager))]
    public class MatchHud : MonoBehaviour
    {
        private MatchManager _match;
        private GUIStyle _centerStyle;

        private void Awake()
        {
            _match = GetComponent<MatchManager>();
        }

        private void OnGUI()
        {
            _centerStyle ??= new GUIStyle(GUI.skin.label)
            {
                alignment = TextAnchor.MiddleCenter,
                fontSize = 28,
                fontStyle = FontStyle.Bold,
            };

            // Score + Timer oben mittig
            string timer = _match.Phase == MatchPhase.RoundActive
                ? $"{Mathf.CeilToInt(_match.RoundTimeRemaining) / 60}:{Mathf.CeilToInt(_match.RoundTimeRemaining) % 60:00}"
                : "-:--";
            GUI.Label(new Rect(Screen.width / 2f - 150f, 10f, 300f, 30f),
                $"Team A  {_match.ScoreTeamA} : {_match.ScoreTeamB}  Team B    ({timer})", _centerStyle);

            // Phasen-Overlay in der Bildschirmmitte
            string overlay = _match.Phase switch
            {
                MatchPhase.Countdown => $"Runde startet in {Mathf.CeilToInt(_match.CountdownRemaining)} …",
                MatchPhase.RoundEnded => _match.LastRoundResult,
                MatchPhase.MatchEnded => $"{_match.LastRoundResult}\n[R] für Rematch",
                MatchPhase.WaitingToStart => "Warte auf Spieler beider Teams …",
                _ => null,
            };
            if (overlay != null)
                GUI.Label(new Rect(0f, Screen.height * 0.3f, Screen.width, 80f), overlay, _centerStyle);

            // Killfeed rechts oben
            float y = 10f;
            foreach (string entry in _match.Killfeed)
            {
                GUI.Label(new Rect(Screen.width - 260f, y, 250f, 22f), entry);
                y += 22f;
            }
        }
    }
}
