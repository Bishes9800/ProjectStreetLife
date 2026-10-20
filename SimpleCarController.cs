using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class SimpleCarController : MonoBehaviour
{
    public float acceleration = 18f;
    public float reverseAcceleration = 9f;
    public float maxSpeed = 22f;
    public float steeringDegreesPerSecond = 80f;
    public float brakeDrag = 3f;

    [HideInInspector] public bool hasDriver;
    [HideInInspector] public float throttle;
    [HideInInspector] public float steering;
    [HideInInspector] public bool braking;

    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
    }

    private void FixedUpdate()
    {
        if (!hasDriver)
        {
            throttle = 0;
            steering = 0;
            braking = false;
            return;
        }

        float forwardSpeed = Vector3.Dot(rb.linearVelocity, transform.forward);
        if (throttle > 0 && forwardSpeed < maxSpeed)
            rb.AddForce(transform.forward * throttle * acceleration, ForceMode.Acceleration);
        else if (throttle < 0 && forwardSpeed > -maxSpeed * 0.35f)
            rb.AddForce(transform.forward * throttle * reverseAcceleration, ForceMode.Acceleration);

        float speedFactor = Mathf.Clamp01(rb.linearVelocity.magnitude / 2f);
        if (Mathf.Abs(forwardSpeed) > 0.2f)
        {
            float direction = forwardSpeed >= 0 ? 1f : -1f;
            float turn = steering * steeringDegreesPerSecond * speedFactor * direction * Time.fixedDeltaTime;
            rb.MoveRotation(rb.rotation * Quaternion.Euler(0, turn, 0));
        }

        rb.linearDamping = braking ? brakeDrag : 0.1f;
    }

    public void SetInput(float drive, float turn, bool brake = false)
    {
        throttle = Mathf.Clamp(drive, -1f, 1f);
        steering = Mathf.Clamp(turn, -1f, 1f);
        braking = brake;
    }

    public float SpeedKmh => rb != null ? rb.linearVelocity.magnitude * 3.6f : 0f;
}
