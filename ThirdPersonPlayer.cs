using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class ThirdPersonPlayer : MonoBehaviour
{
    [Header("Movement")]
    public float walkSpeed = 4f;
    public float runSpeed = 6.5f;
    public float rotationSpeed = 12f;
    public float gravity = -20f;
    public VirtualJoystick moveJoystick;

    [Header("Camera")]
    public Transform cameraTransform;
    public Transform cameraTarget;
    public float cameraDistance = 5f;
    public float cameraHeight = 1.5f;
    public float cameraFollowSharpness = 12f;
    public float minPitch = -25f;
    public float maxPitch = 65f;

    private CharacterController controller;
    private float verticalVelocity;
    private float yaw;
    private float pitch = 15f;
    private bool movementLocked;

    public bool MovementLocked { get => movementLocked; set => movementLocked = value; }

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        if (cameraTransform != null)
        {
            Vector3 angles = cameraTransform.eulerAngles;
            yaw = angles.y;
        }
    }

    private void Update()
    {
        if (cameraTransform == null) return;

        Vector2 input = moveJoystick != null ? moveJoystick.Value :
            new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

        if (!movementLocked)
        {
            Vector3 forward = cameraTransform.forward;
            Vector3 right = cameraTransform.right;
            forward.y = 0; right.y = 0;
            forward.Normalize(); right.Normalize();

            Vector3 move = forward * input.y + right * input.x;
            if (move.sqrMagnitude > 1f) move.Normalize();

            if (move.sqrMagnitude > 0.01f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(move, Vector3.up);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation,
                    rotationSpeed * Time.deltaTime);
            }

            float speed = Input.GetKey(KeyCode.LeftShift) ? runSpeed : walkSpeed;
            controller.Move(move * speed * Time.deltaTime);
        }

        if (controller.isGrounded && verticalVelocity < 0) verticalVelocity = -2f;
        verticalVelocity += gravity * Time.deltaTime;
        controller.Move(Vector3.up * verticalVelocity * Time.deltaTime);
    }

    public void AddCameraLook(Vector2 delta)
    {
        yaw += delta.x;
        pitch = Mathf.Clamp(pitch - delta.y, minPitch, maxPitch);
    }

    private void LateUpdate()
    {
        if (cameraTransform == null) return;
        Vector3 target = cameraTarget != null ? cameraTarget.position : transform.position + Vector3.up * cameraHeight;
        Quaternion orbit = Quaternion.Euler(pitch, yaw, 0);
        Vector3 desiredPosition = target - orbit * Vector3.forward * cameraDistance;
        cameraTransform.position = Vector3.Lerp(cameraTransform.position, desiredPosition,
            1f - Mathf.Exp(-cameraFollowSharpness * Time.deltaTime));
        cameraTransform.rotation = Quaternion.LookRotation(target - cameraTransform.position, Vector3.up);
    }
}
