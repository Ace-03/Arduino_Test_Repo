using UnityEngine;

/// <summary>
/// Slows down the glider when it enters the trigger zone by applying a drag force.
/// </summary>
public class SlowdownArea : MonoBehaviour
{
    [Header("Slowdown Settings")]
    [Tooltip("The amount of drag applied to the glider's velocity (higher value = stronger slowdown).")]
    [SerializeField]
    private float dragStrength = 5.0f;

    [Tooltip("How frequently the damping force is applied (in seconds). Keep small for smooth effect.")]
    [SerializeField]
    private float forceApplicationRate = 0.05f;

    private Rigidbody targetRigidbody;
    private float nextForceTime = 0f;

    void Start()
    {
        // Check for a Collider component and ensure it is set to 'Is Trigger'
        Collider col = GetComponent<Collider>();
        if (col == null || !col.isTrigger)
        {
            Debug.LogError("SlowdownArea requires a Collider component set to 'Is Trigger' to function.");
            enabled = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Check if the entering object is the glider by looking for the Rigidbody
        Rigidbody rb = other.GetComponent<Rigidbody>();
        if (rb != null && other.GetComponent<GlidingSystemV2>() != null)
        {
            targetRigidbody = rb;
            nextForceTime = Time.time; // Ready to apply force immediately
        }
    }

    private void OnTriggerStay(Collider other)
    {
        // Apply the force repeatedly while the glider is inside
        if (targetRigidbody != null && Time.time >= nextForceTime)
        {
            Vector3 velocity = targetRigidbody.linearVelocity;

            // Calculate the damping force: opposite to the current velocity.
            // Force = -Velocity * DragStrength
            // The faster the glider moves, the stronger the drag force will be.
            Vector3 dampingForce = -velocity * dragStrength;

            // Apply the force using ForceMode.Force, which respects the Rigidbody's mass.
            // Using a simple ForceMode.Force for drag is effective here.
            targetRigidbody.AddForce(dampingForce, ForceMode.Force);

            // Set the time for the next force application
            nextForceTime = Time.time + forceApplicationRate;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // Stop tracking the glider when it leaves
        if (targetRigidbody != null && targetRigidbody.gameObject == other.gameObject)
        {
            targetRigidbody = null;
        }
    }
}