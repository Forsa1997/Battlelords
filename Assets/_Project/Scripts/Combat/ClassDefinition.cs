using UnityEngine;

namespace Battlelords.Combat
{
    /// <summary>
    /// Eine Klasse ist im MVP schlicht ein benanntes Set aus 4 Abilities
    /// (Slot 0–3 = Tasten 1–4). Konkrete Werte siehe docs/CLASSES.md.
    /// </summary>
    [CreateAssetMenu(menuName = "Battlelords/Class", fileName = "NewClass")]
    public class ClassDefinition : ScriptableObject
    {
        public string displayName = "Klasse";
        [TextArea] public string description;
        public AbilityDefinition[] abilitySlots = new AbilityDefinition[4];

        /// <summary>Kopie der Slots, damit ein Caster das Asset-Array nicht mutiert.</summary>
        public AbilityDefinition[] CloneAbilitySlots()
        {
            return (AbilityDefinition[])abilitySlots.Clone();
        }
    }
}
