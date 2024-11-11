using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartButton : MonoBehaviour
{
    // Name of the scene to load
    public string sceneToLoad;

    // This method is called when the button is clicked
    public void OnStartButtonClicked()
    {
        SceneManager.LoadScene(sceneToLoad);
    }
}
