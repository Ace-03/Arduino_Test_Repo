using TMPro;
using UnityEngine;

public class GliderInfoUI : MonoBehaviour
{
    public TextMeshProUGUI score;
    public TextMeshProUGUI height;
    public TextMeshProUGUI speed;

    public static GliderInfoUI instance;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this);
    }

    public void SetScoreText(string text)
    {
        score.text = "Score: " + text;
    }
    public void SetHeightText(string text)
    {
        height.text = "Height: " + text + "m";
    }
    public void SetSpeedText(string text)
    {
        speed.text = "Speed: " + text + "mph";
    }
}
