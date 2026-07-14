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

        /// <summary>Verbleibende Schild-Absorption (0 = kein Schild aktiv).</summary>
        public float ShieldCurrent { get; private set; }

        private float _shieldExpiresAt;

        /// <summary>(aktuell, max) nach jeder Änderung.</summary>
        public event Action<float, float> HealthChanged;
        public event Action<DamageInfo> Died;

        private void Awake()
        {
            _controller = GetComponent<ThirdPersonController>();
            Current = maxHealth;
        }

        private void Update()
        {
            if (ShieldCurrent > 0f && Time.time >= _shieldExpiresAt)
                ShieldCurrent = 0f;
        }

        public void TakeDamage(in DamageInfo info)
        {
            if (!IsAlive)
                return;
            if (info.SourceTeam != TeamId.None && info.SourceTeam == team)
                return;
            if (_controller != null && _controller.IsInvulnerable)
                return;

            float remaining = info.Amount;
            if (ShieldCurrent > 0f)
            {
                float absorbed = Mathf.Min(ShieldCurrent, remaining);
                ShieldCurrent -= absorbed;
                remaining -= absorbed;
            }

            if (remaining <= 0f)
                return;

            Current = Mathf.Max(0f, Current - remaining);
            HealthChanged?.Invoke(Current, maxHealth);

            if (!IsAlive)
                Died?.Invoke(info);
        }

        /// <summary>Legt einen zeitlich begrenzten Absorptions-Schild an (ersetzt einen schwächeren aktiven Schild).</summary>
        public void AddShield(float amount, float duration)
        {
            ShieldCurrent = Mathf.Max(ShieldCurrent, amount);
            _shieldExpiresAt = Time.time + duration;
        }

        public void ResetToFull()
        {
            Current = maxHealth;
            ShieldCurrent = 0f;
            HealthChanged?.Invoke(Current, maxHealth);
        }
    }
}
