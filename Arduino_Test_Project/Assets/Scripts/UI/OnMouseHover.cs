using UnityEngine;
using UnityEngine.UI;

public class OnMouseHover : MonoBehaviour
{
    [SerializeField] private Image targetImage;

    [SerializeField] private Sprite hoverSprite;
    [SerializeField] private Sprite defaultSprite;

    private void Start()
    {
        OnHoverExit();
    }

    public void OnHoverEnter()
    {
        targetImage.sprite = hoverSprite;
    }

    public void OnHoverExit()
    {
        targetImage.sprite = defaultSprite;
    }
}
