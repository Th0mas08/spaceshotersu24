
using UnityEngine;
using UnityEngine.UI;

public class QuitButton : MonoBehaviour
{
    // This method is called when the button is clicked
    public void OnQuitButtonClicked()
    {
#if UNITY_EDITOR
        // If we're in the editor, stop play mode
        UnityEditor.EditorApplication.isPlaying = false;
#else
        // If we're in a build, quit the application
        Application.Quit();
#endif
    }
}
