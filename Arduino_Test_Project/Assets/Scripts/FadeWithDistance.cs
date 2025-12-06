using UnityEngine;

public class FadeWithDistance : MonoBehaviour
{
    [SerializeField] private float transparentDistance;
    [SerializeField] private float opaqueDistance;

    private float curDistance;

    private Renderer objectRenderer;
    void Start()
    {
        objectRenderer = GetComponent<Renderer>();
    }

    void Update()
    {
        curDistance = Mathf.Abs(transform.position.z - Camera.main.transform.position.z);

        //Debug.Log($"this pos: {transform.position.z}, cam pos: {Camera.main.transform.position.z}");

        curDistance = Mathf.Clamp(curDistance, opaqueDistance, transparentDistance);
        
        //Debug.Log($"curDist: {curDistance}");

        float transparencyValue = Mathf.InverseLerp(transparentDistance, opaqueDistance, curDistance);

        //Debug.Log($"Transparency Value is: {transparencyValue}");

        Color materialColor = objectRenderer.material.color;
        Color newMatColor = new Color(materialColor.r, materialColor.g, materialColor.b, transparencyValue);
        //Debug.Log($"New Mat Color is: {newMatColor}");

        objectRenderer.material.color = newMatColor;

    }
}
