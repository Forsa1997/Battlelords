using UnityEngine;

namespace Battlelords.Combat
{
    /// <summary>
    /// Skillshot-Projektil: fliegt geradeaus, trifft das erste gegnerische
    /// Ziel im Weg (SphereCast pro Frame, damit auch schnelle Projektile
    /// nichts durchtunneln), despawnt nach maximaler Reichweite.
    /// </summary>
    public class Projectile : MonoBehaviour
    {
        private float _speed;
        private float _remainingRange;
        private float _hitRadius;
        private DamageInfo _damage;
        private LayerMask _hitMask;

        public void Launch(Vector3 direction, AbilityDefinition ability, TeamId sourceTeam, GameObject source, LayerMask hitMask)
        {
            transform.forward = direction.normalized;
            _speed = ability.projectileSpeed;
            _remainingRange = ability.projectileRange;
            _hitRadius = ability.projectileHitRadius;
            _damage = new DamageInfo(ability.damage, sourceTeam, source);
            _hitMask = hitMask;
        }

        private void Update()
        {
            float step = _speed * Time.deltaTime;

            if (Physics.SphereCast(transform.position, _hitRadius, transform.forward,
                    out RaycastHit hit, step, _hitMask, QueryTriggerInteraction.Ignore))
            {
                if (hit.collider.TryGetComponent(out IDamageable target) && target.Team != _damage.SourceTeam)
                {
                    target.TakeDamage(_damage);
                    Destroy(gameObject);
                    return;
                }

                // Wand oder sonstige Geometrie getroffen
                if (!hit.collider.TryGetComponent(out IDamageable _))
                {
                    Destroy(gameObject);
                    return;
                }
            }

            transform.position += transform.forward * step;
            _remainingRange -= step;

            if (_remainingRange <= 0f)
                Destroy(gameObject);
        }
    }
}
