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

        [Header("Prefabs (optional, sonst Debug-Primitives)")]
        public Projectile projectilePrefab;
        public GroundTelegraph telegraphPrefab;
    }
}
