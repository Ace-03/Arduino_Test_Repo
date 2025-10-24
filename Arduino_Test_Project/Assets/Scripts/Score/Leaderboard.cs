// Attach this to your GameManager GameObject
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Leaderboard : MonoBehaviour
{
    private const string LeaderboardKey = "HighScores";
    public int maxScores = 10;

    // The container object to hold all the high scores
    private ScoreData scoreData = new ScoreData();

    public static Leaderboard Instance { get; private set; }

    void Awake()
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

        LoadScores();
    }

    /// <summary>
    /// Loads the saved JSON string from PlayerPrefs and deserializes it.
    /// </summary>
    private void LoadScores()
    {
        string json = PlayerPrefs.GetString(LeaderboardKey, "");

        if (!string.IsNullOrEmpty(json))
        {
            // Deserialize the JSON string back into the ScoreData object
            scoreData = JsonUtility.FromJson<ScoreData>(json);
        }
        else
        {
            // Initialize with an empty list if no data is found
            scoreData = new ScoreData();
        }

        // Ensure scores are sorted on load
        SortScores();
    }

    /// <summary>
    /// Serializes the ScoreData object into a JSON string and saves it to PlayerPrefs.
    /// </summary>
    private void SaveScores()
    {
        // Convert the ScoreData object into a JSON string
        string json = JsonUtility.ToJson(scoreData);

        // Save the JSON string
        PlayerPrefs.SetString(LeaderboardKey, json);

        // Ensure the data is written to disk
        PlayerPrefs.Save();
    }

    /// <summary>
    /// Sorts the scores and truncates the list to the maxScores limit.
    /// </summary>
    private void SortScores()
    {
        // Order by score descending and take only the top 'maxScores'
        scoreData.highScores = scoreData.highScores
            .OrderByDescending(entry => entry.score)
            .Take(maxScores)
            .ToList();
    }

    /// <summary>
    /// Attempts to add a new score and player name to the leaderboard.
    /// </summary>
    /// <param name="newScore">The score achieved.</param>
    /// <param name="playerName">The name of the player.</param>
    /// <returns>True if the score made it onto the leaderboard, otherwise false.</returns>
    public bool AddScore(int newScore, string playerName)
    {
        // Create the new entry
        ScoreEntry newEntry = new ScoreEntry(playerName, newScore);

        // 1. Add the new entry to the list
        scoreData.highScores.Add(newEntry);

        // 2. Sort and truncate the list
        SortScores();

        // 3. Check if the new entry is still in the list after sorting/truncating
        // We only save if it made the cut (i.e., its score is high enough).
        if (scoreData.highScores.Contains(newEntry))
        {
            SaveScores();
            return true; // Score was added and saved
        }

        // If the list size was maxScores and the new score was lower than the 
        // lowest score, the entry will be discarded by SortScores().
        return false; // Score was not high enough
    }

    /// <summary>
    /// Retrieves the current list of high scores.
    /// </summary>
    /// <returns>A list of ScoreEntry objects, sorted descendingly.</returns>
    public List<ScoreEntry> GetHighScores()
    {
        return scoreData.highScores;
    }

    // Optional: A method to clear the saved scores for testing
    [ContextMenu("Clear Leaderboard Scores")]
    public void ClearLeaderboard()
    {
        PlayerPrefs.DeleteKey(LeaderboardKey);
        PlayerPrefs.Save();
        LoadScores(); // Re-initialize after deletion
        Debug.Log("Leaderboard scores cleared!");
    }
}