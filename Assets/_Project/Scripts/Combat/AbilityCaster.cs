using UnityEngine;
using Battlelords.Player;

namespace Battlelords.Combat
{
    /// <summary>
    /// Führt die Abilities des Spielers aus (Tasten 1–4), verwaltet
    /// Cooldowns und Resource-Regeneration. Zielen erfolgt WildStar-artig
    /// frei über die Kamerablickrichtung: Projektile fliegen den
    /// Fadenkreuz-Ray entlang, Boden-Telegraphs landen am Bodenpunkt
    /// unter dem Fadenkreuz (gedeckelt auf Reichweite).
    /// </summary>
    [RequireComponent(typeof(Health))]
    public class AbilityCaster : MonoBehaviour
    {
        [Tooltip("Optional: überschreibt bei Spielstart die Ability-Slots mit dem Set der Klasse.")]
        [SerializeField] private ClassDefinition classDefinition;
        [SerializeField] private AbilityDefinition[] abilities = new AbilityDefinition[4];
        [SerializeField] private Transform castOrigin;
        [SerializeField] private Camera aimCamera;
        [SerializeField] private LayerMask hitMask = ~0;
        [SerializeField] private LayerMask groundMask = ~0;

        [Header("Resource")]
        [SerializeField] private float maxResource = 100f;
        [SerializeField] private float resourceRegenPerSecond = 8f;

        private static readonly KeyCode[] SlotKeys =
            { KeyCode.Alpha1, KeyCode.Alpha2, KeyCode.Alpha3, KeyCode.Alpha4 };

        private readonly float[] _cooldownTimers = new float[4];
        private Health _health;

        private float _castElapsed;
        private float _interruptLockoutUntil;

        public float MaxResource => maxResource;
        public float CurrentResource { get; private set; }
        public AbilityDefinition[] Abilities => abilities;

        /// <summary>Die Ability, die gerade gecastet wird (null = kein laufender Cast).</summary>
        public AbilityDefinition CastingAbility { get; private set; }
        private int _castingSlot = -1;

        public bool IsCasting => CastingAbility != null;
        public float CastProgress => IsCasting ? Mathf.Clamp01(_castElapsed / CastingAbility.castTime) : 0f;
        public bool IsInterruptLocked => Time.time < _interruptLockoutUntil;
        public float InterruptLockoutRemaining => Mathf.Max(0f, _interruptLockoutUntil - Time.time);

        /// <summary>Verbleibender Cooldown des Slots in Sekunden (0 = bereit).</summary>
        public float GetCooldownRemaining(int slot) => Mathf.Max(0f, _cooldownTimers[slot]);

        private void Awake()
        {
            _health = GetComponent<Health>();
            CurrentResource = maxResource;
            if (classDefinition != null)
                abilities = classDefinition.CloneAbilitySlots();
            if (aimCamera == null)
                aimCamera = Camera.main;
            if (castOrigin == null)
                castOrigin = transform;
        }

        private void Update()
        {
            CurrentResource = Mathf.Min(maxResource, CurrentResource + resourceRegenPerSecond * Time.deltaTime);

            for (int i = 0; i < _cooldownTimers.Length; i++)
            {
                if (_cooldownTimers[i] > 0f)
                    _cooldownTimers[i] -= Time.deltaTime;
            }

            if (!_health.IsAlive)
            {
                CancelCast();
                return;
            }

            if (IsCasting)
            {
                _castElapsed += Time.deltaTime;
                if (_castElapsed >= CastingAbility.castTime)
                    FinishCast();
                return;
            }

            if (IsInterruptLocked)
                return;

            for (int slot = 0; slot < SlotKeys.Length; slot++)
            {
                if (Input.GetKeyDown(SlotKeys[slot]))
                    TryCast(slot);
            }
        }

        private void TryCast(int slot)
        {
            AbilityDefinition ability = slot < abilities.Length ? abilities[slot] : null;
            if (ability == null || _cooldownTimers[slot] > 0f || CurrentResource < ability.resourceCost)
                return;

            if (ability.castTime > 0f)
            {
                // Kosten/Cooldown fallen erst bei erfolgreichem Abschluss an —
                // ein Interrupt bricht den Cast "kostenlos" ab, nur die Zeit ist verloren.
                CastingAbility = ability;
                _castingSlot = slot;
                _castElapsed = 0f;
                return;
            }

            ResolveAndPay(ability, slot);
        }

        private void FinishCast()
        {
            AbilityDefinition ability = CastingAbility;
            int slot = _castingSlot;
            CancelCast();
            ResolveAndPay(ability, slot);
        }

        private void CancelCast()
        {
            CastingAbility = null;
            _castingSlot = -1;
            _castElapsed = 0f;
        }

        /// <summary>Bricht einen laufenden Cast ab und sperrt das Wirken für die Lockout-Dauer.</summary>
        public void ApplyInterrupt(float lockoutDuration)
        {
            if (!IsCasting)
                return;
            CancelCast();
            _interruptLockoutUntil = Time.time + lockoutDuration;
        }

        private void ResolveAndPay(AbilityDefinition ability, int slot)
        {
            bool cast = ability.castType switch
            {
                AbilityCastType.Projectile => CastProjectile(ability),
                AbilityCastType.GroundTelegraph => CastGroundTelegraph(ability),
                AbilityCastType.Dash => CastDash(ability),
                AbilityCastType.MeleeCone => CastMeleeCone(ability),
                AbilityCastType.SelfShield => CastSelfShield(ability),
                AbilityCastType.Interrupt => CastInterrupt(ability),
                _ => false,
            };

            if (cast)
            {
                _cooldownTimers[slot] = ability.cooldown;
                CurrentResource -= ability.resourceCost;
            }
        }

