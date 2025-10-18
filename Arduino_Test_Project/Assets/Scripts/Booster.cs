using UnityEngine;
using System.Collections; // Required for Coroutines

/// <summary>
/// Adds a forward boost/dash functionality when the Spacebar is pressed.
/// This script should be attached to the same GameObject as GlidingSystemV2.
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class Booster : MonoBehaviour
{
    [Header("Boost Characteristics")]
    [Tooltip("The force applied instantly when boosting.")]
    [SerializeField]
    private float boostForceMagnitude = 100f;

    [Tooltip("Time in seconds before another boost can be initiated.")]
    [SerializeField]
    private float boostCooldown = 2.0f;

    [Tooltip("Maximum number of boosts available before 'refueling'.")]
    [SerializeField]
    private int maxBoostCharges = 3;

    [Tooltip("Time in seconds it takes to regenerate one boost charge.")]
    [SerializeField]
    private float chargeRegenTime = 5.0f;

    private Rigidbody rb;
    private int currentBoostCharges;
    private bool isRegenerating = false;

    // Use to track if the cooldown is active, independent of charges
    private float lastBoostTime;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            Debug.LogError("Booster script requires a Rigidbody component on the same GameObject.");
            enabled = false; // Disable the script if no Rigidbody is found
            return;
        }

        currentBoostCharges = maxBoostCharges;
        lastBoostTime = -boostCooldown; // Allow immediate boost on start
    }

    void Update()
    {
        // Check for boost input only in Update (for instant response)
        if (Input.GetKeyDown(KeyCode.Space))
        {
            AttemptBoost();
        }

        // Start the regeneration process if not all charges are full and not already regenerating
        if (currentBoostCharges < maxBoostCharges && !isRegenerating)
        {
            StartCoroutine(RegenerateChargeRoutine());
        }
    }

    /// <summary>
    /// Checks conditions and applies the forward boost force.
    /// </summary>
    void AttemptBoost()
    {
        // 1. Check for cooldown
        if (Time.time < lastBoostTime + boostCooldown)
        {
            Debug.Log($"Boost on cooldown. Remaining: {lastBoostTime + boostCooldown - Time.time:F2}s");
            return;
        }

        // 2. Check for available charges
        if (currentBoostCharges <= 0)
        {
            Debug.Log("No boost charges remaining. Wait for regeneration.");
            return;
        }

        // Apply the boost
        Vector3 boostDirection = -transform.forward;
        // Use ForceMode.Impulse for an instantaneous, short, and quick application of force.
        rb.AddForce(boostDirection * boostForceMagnitude, ForceMode.Impulse);

        // Consume charge and reset cooldown
        currentBoostCharges--;
        lastBoostTime = Time.time;

        Debug.Log($"Boost activated! Charges remaining: {currentBoostCharges}");
    }

    /// <summary>
    /// Coroutine to handle the timed regeneration of a boost charge.
    /// </summary>
    IEnumerator RegenerateChargeRoutine()
    {
        isRegenerating = true;
        yield return new WaitForSeconds(chargeRegenTime);

        if (currentBoostCharges < maxBoostCharges)
        {
            currentBoostCharges++;
            Debug.Log($"Boost charge regenerated! Total charges: {currentBoostCharges}");
        }

        isRegenerating = false;
        // Note: The Update function will restart the coroutine if charges are still below max.
    }

    // Optional: Public getter for UI display
    public int GetCurrentBoostCharges()
    {
        return currentBoostCharges;
    }

    public int GetMaxBoostCharges()
    {
        return maxBoostCharges;
    }
}