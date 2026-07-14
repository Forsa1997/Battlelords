using UnityEngine;

namespace Battlelords.Combat
{
    public enum AbilityCastType
    {
        /// <summary>Skillshot-Projektil in Kamerablickrichtung.</summary>
        Projectile,
        /// <summary>Boden-AoE mit Telegraph (Warnung → Zündung).</summary>
        GroundTelegraph,
        /// <summary>Gap-Closer: schneller Lunge nach vorn.</summary>
        Dash,
        /// <summary>Nahkampf-Kegel vor dem Charakter, trifft sofort.</summary>
        MeleeCone,
        /// <summary>Defensiv: Schild auf sich selbst.</summary>
        SelfShield,
        /// <summary>Hitscan-Interrupt: bricht den laufenden Cast des Ziels ab.</summary>
        Interrupt,
    }

    /// <summary>
    /// Daten-Definition einer Fähigkeit als ScriptableObject, damit
    /// Balancing ohne Codeänderung im Inspector passieren kann.
    /// Klassen sind im MVP einfach unterschiedliche Sets aus 3–4 Abilities.
    /// </summary>
    [CreateAssetMenu(menuName = "Battlelords/Ability", fileName = "NewAbility")]
    public class AbilityDefinition : ScriptableObject
    {
        [Header("Allgemein")]
        public string displayName = "Ability";
        public AbilityCastType castType = AbilityCastType.Projectile;
        public float cooldown = 6f;
        public float resourceCost = 20f;
        public float damage = 150f;
        [Tooltip("0 = instant. Während der Cast-Zeit ist der Cast sichtbar und kann unterbrochen werden.")]
        public float castTime = 0f;

        [Header("Projectile")]
        public float projectileSpeed = 25f;
        public float projectileRange = 30f;
        public float projectileHitRadius = 0.5f;

        [Header("Ground Telegraph")]
        public float telegraphRange = 20f;
        public float telegraphRadius = 4f;
        [Tooltip("Zeit zwischen sichtbarer Warnung und Zündung – das Kern-Dodge-Fenster.")]
        public float telegraphWarnDuration = 1.2f;

        [Header("Dash")]
        public float dashDistance = 8f;
        public float dashDuration = 0.25f;

        [Header("Melee Cone")]
        public float coneRange = 4f;
        [Tooltip("Voller Öffnungswinkel des Kegels in Grad.")]
        public float coneAngle = 90f;

        [Header("Self Shield")]
        public float shieldAmount = 250f;
        public float shieldDuration = 4f;

        [Header("Interrupt")]
        public float interruptRange = 20f;
        [Tooltip("So lange kann das unterbrochene Ziel keine neue Fähigkeit wirken.")]
        public float interruptLockout = 2f;

        [Header("Prefabs (optional, sonst Debug-Primitives)")]
        public Projectile projectilePrefab;
        public GroundTelegraph telegraphPrefab;
    }
}
