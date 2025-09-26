using UnityEngine;

public class GlidingSystem : MonoBehaviour
{
    [SerializeField] 
    private float maxGlideSpeed = 10f;
    [SerializeField]
    private float maxDampening = 10f;

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        float verticalInput = Input.GetAxis("Vertical");
        UpdateRotation(verticalInput);
        ApplyForwardForce();
    }


    void UpdateRotation(float rotationAdjustment)
    {
        Vector3 currentRotation = transform.localEulerAngles;
        Vector3 newRotation = new Vector3(currentRotation.x, currentRotation.y, currentRotation.z + rotationAdjustment);

        transform.rotation = Quaternion.Euler(newRotation);
        AdjustDampening();
    }

    void AdjustDampening()
    {
        float verticalCos = Mathf.Abs(Mathf.Cos(Mathf.Deg2Rad * transform.localEulerAngles.z));
        Debug.Log(verticalCos);
        Debug.Log(transform.localEulerAngles);
        rb.linearDamping = verticalCos * maxDampening;
    }

    void ApplyForwardForce()
    {
        Vector3 forwardForce = transform.localEulerAngles * maxGlideSpeed;
        rb.AddForce(forwardForce, ForceMode.Acceleration);
    }
}
