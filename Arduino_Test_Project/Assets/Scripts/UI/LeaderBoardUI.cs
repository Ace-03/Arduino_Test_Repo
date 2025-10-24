using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LeaderBoardUI : MonoBehaviour
{
    public GameObject LeaderBoardObject;

    // The prefab containing the Text components for a single entry
    public GameObject scoreEntryPrefab;

    // The parent Transform where new entries will be instantiated (e.g., a Vertical Layout Group)
    public Transform contentParent;

    private Leaderboard leaderboardManager;

    public static LeaderBoardUI Instance { get; private set; }

    void Start()
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

        leaderboardManager = Leaderboard.Instance;
    }

    /// <summary>
    /// Fetches scores and creates UI elements to display them.
    /// </summary>
    public void DisplayScores()
    {
        LeaderBoardObject.SetActive(true);

        // 1. Clear any existing entries
        foreach (Transform child in contentParent)
        {
            Destroy(child.gameObject);
        }

        // 2. Get the sorted list of high scores
        List<ScoreEntry> scores = leaderboardManager.GetHighScores();

        // 3. Iterate through the scores and create a UI entry for each
        for (int i = 0; i < scores.Count; i++)
        {
            ScoreEntry entry = scores[i];

            // Instantiate the prefab under the content parent
            GameObject entryObject = Instantiate(scoreEntryPrefab, contentParent);

            // Rename for organizational purposes (optional)
            entryObject.name = $"Entry_{i + 1}";

            // Get the Text components from the prefab
            TextMeshProUGUI[] texts = entryObject.GetComponentsInChildren<TextMeshProUGUI>();

            // Ensure the prefab has at least three Text components (Rank, Name, Score)
            if (texts.Length < 3)
            {
                Debug.LogError("Score Entry Prefab must contain at least 3 Text components for Rank, Name, and Score.");
                return;
            }

            // Assign the values to the Text components (order is important here)
            // You may need to adjust the index (0, 1, 2) based on how you set up your prefab.

            // Text 1: Rank (e.g., "1.", "2.", "3.")
            texts[0].text = (i + 1).ToString() + ".";

            // Text 2: Player Name
            texts[1].text = entry.playerName;

            // Text 3: Score
            texts[2].text = entry.score.ToString("N0"); // Format score with thousands separator

            // Make the entry visible (if it was initially disabled)
            entryObject.SetActive(true);
        }
    }

    public void HideLeaderBoard()
    {
        LeaderBoardObject.SetActive(false);
    }
}