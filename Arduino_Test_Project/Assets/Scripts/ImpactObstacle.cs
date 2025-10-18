using UnityEngine;

public class ImpactObstacle : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        // 1. Check if the collided object has the GlidingSystemV2 script
        GlidingSystemV2 glider = collision.gameObject.GetComponent<GlidingSystemV2>();

        if (glider != null)
        {
            // 2. Get the collision normal (direction of impact)
            // We use the first contact point's normal
            Vector3 impactNormal = collision.contacts[0].normal;

            // 3. Call the public method on the glider to trigger the reaction
            glider.TriggerImpact(impactNormal);

            Collider col = GetComponent<Collider>();
            if (col != null) col.enabled = false;
            Invoke("EnableCollider", 0.1f); 
        }
    }

    private void EnableCollider()
    {
        Collider col = GetComponent<Collider>();
        if (col != null) col.enabled = true;
    }
}