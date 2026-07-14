using UnityEngine;

namespace Battlelords.Combat
{
    public readonly struct DamageInfo
    {
        public readonly float Amount;
        public readonly TeamId SourceTeam;
        public readonly GameObject Source;

        public DamageInfo(float amount, TeamId sourceTeam, GameObject source)
        {
            Amount = amount;
            SourceTeam = sourceTeam;
            Source = source;
        }
    }

    public interface IDamageable
    {
        TeamId Team { get; }
        bool IsAlive { get; }
        void TakeDamage(in DamageInfo info);
    }
}
