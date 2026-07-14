using UnityEngine;
using Battlelords.Combat;

namespace Battlelords.UI
{
    /// <summary>
    /// Wegwerf-HUD für Phase 1/2 auf IMGUI-Basis: HP, Resource,
    /// Cooldowns und Fadenkreuz. Wird in Phase 5 durch ein richtiges
    /// UI-Toolkit/uGUI-HUD ersetzt.
    /// </summary>
    public class DebugHud : MonoBehaviour
    {
        [SerializeField] private Health playerHealth;
        [SerializeField] private AbilityCaster playerCaster;

        private void OnGUI()
        {
            if (playerHealth == null || playerCaster == null)
                return;

            // Fadenkreuz
            GUI.Label(new Rect(Screen.width / 2f - 5f, Screen.height / 2f - 10f, 20f, 20f), "+");

            GUILayout.BeginArea(new Rect(20f, Screen.height - 140f, 400f, 120f));
            GUILayout.Label($"HP: {playerHealth.Current:0} / {playerHealth.Max:0}");
            GUILayout.Label($"Resource: {playerCaster.CurrentResource:0} / {playerCaster.MaxResource:0}");

            GUILayout.BeginHorizontal();
            for (int i = 0; i < playerCaster.Abilities.Length; i++)
            {
                AbilityDefinition ability = playerCaster.Abilities[i];
                if (ability == null)
                    continue;

                float cd = playerCaster.GetCooldownRemaining(i);
                string label = cd > 0f ? $"[{i + 1}] {ability.displayName} ({cd:0.0}s)" : $"[{i + 1}] {ability.displayName}";
                GUILayout.Label(label, GUILayout.Width(140f));
            }
            GUILayout.EndHorizontal();
            GUILayout.EndArea();
        }
    }
}
