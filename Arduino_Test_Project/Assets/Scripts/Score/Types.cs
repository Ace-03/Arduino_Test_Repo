using UnityEngine;
using System.Collections.Generic;
using System.Linq;

// 1. Class to hold a single score and name pair
[System.Serializable]
public class ScoreEntry
{
    public string playerName;
    public int score;

    public ScoreEntry(string name, int s)
    {
        playerName = name;
        score = s;
    }
}

// 2. Class to act as a container for the list of entries
// JsonUtility requires a top-level object to serialize lists correctly.
[System.Serializable]
public class ScoreData
{
    public List<ScoreEntry> highScores = new List<ScoreEntry>();
}