using UnityEngine;

public class Esc_key : MonoBehaviour
{
    // Reference to the GameObject you want to toggle
    public GameObject OptionsMenu;

    // Update is called once per frame
    void Update()
    {
        // Check if the Escape key is pressed
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            // Toggle the active state of the GameObject
            OptionsMenu.SetActive(!OptionsMenu.activeSelf);
        }
    }
}

