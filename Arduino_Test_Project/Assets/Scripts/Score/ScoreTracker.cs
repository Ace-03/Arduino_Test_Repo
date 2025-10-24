using TMPro;
using UnityEngine;

public class ScoreTracker : MonoBehaviour
{
    public TextMeshProUGUI scoreText;
    
    private GameObject player;
    private string playerName = "Player";

    public static ScoreTracker Instance { get; private set; }

    private float score = 0f;

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
    }

    // Update is called once per frame
    void Update()
    {
        score = Mathf.Abs(player.transform.position.z);
        scoreText.text = "Score: " + Mathf.FloorToInt(score).ToString();
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
