using UnityEngine;

public class GlidingSystem : MonoBehaviour
{
    [SerializeField] 
    private float maxGlideSpeed = 10f;
    [SerializeField]
    private float maxDampening = 10f;
    [SerializeField]
    private float turnSpeed = 1f;

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        float verticalInput = Input.GetAxis("Vertical");
        UpdateRotation(verticalInput * turnSpeed);
        ApplyForwardForce();
    }


    void UpdateRotation(float rotationAdjustment)
    {
        Vector3 currentRotation = transform.localEulerAngles;
        Vector3 newRotation = new Vector3(currentRotation.x + rotationAdjustment, currentRotation.y, currentRotation.z);

        if ((newRotation.x <= 180 && newRotation.x >= 70) || (newRotation.x > 180 && newRotation.x <= 290))
            return;

        transform.rotation = Quaternion.Euler(newRotation);
        AdjustDampening();
    }

    void AdjustDampening()
    {
        float verticalCos = Mathf.Abs(Mathf.Cos(Mathf.Deg2Rad * transform.localEulerAngles.x));
        Debug.Log(verticalCos);
        Debug.Log(transform.localEulerAngles);
        rb.linearDamping = verticalCos * maxDampening;
    }

    void ApplyForwardForce()
    {
        Vector3 forwardForce = transform.forward.normalized * maxGlideSpeed;
        Debug.DrawRay(transform.position, forwardForce * 10 * rb.linearDamping);
        rb.AddForce(forwardForce, ForceMode.Acceleration);
    }
}
