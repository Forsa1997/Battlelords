using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Battlelords.Combat
{
    public enum MatchPhase
    {
        WaitingToStart,
        Countdown,
        RoundActive,
        RoundEnded,
        MatchEnded,
    }

    /// <summary>
    /// Steuert den 2v2-Match-Flow: Countdown → Runde → Rundenende →
    /// Reset, bis ein Team die Ziel-Rundenzahl gewinnt. Läuft die
    /// Rundenzeit ab, endet die Runde unentschieden (kein Punkt).
    ///
    /// Lokale Phase-4-Version; die Zustandsmaschine wandert in Phase 3/4
    /// in ein server-autoritatives NetworkBehaviour, die Struktur
    /// (Phasen + Übergänge) bleibt dieselbe.
    /// </summary>
    public class MatchManager : MonoBehaviour
    {
        [SerializeField] private int roundsToWin = 3;
        [SerializeField] private float roundTimeLimit = 180f;
        [SerializeField] private float countdownDuration = 3f;
        [SerializeField] private float roundEndPause = 4f;

        private readonly List<Health> _teamA = new();
        private readonly List<Health> _teamB = new();
        private readonly List<SpawnPoint> _spawnPoints = new();
        private readonly List<string> _killfeed = new();

        private float _phaseTimer;

        public MatchPhase Phase { get; private set; } = MatchPhase.WaitingToStart;
        public int ScoreTeamA { get; private set; }
        public int ScoreTeamB { get; private set; }
        public float RoundTimeRemaining { get; private set; }
        public float CountdownRemaining => Mathf.Max(0f, _phaseTimer);
        public string LastRoundResult { get; private set; } = "";
        public IReadOnlyList<string> Killfeed => _killfeed;
        public int RoundsToWin => roundsToWin;

        private void Start()
        {
            foreach (Health health in FindObjectsByType<Health>(FindObjectsSortMode.None))
            {
                if (health.Team == TeamId.TeamA) _teamA.Add(health);
                else if (health.Team == TeamId.TeamB) _teamB.Add(health);
                health.Died += info => OnPlayerDied(health, info);
            }

            _spawnPoints.AddRange(FindObjectsByType<SpawnPoint>(FindObjectsSortMode.None));

            if (_teamA.Count > 0 && _teamB.Count > 0)
                BeginCountdown();
            else
                Debug.LogWarning("MatchManager: Es braucht mindestens je einen Spieler in TeamA und TeamB.");
        }

        private void Update()
        {
            switch (Phase)
            {
                case MatchPhase.Countdown:
                    _phaseTimer -= Time.deltaTime;
                    if (_phaseTimer <= 0f)
                        BeginRound();
                    break;

                case MatchPhase.RoundActive:
                    RoundTimeRemaining -= Time.deltaTime;
                    if (RoundTimeRemaining <= 0f)
                        EndRound(TeamId.None);
                    break;

                case MatchPhase.RoundEnded:
                    _phaseTimer -= Time.deltaTime;
                    if (_phaseTimer <= 0f)
                        BeginCountdown();
                    break;

                case MatchPhase.MatchEnded:
                    if (Input.GetKeyDown(KeyCode.R))
                        Rematch();
                    break;
            }
        }

        private void BeginCountdown()
        {
            ResetAllPlayers();
            Phase = MatchPhase.Countdown;
            _phaseTimer = countdownDuration;
        }

        private void BeginRound()
        {
            Phase = MatchPhase.RoundActive;
            RoundTimeRemaining = roundTimeLimit;
        }

        private void OnPlayerDied(Health player, DamageInfo killingBlow)
        {
            if (Phase != MatchPhase.RoundActive)
                return;

            string killer = killingBlow.Source != null ? killingBlow.Source.name : "?";
            AddKillfeedEntry($"{killer} → {player.name}");

            if (_teamA.All(h => !h.IsAlive))
                EndRound(TeamId.TeamB);
            else if (_teamB.All(h => !h.IsAlive))
                EndRound(TeamId.TeamA);
        }

        private void EndRound(TeamId winner)
        {
            switch (winner)
            {
                case TeamId.TeamA:
                    ScoreTeamA++;
                    LastRoundResult = "Team A gewinnt die Runde!";
                    break;
                case TeamId.TeamB:
                    ScoreTeamB++;
                    LastRoundResult = "Team B gewinnt die Runde!";
                    break;
                default:
                    LastRoundResult = "Zeit abgelaufen — unentschieden.";
                    break;
            }

            if (ScoreTeamA >= roundsToWin || ScoreTeamB >= roundsToWin)
            {
                Phase = MatchPhase.MatchEnded;
                LastRoundResult = ScoreTeamA >= roundsToWin
                    ? "TEAM A GEWINNT DAS MATCH!"
                    : "TEAM B GEWINNT DAS MATCH!";
            }
            else
            {
                Phase = MatchPhase.RoundEnded;
                _phaseTimer = roundEndPause;
            }
        }

        private void Rematch()
        {
            ScoreTeamA = 0;
            ScoreTeamB = 0;
            _killfeed.Clear();
            LastRoundResult = "";
            BeginCountdown();
        }

        private void ResetAllPlayers()
        {
            var usedSpawns = new HashSet<SpawnPoint>();
            foreach (Health player in _teamA.Concat(_teamB))
            {
                player.ResetToFull();
                SpawnPoint spawn = _spawnPoints
                    .FirstOrDefault(s => s.team == player.Team && !usedSpawns.Contains(s));
                if (spawn == null)
                    continue;
                usedSpawns.Add(spawn);

                // CharacterController blockiert direkte Transform-Teleports,
                // solange er aktiv ist
                if (player.TryGetComponent(out CharacterController controller))
                {
                    controller.enabled = false;
                    player.transform.SetPositionAndRotation(spawn.transform.position, spawn.transform.rotation);
                    controller.enabled = true;
                }
                else
                {
                    player.transform.SetPositionAndRotation(spawn.transform.position, spawn.transform.rotation);
                }
            }
        }

        private void AddKillfeedEntry(string entry)
        {
            _killfeed.Add(entry);
            if (_killfeed.Count > 5)
                _killfeed.RemoveAt(0);
        }
    }
}
