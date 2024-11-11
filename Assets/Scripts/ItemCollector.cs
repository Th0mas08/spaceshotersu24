using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ItemCollector : MonoBehaviour
{
    private int pointsCount = 0; // Use gemCount instead of Gem for clarity

    [SerializeField] private TextMeshProUGUI gemText; // Make sure this is set to TextMeshProUGUI

    private void Start()
    {
        UpdateGemText(); // Initialize the text display
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Points"))
        {
            Debug.Log("Points collected!"); // Log when a gem is collected
            Destroy(collision.gameObject);
            pointsCount++;
            UpdateGemText();
        }
    }

    private void UpdateGemText()
    {
        gemText.text = "Points: " + pointsCount; // Update the UI text
    }
}
