using System.Collections;
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

    [Header("Turning Controls")] // New Header for clarity

    [SerializeField]
    private float pitchSpeed = 1f; // Renamed for clarity (was turnSpeed)
    [SerializeField]
    private float yawSpeed = 1f; // New variable for left/right turning
    [SerializeField]
    private float rollLimit = 45f; // Limits the roll angle for banking
    [SerializeField]
    private float verticalClampMax = 85;
    [SerializeField]
    private float verticalClampMin = -85;


    [Header("Collision & Recovery")]
    [SerializeField]
    private float impactForceMagnitude = 500f; // Force applied on impact to knock it back
    [SerializeField]
    private float recoveryTime = 2f; // Duration of the tumbling and recovery phase
    [SerializeField]
    private float postImpactSpeedReduction = 0.5f; // Percentage of speed remaining after recovery
    [SerializeField]
    private float angularVelocityTumble = 180f; // Max rotation speed during tumble

    [HideInInspector]
    public bool pitchLocked = false; // Public variable to lock rotation when needed

    private Rigidbody rb;
    private CameraFollow camFollow;
    private bool isRecovering = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        // Ensure gravity is enabled on the Rigidbody for the pull-down effect
        rb.useGravity = true;
        // Prevents unrealistic rotation from physics
        rb.freezeRotation = true;

        camFollow = GetComponent<CameraFollow>();
    }

    void FixedUpdate()
    {
        // Inputs and Flight Forces are ignored if we are recovering
        if (!isRecovering)
        {
            float verticalInput = Input.GetAxis("Vertical");
            float horizontalInput = Input.GetAxis("Horizontal");

            UpdateRotation(-verticalInput * pitchSpeed, horizontalInput * yawSpeed);

            ApplyFlightForces();
        }
        else
        {
            // Add optional small damping to the linear velocity to prevent runaway speed
            rb.linearVelocity *= 0.99f;
        }
    }

    // --------------------------------------------------------------------------------
    // Rotation and Input
    // --------------------------------------------------------------------------------

    void UpdateRotation(float pitchAdjustment, float yawAdjustment)
    {
        Vector3 currentRotation = transform.localEulerAngles;
        float newPitch = currentRotation.x;

        // 1. Pitch (X-axis, up/down)
        if (!pitchLocked)
        {
            newPitch += pitchAdjustment;

            // Handle wrap-around for negative angles
            if (newPitch > 180f)
            {
                newPitch -= 360f;
            }
            // Clamp pitch to prevent over-rotation
            newPitch = Mathf.Clamp(newPitch, verticalClampMin, verticalClampMax);
        }

        // 2. Yaw (Y-axis, left/right)
        // Apply rotation directly to the Y-axis (global or local, depending on desired control)
        // Using Space.Self ensures the turn is relative to the glider's current facing
        transform.Rotate(Vector3.up, yawAdjustment * Time.fixedDeltaTime * 60f, Space.Self);

        // 3. Roll (Z-axis, banking for aesthetics)
        // Smoothly rotate the glider on the Z-axis (roll) based on yaw input.
        // This adds a "banking" effect to make the turn feel more realistic.
        float targetRoll = yawAdjustment * rollLimit; // Negative to tilt in the turn direction
        float newRoll = Mathf.LerpAngle(currentRotation.z, targetRoll, Time.fixedDeltaTime * 5f);

        // 4. Apply all rotations
        // Since we used transform.Rotate for Yaw, we only need to set Pitch and Roll
        // The Y rotation from transform.Rotate is preserved.
        transform.localRotation = Quaternion.Euler(newPitch, transform.localEulerAngles.y, newRoll);
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

        // 1. Calculate SIGNED Angle of Attack (AoA)
        // angle between the travel vector (velocity) and the wing's forward vector (transform.forward)
        // The axis of rotation is the glider's right vector (transform.right)
        float aoaDegrees = Vector3.SignedAngle(velocity, transform.forward, transform.right);
        float aoaRadians = aoaDegrees * Mathf.Deg2Rad;

        // 2. Simplified Lift Magnitude
        // Use the signed AoA (or its sin) to determine if lift is positive (up) or negative (downforce)
        float liftMagnitude = 0.5f * airDensity * speed * speed * liftCoefficient * wingArea * Mathf.Sin(aoaRadians);

        // 3. Lift Direction
        // Lift must be perpendicular to the airflow (velocity). 
        // Use the cross product between velocity and the glider's right vector.
        // Ensure normalization for a unit direction vector.
        Vector3 liftDirection = Vector3.Cross(velocity.normalized, transform.right).normalized;

        // Apply Lift
        Vector3 liftForce = liftDirection * liftMagnitude;
        rb.AddForce(liftForce, ForceMode.Force);


        // Debug visualization
        Debug.DrawRay(transform.position, velocity.normalized * 5f, Color.blue); // Velocity
        Debug.DrawRay(transform.position, dragForce.normalized * 5f, Color.red); // Drag
        Debug.DrawRay(transform.position, liftForce.normalized * 5f, Color.green); // Lift
    }

    public void TriggerImpact(Vector3 impactNormal)
    {
        if (!isRecovering)
        {
            isRecovering = true;

            // 1. Knock the plane backwards
            // Force is applied opposite to the collision normal provided by the obstacle
            rb.AddForce((-impactNormal + Vector3.up).normalized * impactForceMagnitude, ForceMode.Impulse);

            // 2. Allow it to tumble
            rb.freezeRotation = false; // Unfreeze rotation
            rb.angularVelocity = Vector3.zero; // Clear previous velocity
            rb.AddTorque(Random.insideUnitSphere * angularVelocityTumble, ForceMode.VelocityChange); // Start a random spin

            camFollow.MoveTargetPosition(-impactNormal + Vector3.up, impactForceMagnitude);
            // Start the recovery process
            StartCoroutine(HandleImpactAndRecovery());
        }
    }

    private IEnumerator HandleImpactAndRecovery()
    {
        yield return new WaitForSeconds(recoveryTime);

        // --- Recovery Phase ---

        // 1. Calculate new reduced speed and direction
        Vector3 currentVelocity = rb.linearVelocity;
        float newSpeed = currentVelocity.magnitude * postImpactSpeedReduction;
        Vector3 newDirection = currentVelocity.normalized;

        // Reset rotation and re-freeze it
        transform.rotation = Quaternion.LookRotation(newDirection);
        rb.freezeRotation = true;
        rb.angularVelocity = Vector3.zero; // Stop any residual spinning

        // 2. Set the new reduced velocity
        rb.linearVelocity = newDirection * newSpeed;

        // Reset the state flag
        isRecovering = false;
        pitchLocked = false;

        camFollow.ResetTargetPosition();
    }
}