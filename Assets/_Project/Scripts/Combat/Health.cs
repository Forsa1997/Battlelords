using System;
using UnityEngine;
using Battlelords.Player;

namespace Battlelords.Combat
{
    /// <summary>
    /// Health-Komponente für Spieler und Trainings-Dummies.
    /// Respektiert die Dodge-i-Frames des ThirdPersonController, falls
    /// einer am selben GameObject hängt.
    /// </summary>
    public class Health : MonoBehaviour, IDamageable
    {
        [SerializeField] private float maxHealth = 1000f;
        [SerializeField] private TeamId team = TeamId.None;

        private ThirdPersonController _controller;

        public float Max => maxHealth;
        public float Current { get; private set; }
        public TeamId Team => team;
        public bool IsAlive => Current > 0f;

        /// <summary>(aktuell, max) nach jeder Änderung.</summary>
        public event Action<float, float> HealthChanged;
        public event Action<DamageInfo> Died;

        private void Awake()
        {
            _controller = GetComponent<ThirdPersonController>();
            Current = maxHealth;
        }

        public void TakeDamage(in DamageInfo info)
        {
            if (!IsAlive)
                return;
            if (info.SourceTeam != TeamId.None && info.SourceTeam == team)
                return;
            if (_controller != null && _controller.IsInvulnerable)
                return;

            Current = Mathf.Max(0f, Current - info.Amount);
            HealthChanged?.Invoke(Current, maxHealth);

            if (!IsAlive)
                Died?.Invoke(info);
        }

        public void ResetToFull()
        {
            Current = maxHealth;
            HealthChanged?.Invoke(Current, maxHealth);
        }
    }
}
