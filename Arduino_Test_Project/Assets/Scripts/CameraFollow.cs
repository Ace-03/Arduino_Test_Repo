using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform targetPostition;
    public Transform targetLookat;

    public float followSpeed; 

    private Transform cam;

    private void Start()
    {
        cam = Camera.main.transform;
    }

    private void FixedUpdate()
    {
        MoveToTarget(targetPostition.position);
        cam.LookAt(targetLookat.position);
    }

    void MoveToTarget(Vector3 targetPosition)
    {
        cam.position = Vector3.Lerp(cam.position, targetPosition, Time.deltaTime * followSpeed);
    }
}
