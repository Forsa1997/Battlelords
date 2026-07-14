using UnityEngine;

namespace Battlelords.Player
{
    /// <summary>
    /// Phase-1-Prototyp: lokale Bewegung ohne Netzwerk-Anbindung.
    /// Wird in Phase 3 auf ein Fusion NetworkBehaviour mit
    /// Client-Prediction umgestellt; die Eingabe-/Bewegungslogik hier
    /// dient als Referenz für dieses spätere Networked-Movement.
    /// </summary>
    [RequireComponent(typeof(CharacterController))]
    public class ThirdPersonController : MonoBehaviour
    {
        [Header("Movement")]
        [SerializeField] private float walkSpeed = 4f;
        [SerializeField] private float sprintSpeed = 7f;
        [SerializeField] private float rotationSpeed = 12f;
        [SerializeField] private float gravity = -20f;

        [Header("Dodge Roll")]
        [SerializeField] private float dodgeDistance = 4f;
        [SerializeField] private float dodgeDuration = 0.35f;
        [SerializeField] private float dodgeCooldown = 1f;

        [Header("Camera")]
        [SerializeField] private Transform cameraTransform;

        private CharacterController _controller;
        private Vector3 _verticalVelocity;
        private bool _isDodging;
        private float _dodgeCooldownTimer;

        public bool IsInvulnerable { get; private set; }

        private void Awake()
        {
            _controller = GetComponent<CharacterController>();
        }

        private void Update()
        {
            if (_dodgeCooldownTimer > 0f)
                _dodgeCooldownTimer -= Time.deltaTime;

            if (_isDodging)
                return;

            Vector2 moveInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

            if (Input.GetKeyDown(KeyCode.Space) && moveInput.sqrMagnitude > 0.01f && _dodgeCooldownTimer <= 0f)
            {
                StartCoroutine(DodgeRoll(moveInput));
                return;
            }

            Move(moveInput, Input.GetKey(KeyCode.LeftShift));
        }

        private void Move(Vector2 input, bool sprinting)
        {
            Vector3 camForward = FlattenAndNormalize(cameraTransform.forward);
            Vector3 camRight = FlattenAndNormalize(cameraTransform.right);
            Vector3 moveDir = camForward * input.y + camRight * input.x;

            float speed = sprinting ? sprintSpeed : walkSpeed;
            ApplyGravity();

            _controller.Move((moveDir * speed + _verticalVelocity) * Time.deltaTime);

            // WildStar-Feeling: Charakter strafet in Bewegungsrichtung,
            // dreht sich aber immer zur Kamerablickrichtung, nicht zur
            // Laufrichtung (kein "Tank-Turning" wie bei klassischem MMO-Movement).
            Quaternion targetRotation = Quaternion.LookRotation(camForward, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }

        private System.Collections.IEnumerator DodgeRoll(Vector2 input)
        {
            _isDodging = true;
            IsInvulnerable = true;
            _dodgeCooldownTimer = dodgeCooldown;

            Vector3 camForward = FlattenAndNormalize(cameraTransform.forward);
            Vector3 camRight = FlattenAndNormalize(cameraTransform.right);
            Vector3 dodgeDir = (camForward * input.y + camRight * input.x).normalized;
            float dodgeSpeed = dodgeDistance / dodgeDuration;

            float elapsed = 0f;
            while (elapsed < dodgeDuration)
            {
                ApplyGravity();
                _controller.Move((dodgeDir * dodgeSpeed + _verticalVelocity) * Time.deltaTime);
                elapsed += Time.deltaTime;
                yield return null;
            }

            IsInvulnerable = false;
            _isDodging = false;
        }

        private void ApplyGravity()
        {
            if (_controller.isGrounded && _verticalVelocity.y < 0f)
                _verticalVelocity.y = -2f;
            else
                _verticalVelocity.y += gravity * Time.deltaTime;
        }

        private static Vector3 FlattenAndNormalize(Vector3 v)
        {
            v.y = 0f;
            return v.normalized;
        }
    }
}
