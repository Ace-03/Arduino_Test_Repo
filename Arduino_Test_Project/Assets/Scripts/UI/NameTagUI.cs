using UnityEngine;

public class NameTagUI : MonoBehaviour
{
    [SerializeField] private Booster booster;
    [SerializeField] GameObject buttonObject;
    [SerializeField] GameObject nameTagObject;
    public void OnStartGame()
    {
        booster.enabled = true;
        buttonObject.SetActive(false);
        nameTagObject.SetActive(false);
    }
}
