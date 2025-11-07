using UnityEngine;

public class RoundStart : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponent<CameraFollow>().enabled = true;
            other.GetComponent<Booster>().enabled = false;
            other.GetComponent<GlidingSystemV2>().SetControlsEnabled(true, 1.2f);
        }
    }
}
