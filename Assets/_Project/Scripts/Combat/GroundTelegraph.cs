using UnityEngine;

namespace Battlelords.Combat
{
    /// <summary>
    /// Kreisförmiger Boden-Telegraph im WildStar-Stil: Warnfläche wird
    /// sichtbar angezeigt und füllt sich über die Warndauer, danach zündet
    /// der Schaden auf alle gegnerischen Ziele in der Fläche. Wer bis zur
    /// Zündung herausläuft oder mit i-Frames dodged, nimmt keinen Schaden.
    /// </summary>
    public class GroundTelegraph : MonoBehaviour
    {
        [Tooltip("Flaches Child-Mesh (z.B. Quad/Zylinder), das den Füllstand anzeigt.")]
        [SerializeField] private Transform fillIndicator;
        [SerializeField] private Transform outline;

        private float _radius;
        private float _warnDuration;
        private float _elapsed;
        private DamageInfo _damage;
        private LayerMask _hitMask;

        /// <summary>Setzt den Füllstands-Indicator, wenn der Telegraph zur Laufzeit ohne Prefab erzeugt wurde.</summary>
        public void SetDebugIndicator(Transform indicator)
        {
            fillIndicator = indicator;
        }

        public void Arm(Vector3 position, AbilityDefinition ability, TeamId sourceTeam, GameObject source, LayerMask hitMask)
        {
            transform.position = position;
            _radius = ability.telegraphRadius;
            _warnDuration = ability.telegraphWarnDuration;
            _damage = new DamageInfo(ability.damage, sourceTeam, source);
            _hitMask = hitMask;

            if (outline != null)
                outline.localScale = new Vector3(_radius * 2f, outline.localScale.y, _radius * 2f);
        }

        private void Update()
        {
            _elapsed += Time.deltaTime;
            float progress = Mathf.Clamp01(_elapsed / _warnDuration);

            if (fillIndicator != null)
                fillIndicator.localScale = new Vector3(_radius * 2f * progress, fillIndicator.localScale.y, _radius * 2f * progress);

            if (_elapsed >= _warnDuration)
            {
                Detonate();
                Destroy(gameObject);
            }
        }

        private void Detonate()
        {
            // Höhenversatz, damit auch springende/stehende Ziele erfasst werden
            Collider[] hits = Physics.OverlapSphere(transform.position + Vector3.up, _radius, _hitMask, QueryTriggerInteraction.Ignore);
            foreach (Collider hit in hits)
            {
                if (hit.TryGetComponent(out IDamageable target) && target.Team != _damage.SourceTeam)
                    target.TakeDamage(_damage);
            }
        }
    }
}
