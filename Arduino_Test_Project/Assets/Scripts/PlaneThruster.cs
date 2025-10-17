using UnityEngine;

public class PlaneThruster : MonoBehaviour
{
    public float boostThrust;
    
    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            ApplyBurstThrust(boostThrust);
        }
    

    }
    
    public void ApplyBurstThrust(float amount)
    {
        rb.AddForce(-transform.forward * amount * 10, ForceMode.Impulse);
    }
}
