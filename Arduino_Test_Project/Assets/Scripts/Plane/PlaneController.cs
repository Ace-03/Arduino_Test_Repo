using System;
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

    private void FixedUpdate()
    {
        ApplyAerodynamics();

        HandlePlayerInput();

        AlignToVelocity();
    }

    private void ApplyAerodynamics()
    {
        float forwardSpeed = rb.linearVelocity.magnitude;

        if (forwardSpeed < 0.1f) return;

        float angleOfAttack = Vector3.Angle(rb.linearVelocity, transform.forward);
        float liftForce = (float)(0.5 * airDensity * Mathf.Pow(forwardSpeed, 2) * dragCoefficient * wingArea);
        rb.AddForce(transform.up * liftForce, ForceMode.Force);

        float dragForce = (float)(0.5 * airDensity * Mathf.Pow(forwardSpeed, 2) * dragCoefficient * wingArea);
        rb.AddForce(-rb.linearVelocity.normalized * dragForce, ForceMode.Force);
    }

    private void HandlePlayerInput()
    {
        float pitchInput = Input.GetAxis("Vertical");
        float rollInput = Input.GetAxis("Horizontal");

        rb.AddTorque(transform.right * pitchInput * pitchTorque, ForceMode.Force);
        rb.AddTorque(transform.forward * -rollInput * rollTorque, ForceMode.Force);
    }

    private void AlignToVelocity()
    {
        if (rb.linearVelocity.magnitude > 0.01f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(rb.linearVelocity, Vector3.up);
            rb.MoveRotation(targetRotation);
        }
    }
}
