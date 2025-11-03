using TMPro;
using UnityEngine;

public class ScoreTracker : MonoBehaviour
{
    private GameObject player;
    private Rigidbody playerRb;
    private string playerName = "Player";

    float score = 0f;
    public static ScoreTracker Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = GameObject.FindWithTag("Player");
        playerRb = player.GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        score = Mathf.Abs(player.transform.position.z);
        string height = Mathf.FloorToInt(Mathf.Abs(player.transform.position.y - 14)).ToString();
        string speed = Mathf.FloorToInt(playerRb.linearVelocity.magnitude).ToString();

        GliderInfoUI.instance.SetScoreText(Mathf.FloorToInt(score).ToString());
        GliderInfoUI.instance.SetHeightText(height);
        GliderInfoUI.instance.SetSpeedText(speed);
    }

    public void SetPlayerName(string name)
    {
        playerName = name;
    }

    public void SaveScoreToLeaderboard()
    {
        Leaderboard.Instance.AddScore(Mathf.FloorToInt(score), playerName);
    }
}
