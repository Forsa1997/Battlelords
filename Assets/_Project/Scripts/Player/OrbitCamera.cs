using UnityEngine;

namespace Battlelords.Player
{
    /// <summary>
    /// Einfache Third-Person-Orbit-Kamera für den Phase-1-Prototyp.
    /// Maus steuert Yaw/Pitch, Kamera folgt dem Ziel mit festem Abstand.
    /// Kann später durch Cinemachine ersetzt werden, ohne dass der
    /// ThirdPersonController angepasst werden muss (der liest nur
    /// diese Transform-Ausrichtung).
    /// </summary>
    public class OrbitCamera : MonoBehaviour
    {
        [SerializeField] private Transform target;
        [SerializeField] private Vector3 targetOffset = new Vector3(0f, 1.6f, 0f);
        [SerializeField] private float distance = 5f;
        [SerializeField] private float mouseSensitivity = 2.5f;
        [SerializeField] private float minPitch = -35f;
        [SerializeField] private float maxPitch = 70f;
        [SerializeField] private LayerMask collisionMask = ~0;
        [SerializeField] private float collisionRadius = 0.25f;

        private float _yaw;
        private float _pitch = 15f;

        private void Start()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        private void LateUpdate()
        {
            if (target == null)
                return;

            _yaw += Input.GetAxis("Mouse X") * mouseSensitivity;
            _pitch -= Input.GetAxis("Mouse Y") * mouseSensitivity;
            _pitch = Mathf.Clamp(_pitch, minPitch, maxPitch);

            Quaternion rotation = Quaternion.Euler(_pitch, _yaw, 0f);
            Vector3 pivot = target.position + targetOffset;
            Vector3 desiredPosition = pivot - rotation * Vector3.forward * distance;

            // Kamera nicht durch Wände/Boden clippen lassen
            if (Physics.SphereCast(pivot, collisionRadius, (desiredPosition - pivot).normalized,
                    out RaycastHit hit, distance, collisionMask, QueryTriggerInteraction.Ignore))
            {
                desiredPosition = pivot + (desiredPosition - pivot).normalized * hit.distance;
            }

            transform.SetPositionAndRotation(desiredPosition, rotation);
        }
    }
}
