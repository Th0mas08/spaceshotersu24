using UnityEngine;
using TMPro; // For TextMeshProUGUI

public class GameManager : MonoBehaviour
{
    public static GameManager Instance; // Singleton instance of the GameManager
    private int pointsCount = 0; // Tracks total points across all enemies
    [SerializeField] private TextMeshProUGUI pointsText; // Reference to the UI Text component

    private void Awake()
    {
        // Ensure there's only one instance of GameManager
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Keep the GameManager alive across scenes
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // Load points from PlayerPrefs (if any) at the start of the scene
        pointsCount = PlayerPrefs.GetInt("Points", 0); // Default to 0 if not found
        UpdatePointsText();
    }

    // Method to add points when an enemy is defeated
    public void AddPoints(int points)
    {
        pointsCount += points;
        Debug.Log("Points added: " + points + ". Total points: " + pointsCount);

        // Save the new points to PlayerPrefs
        PlayerPrefs.SetInt("Points", pointsCount);
        PlayerPrefs.Save();

        UpdatePointsText();
    }

    // Update the displayed points on the screen
    private void UpdatePointsText()
    {
        pointsText.text = "Points: " + pointsCount;
    }

    private void OnApplicationQuit()
    {
        // Ensure points are saved when the application closes
        PlayerPrefs.SetInt("Points", pointsCount);
        PlayerPrefs.Save();
    }

    // Public method to get the current points
    public int GetPoints()
    {
        return pointsCount;
    }
}
