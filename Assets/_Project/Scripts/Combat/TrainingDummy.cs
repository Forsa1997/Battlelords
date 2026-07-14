using System.Collections;
using UnityEngine;

namespace Battlelords.Combat
{
    /// <summary>
    /// Trainings-Dummy für die Testszene: blinkt bei Treffern rot,
    /// loggt den Schaden und heilt sich nach dem "Tod" automatisch
    /// wieder hoch, damit durchgehend getestet werden kann.
    /// </summary>
    [RequireComponent(typeof(Health))]
    public class TrainingDummy : MonoBehaviour
    {
        [SerializeField] private float respawnDelay = 2f;
        [SerializeField] private Renderer bodyRenderer;

        private Health _health;
        private Color _baseColor;

        private void Awake()
        {
            _health = GetComponent<Health>();
            if (bodyRenderer == null)
                bodyRenderer = GetComponentInChildren<Renderer>();
            if (bodyRenderer != null)
                _baseColor = bodyRenderer.material.color;

            _health.HealthChanged += OnHealthChanged;
            _health.Died += OnDied;
        }

        private void OnHealthChanged(float current, float max)
        {
            Debug.Log($"{name}: {current:0}/{max:0} HP");
            if (bodyRenderer != null)
                StartCoroutine(FlashRed());
        }

        private void OnDied(DamageInfo killingBlow)
        {
            Debug.Log($"{name} wurde besiegt von {killingBlow.Source?.name}");
            StartCoroutine(Respawn());
        }

        private IEnumerator FlashRed()
        {
            bodyRenderer.material.color = Color.red;
            yield return new WaitForSeconds(0.15f);
            bodyRenderer.material.color = _baseColor;
        }

        private IEnumerator Respawn()
        {
            yield return new WaitForSeconds(respawnDelay);
            _health.ResetToFull();
        }
    }
}
