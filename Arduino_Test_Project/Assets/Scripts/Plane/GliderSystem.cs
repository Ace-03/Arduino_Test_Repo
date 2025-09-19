using UnityEngine;

public class GliderSystem : MonoBehaviour
{
    [SerializeField] private float baseSpeed = 10f;
    [SerializeField] private float currentThrustSpeed = 10f;
    [SerializeField] private float maxThrustSpeed = 10f;
    [SerializeField] private float minThrustSpeed = 10f;
    private float CurrentThrustSpeed;

    private Transform CameraTransform;
    private Rigidbody rb;

    void Start()
    {
        CameraTransform = Camera.main.transform.parent;
        rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        GlidingMovement();
    }

    void Update()
    {
        ManageRotation();
    }


    private void GlidingMovement()
    {
        float pitchInRads = transform.eulerAngles.x * Mathf.Deg2Rad;
        float mappedPitch = Mathf.Sin(pitchInRads);
        Vector3 glidingForce = -Vector3.forward * currentThrustSpeed;

        currentThrustSpeed = mappedPitch * Time.deltaTime;
        currentThrustSpeed = Mathf.Clamp(currentThrustSpeed, 0, maxThrustSpeed);

        rb.AddRelativeForce(glidingForce); 
    }

    private void ManageRotation()
    {
        Quaternion targetRotation = Quaternion.Euler(CameraTransform.eulerAngles.x, CameraTransform.eulerAngles.y, transform.eulerAngles.z);
        transform.rotation = targetRotation;
    }
}
