using UnityEngine;
using UnityEngine.SceneManagement;

public class GroundCollision : MonoBehaviour
{
    public ArduinoSerialInput reader;
    bool gameEnded = false;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Player") && gameEnded == false)
        {
            gameEnded = true;
            ScoreTracker.Instance.SaveScoreToLeaderboard();
            LeaderBoardUI.Instance.DisplayScores();
            Invoke("EndGame", 4f);
        }
    }

    private void EndGame()
    {
        reader.ClosePort();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
