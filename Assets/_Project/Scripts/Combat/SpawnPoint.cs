using UnityEngine;

namespace Battlelords.Combat
{
    /// <summary>Markiert einen Team-Spawnpunkt in der Arena.</summary>
    public class SpawnPoint : MonoBehaviour
    {
        public TeamId team = TeamId.TeamA;

        private void OnDrawGizmos()
        {
            Gizmos.color = team == TeamId.TeamA ? Color.cyan : Color.magenta;
            Gizmos.DrawWireSphere(transform.position, 0.5f);
            Gizmos.DrawRay(transform.position, transform.forward * 1.5f);
        }
    }
}