        private bool CastProjectile(AbilityDefinition ability)
        {
            Vector3 origin = castOrigin.position + Vector3.up * 1.4f;
            Vector3 direction = GetAimDirection(origin, ability.projectileRange);

            Projectile projectile = ability.projectilePrefab != null
                ? Instantiate(ability.projectilePrefab, origin, Quaternion.identity)
                : CreateDebugProjectile(origin);

            projectile.Launch(direction, ability, _health.Team, gameObject, hitMask);
            return true;
        }

        private bool CastGroundTelegraph(AbilityDefinition ability)
        {
            if (!TryGetGroundAimPoint(ability.telegraphRange, out Vector3 point))
                return false;

            GroundTelegraph telegraph = ability.telegraphPrefab != null
                ? Instantiate(ability.telegraphPrefab, point, Quaternion.identity)
                : CreateDebugTelegraph(point, ability.telegraphRadius);

            telegraph.Arm(point, ability, _health.Team, gameObject, hitMask);
            return true;
        }

        private bool CastDash(AbilityDefinition ability)
        {
            // Vereinfachter Gap-Closer für den Prototyp: sofortiger Lunge
            // in Blickrichtung über den CharacterController.
            if (!TryGetComponent(out CharacterController controller))
                return false;

            Vector3 direction = transform.forward;
            controller.Move(direction * ability.dashDistance);
            return true;
        }

        private bool CastMeleeCone(AbilityDefinition ability)
        {
            var damage = new DamageInfo(ability.damage, _health.Team, gameObject);
            Collider[] hits = Physics.OverlapSphere(transform.position + Vector3.up, ability.coneRange, hitMask, QueryTriggerInteraction.Ignore);
            foreach (Collider hit in hits)
            {
                if (hit.gameObject == gameObject)
                    continue;
                if (!hit.TryGetComponent(out IDamageable target) || target.Team == _health.Team)
                    continue;

                Vector3 toTarget = hit.transform.position - transform.position;
                toTarget.y = 0f;
                if (Vector3.Angle(transform.forward, toTarget) <= ability.coneAngle * 0.5f)
                    target.TakeDamage(damage);
            }
            return true;
        }

        private bool CastSelfShield(AbilityDefinition ability)
        {
            _health.AddShield(ability.shieldAmount, ability.shieldDuration);
            return true;
        }

        private bool CastInterrupt(AbilityDefinition ability)
        {
            // Hitscan in Fadenkreuz-Richtung: trifft sofort, macht (wenig)
            // Schaden und bricht den laufenden Cast des Ziels ab.
            Vector3 origin = castOrigin.position + Vector3.up * 1.4f;
            Vector3 direction = GetAimDirection(origin, ability.interruptRange);

            if (!Physics.SphereCast(origin, 0.4f, direction, out RaycastHit hit,
                    ability.interruptRange, hitMask, QueryTriggerInteraction.Ignore))
                return true; // verfehlt, Cooldown fällt trotzdem an

            if (hit.collider.TryGetComponent(out IDamageable target) && target.Team != _health.Team)
            {
                if (ability.damage > 0f)
                    target.TakeDamage(new DamageInfo(ability.damage, _health.Team, gameObject));
                if (hit.collider.TryGetComponent(out AbilityCaster targetCaster))
                    targetCaster.ApplyInterrupt(ability.interruptLockout);
            }
            return true;
        }

        /// <summary>
        /// Richtung vom Cast-Ursprung zu dem Punkt, den das Fadenkreuz
        /// (Bildschirmmitte) anvisiert — so treffen Projektile das,
        /// worauf die Kamera zeigt, nicht parallel daran vorbei.
        /// </summary>
        private Vector3 GetAimDirection(Vector3 origin, float range)
        {
            Ray ray = aimCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
            Vector3 targetPoint = Physics.Raycast(ray, out RaycastHit hit, range * 2f, hitMask, QueryTriggerInteraction.Ignore)
                ? hit.point
                : ray.GetPoint(range);
            return (targetPoint - origin).normalized;
        }

        private bool TryGetGroundAimPoint(float range, out Vector3 point)
        {
            Ray ray = aimCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
            if (Physics.Raycast(ray, out RaycastHit hit, 100f, groundMask, QueryTriggerInteraction.Ignore))
            {
                Vector3 flatOffset = hit.point - transform.position;
                flatOffset.y = 0f;
                // Auf Reichweite deckeln statt Cast zu verweigern
                point = flatOffset.magnitude > range
                    ? transform.position + flatOffset.normalized * range + Vector3.up * (hit.point.y - transform.position.y)
                    : hit.point;
                return true;
            }

            point = default;
            return false;
        }

        private static Projectile CreateDebugProjectile(Vector3 origin)
        {
            GameObject go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            go.name = "DebugProjectile";
            go.transform.position = origin;
            go.transform.localScale = Vector3.one * 0.4f;
            Destroy(go.GetComponent<Collider>());
            return go.AddComponent<Projectile>();
        }

        private static GroundTelegraph CreateDebugTelegraph(Vector3 point, float radius)
        {
            GameObject root = new GameObject("DebugTelegraph");
            root.transform.position = point;

            GameObject disc = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            disc.name = "Fill";
            Destroy(disc.GetComponent<Collider>());
            disc.transform.SetParent(root.transform, false);
            disc.transform.localPosition = Vector3.up * 0.02f;
            disc.transform.localScale = new Vector3(0f, 0.02f, 0f);
            disc.GetComponent<Renderer>().material.color = new Color(1f, 0.3f, 0.1f, 0.6f);

            GroundTelegraph telegraph = root.AddComponent<GroundTelegraph>();
            telegraph.SetDebugIndicator(disc.transform);
            return telegraph;
        }
    }
}
