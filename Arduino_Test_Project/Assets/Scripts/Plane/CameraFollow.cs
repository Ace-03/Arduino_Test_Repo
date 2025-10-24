using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform targetAnchor;
    public Transform targetPostition;
    public Transform targetLookat;

    public float followSpeed; 

    private Transform cam;

    public static CameraFollow instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this);
        }
    }

    [HideInInspector]
    public Vector3 targetStartingPos;

    private void Start()
    {
        cam = Camera.main.transform;
        targetStartingPos = targetPostition.localPosition;
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

    public void MoveTargetPosition(Vector3 dir, float magnitude)
    {
        targetPostition.parent = null;
        targetPostition.position = targetPostition.position + dir * magnitude;
    }

    public void ResetTargetPosition()
    {
        targetPostition.parent = targetAnchor;
        targetPostition.localPosition = targetStartingPos;
    }
}
