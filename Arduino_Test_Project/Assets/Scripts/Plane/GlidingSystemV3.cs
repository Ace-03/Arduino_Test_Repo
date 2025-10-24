using System.Collections;
using UnityEngine;

public class GlidingSystemV3 : MonoBehaviour, IGlider
{
    [Header("Flight Characteristics")]
    [SerializeField]
    private float liftCoefficient = 0.1f;
    [SerializeField]
    private float dragCoefficient = 0.05f;
    [SerializeField]
    private float airDensity = 1.225f;
    [SerializeField]
    private float wingArea = 2f;

    [Header("Speed Control")]
    [SerializeField]
    private float maxGlidingSpeed = 50f;

    [Header("Turning Controls")]
    [SerializeField]
    private float pitchSpeed = 1f;
    [SerializeField]
    private float yawSpeed = 1f;
    [SerializeField]
    private float rollLimit = 45f;
    [SerializeField]
    private float verticalClampMax = 85;
    [SerializeField]
    private float verticalClampMin = -85;


    [Header("Collision & Recovery")]
    [SerializeField]
    private float impactForceMagnitude = 500f;
    [SerializeField]
    private float recoveryTime = 2f;
    [SerializeField]
    private float postImpactSpeedReduction = 0.5f;
    [SerializeField]
    private float angularVelocityTumble = 180f;

    [HideInInspector]
    public bool pitchLocked = false;

    private Rigidbody rb;
    private CameraFollow camFollow;
    private bool isRecovering = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = true;
        rb.freezeRotation = true;

        camFollow = GetComponent<CameraFollow>(); 
    }

    void FixedUpdate()
    {
        if (!isRecovering)
        {
            float verticalInput = Input.GetAxis("Vertical");
            float horizontalInput = Input.GetAxis("Horizontal");

            UpdateRotation(-verticalInput * pitchSpeed, horizontalInput * yawSpeed);

            ApplyFlightForces();
            LimitMaxSpeed();
        }
        else
        {
            rb.linearVelocity *= 0.99f;
        }
    }

    void LimitMaxSpeed()
    {
        if (rb.linearVelocity.sqrMagnitude > maxGlidingSpeed * maxGlidingSpeed)
        {
            rb.linearVelocity = rb.linearVelocity.normalized * maxGlidingSpeed;
        }
    }

    void UpdateRotation(float pitchAdjustment, float yawAdjustment)
    {
        Vector3 currentRotation = transform.localEulerAngles;
        float newPitch = currentRotation.x;

        if (!pitchLocked)
        {
            newPitch += pitchAdjustment;

            if (newPitch > 180f)
            {
                newPitch -= 360f;
            }
            newPitch = Mathf.Clamp(newPitch, verticalClampMin, verticalClampMax);
        }

        transform.Rotate(Vector3.up, yawAdjustment * Time.fixedDeltaTime * 60f, Space.Self);

        float targetRoll = yawAdjustment * rollLimit;
        float newRoll = Mathf.LerpAngle(currentRotation.z, targetRoll, Time.fixedDeltaTime * 5f);

        transform.localRotation = Quaternion.Euler(newPitch, transform.localEulerAngles.y, newRoll);
    }

    void ApplyFlightForces()
    {
        Vector3 velocity = rb.linearVelocity;
        float speed = velocity.magnitude;

        if (speed < 0.1f) return;

        Vector3 dragDirection = -velocity.normalized;
        float dragMagnitude = 0.5f * airDensity * speed * speed * dragCoefficient * wingArea;
        Vector3 dragForce = dragDirection * dragMagnitude;
        rb.AddForce(dragForce, ForceMode.Force);

        float aoaDegrees = Vector3.SignedAngle(velocity, transform.forward, transform.right);
        float aoaRadians = aoaDegrees * Mathf.Deg2Rad;

        float liftMagnitude = 0.5f * airDensity * speed * speed * liftCoefficient * wingArea * Mathf.Sin(aoaRadians);

        Vector3 liftDirection = Vector3.Cross(velocity.normalized, transform.right).normalized;

        Vector3 liftForce = liftDirection * liftMagnitude;
        rb.AddForce(liftForce, ForceMode.Force);
    }

    public void TriggerImpact(Vector3 impactNormal)
    {
        if (!isRecovering)
        {
            isRecovering = true;
            rb.AddForce((-impactNormal + Vector3.up).normalized * impactForceMagnitude, ForceMode.Impulse);
            rb.freezeRotation = false;
            rb.angularVelocity = Vector3.zero;
            rb.AddTorque(Random.insideUnitSphere * angularVelocityTumble, ForceMode.VelocityChange);
            camFollow.MoveTargetPosition(-impactNormal + Vector3.up, impactForceMagnitude);
            StartCoroutine(HandleImpactAndRecovery());
        }
    }

    private IEnumerator HandleImpactAndRecovery()
    {
        yield return new WaitForSeconds(recoveryTime);

        Vector3 currentVelocity = rb.linearVelocity;
        float newSpeed = currentVelocity.magnitude * postImpactSpeedReduction;
        Vector3 newDirection = currentVelocity.normalized;

        transform.rotation = Quaternion.LookRotation(newDirection);
        rb.freezeRotation = true;
        rb.angularVelocity = Vector3.zero;

        rb.linearVelocity = newDirection * newSpeed;

        isRecovering = false;
        pitchLocked = false;

        camFollow.ResetTargetPosition();
    }
}