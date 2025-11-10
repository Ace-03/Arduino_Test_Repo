using UnityEngine;

public class CameraLayer : MonoBehaviour
{
    [SerializeField] private float distance = 50;
    void Start()
    {
        Camera camera = GetComponent<Camera>();
        float[] distances = new float[32];
        distances[16] = distance;
        camera.layerCullDistances = distances;
    }
}
