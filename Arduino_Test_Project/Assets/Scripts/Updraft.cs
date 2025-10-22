using UnityEngine;

public class Updraft : MonoBehaviour
{
    [Header("Updraft Settings")]
    [Tooltip("The direction the glider will be forced to face (and where the force is applied).")]
    [SerializeField]
    private Vector3 updraftDirection = new Vector3(0f, 1f, 0f);

    [Tooltip("The speed (m/s) the glider rotates to align with the updraft pitch.")]
    [SerializeField]
    private float pitchRotationSpeed = 5f;

    [Tooltip("The continuous force applied in the glider's current forward direction.")]
    [SerializeField]
    private float forwardForceMagnitude = 100f;

    private GlidingSystemV2 targetGlider;
    private Rigidbody targetRigidbody;

    void Start()
    {
        Collider col = GetComponent<Collider>();
        if (col == null || !col.isTrigger)
        {
            Debug.LogError("UpdraftArea requires a Collider component set to 'Is Trigger' to function.");
            enabled = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        targetGlider = other.GetComponent<GlidingSystemV2>();
        targetRigidbody = other.GetComponent<Rigidbody>();

        if (targetGlider != null && targetRigidbody != null)
        {
            // 1. Lock the glider's PITCH input
            targetGlider.pitchLocked = true;
            CameraFollow.instance.MoveTargetPosition(updraftDirection, transform.localScale.y);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (targetGlider != null && targetRigidbody != null)
        {
            // 1. Calculate Target Pitch
            // Create a rotation that points only toward the updraft direction.
            Quaternion targetPitchRotation = Quaternion.LookRotation(-updraftDirection.normalized);

            // Extract ONLY the pitch (X-axis) from the target rotation, 
            // and keep the glider's current Y (Yaw) and Z (Roll).
            Vector3 currentEuler = targetGlider.transform.localEulerAngles;

            // Smoothly move the current pitch (X) towards the target pitch (X)
            float newPitch = Mathf.LerpAngle(
                currentEuler.x,
                targetPitchRotation.eulerAngles.x,
                pitchRotationSpeed * Time.fixedDeltaTime
            );

            // 2. Apply the new pitch, preserving the player's current Yaw and Roll
            targetGlider.transform.localRotation = Quaternion.Euler(
                newPitch,
                currentEuler.y,
                currentEuler.z
            );

            // 3. Apply Forward Force
            // The force is applied in the glider's current forward direction, 
            // which the player can influence with Yaw (A/D).
            Vector3 force = -targetGlider.transform.forward * forwardForceMagnitude;
            targetRigidbody.AddForce(force, ForceMode.Force);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (targetGlider != null)
        {
            // 1. Unlock the glider's PITCH input
            targetGlider.pitchLocked = false;

            // 2. Clear references
            targetGlider = null;
            targetRigidbody = null;

            CameraFollow.instance.ResetTargetPosition();
        }
    }
}