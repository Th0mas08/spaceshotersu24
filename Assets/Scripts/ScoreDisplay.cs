using UnityEngine;
using TMPro;

public class ScoreDisplay : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI pointsText; // Reference to the UI Text component

    void Start()
    {
        // Get points from the GameManager (this will persist across scenes)
        int points = GameManager.Instance.GetPoints();
        pointsText.text = "your high score is: " + points.ToString();
    }
}
