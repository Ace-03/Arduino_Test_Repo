using UnityEngine;

public class GlidingSystemV2 : MonoBehaviour
{
    [Header("Flight Characteristics")]
    [SerializeField]
    private float liftCoefficient = 0.1f; // How much lift is generated (adjust for "floatiness")
    [SerializeField]
    private float dragCoefficient = 0.05f; // How much air resistance is applied
    [SerializeField]
    private float airDensity = 1.225f; // Density of air (standard at sea level)
    [SerializeField]
    private float wingArea = 2f; // Conceptual wing area (adjust for overall effect)
    [SerializeField]
    private float turnSpeed = 1f;

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        // Ensure gravity is enabled on the Rigidbody for the pull-down effect
        rb.useGravity = true;
        // Prevents unrealistic rotation from physics
        rb.freezeRotation = true;
    }

    void FixedUpdate()
    {
        float verticalInput = Input.GetAxis("Vertical");
        UpdateRotation(verticalInput * turnSpeed);

        // Physics calculations should be in FixedUpdate
        ApplyFlightForces();
    }

    // --------------------------------------------------------------------------------
    // Rotation and Input
    // --------------------------------------------------------------------------------

    void UpdateRotation(float rotationAdjustment)
    {
        Vector3 currentRotation = transform.localEulerAngles;
        // Adjust the X-axis (pitch)
        float newPitch = currentRotation.x + rotationAdjustment;

        // Simple clamping to prevent excessive flipping (optional)
        // Adjust these values to suit your desired pitch limits.
        if (newPitch > 180f)
        {
            newPitch -= 360f; // Handle wrap-around for negative angles
        }
        newPitch = Mathf.Clamp(newPitch, -70f, 70f); // Example: between -80 (dive) and 80 (climb)

        transform.localRotation = Quaternion.Euler(newPitch, currentRotation.y, currentRotation.z);
    }

    // --------------------------------------------------------------------------------
    // Core Physics Calculations
    // --------------------------------------------------------------------------------

    void ApplyFlightForces()
    {
        Vector3 velocity = rb.linearVelocity;
        float speed = velocity.magnitude;

        // If speed is zero, no air forces can be applied
        if (speed < 0.1f) return;

        // 1. Drag Force (Always opposite to velocity)
        // Formula: F_D = 0.5 * rho * v^2 * C_D * A 
        // where rho=airDensity, v=speed, C_D=dragCoefficient, A=wingArea
        Vector3 dragDirection = -velocity.normalized;
        float dragMagnitude = 0.5f * airDensity * speed * speed * dragCoefficient * wingArea;
        Vector3 dragForce = dragDirection * dragMagnitude;
        rb.AddForce(dragForce, ForceMode.Force);

        // 2. Lift Force (Perpendicular to velocity, and generally upwards relative to the plane)
        // Lift is proportional to the **Angle of Attack (AoA)**.
        // AoA is the angle between the plane's forward direction (transform.forward) 
        // and the direction of travel (velocity.normalized).

        // Calculate Angle of Attack (AoA) in radians
        float aoa = Vector3.Angle(transform.forward, velocity) * Mathf.Deg2Rad;

        // Simplified Lift Magnitude: proportional to speed squared and AoA
        // Formula: F_L = 0.5 * rho * v^2 * C_L * A * sin(AoA)
        // A simple sin(AoA) works well for small angles and ensures lift is zero when AoA is zero.
        float liftMagnitude = 0.5f * airDensity * speed * speed * liftCoefficient * wingArea * Mathf.Sin(aoa);

        // Lift Direction: Perpendicular to velocity and in the plane's "up" direction.
        // The cross product gives a vector perpendicular to both velocity and the plane's right vector (transform.right).
        Vector3 liftDirection = Vector3.Cross(velocity.normalized, -transform.right);

        // Apply Lift
        Vector3 liftForce = liftDirection * liftMagnitude;
        rb.AddForce(liftForce, ForceMode.Force);

        // 3. Gravity (Handled by rb.useGravity = true, but you can add it explicitly for control)
        // Vector3 gravity = Physics.gravity * rb.mass;
        // rb.AddForce(gravity, ForceMode.Force);

        // Debug visualization
        Debug.DrawRay(transform.position, velocity.normalized * 5f, Color.blue); // Velocity
        Debug.DrawRay(transform.position, dragForce.normalized * 5f, Color.red); // Drag
        Debug.DrawRay(transform.position, liftForce.normalized * 5f, Color.green); // Lift
    }
}