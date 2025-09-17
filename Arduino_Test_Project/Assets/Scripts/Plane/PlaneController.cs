using UnityEngine;

public class PlaneController : MonoBehaviour
{
    private Rigidbody rb;

    public float launchForceMultiplier = 100f;
    private Vector3 launchStartPos;
    private bool isDragging = false;

    [Header("Flight Controls")]
    public float rollTorque = 10f;
    public float pitchTorque = 10f;
    public float yawTorque = 5f;

    [Header("Aerodynamics")]
    public float liftCoefficient = 0.1f;
    public float dragCoefficient = 0.01f;
    public float wingArea = 1.0f;
    private const float airDensity = 1.225f;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();

        rb.interpolation = RigidbodyInterpolation.Interpolate;
    }

    private void OnMouseDown()
    {
        if (rb.linearVelocity.magnitude < 0.1f)
        {
            isDragging = true;
            launchStartPos = Input.mousePosition;
        }
    }

    private void OnMouseUp()
    {
        if (isDragging)
        {
            isDragging = false;
            Vector3 dragVector = launchStartPos - Input.mousePosition;
            Vector3 launchDirection = new Vector3(dragVector.x, dragVector.y, Mathf.Abs(dragVector.y)).normalized;
            float launchMagnitude = Mathf.Clamp(dragVector.magnitude, 0, 200);
            rb.AddForce(launchDirection * launchMagnitude * launchForceMultiplier, ForceMode.Impulse);
        }

    }
}
