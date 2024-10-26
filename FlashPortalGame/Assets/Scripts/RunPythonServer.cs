using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI;
using ZenFulcrum.EmbeddedBrowser;

public class RunPythonServer : MonoBehaviour
{
    public Button loadHTML1Button;
    public Button loadHTML2Button;
    public Button closeButton;
    public Browser browser; // Drag your Browser GameObject here in the Inspector

    private Process pythonProcess;
    private bool serverStarted = false;

    void Start()
    {
        // Initially hide the browser
        browser.gameObject.SetActive(false);
    }

    void OnApplicationQuit()
    {
        StopPythonServer();
    }

    private void StartPythonServer()
    {
        if (serverStarted) return; // Prevents starting the server multiple times

        string pythonPath = "python"; // Adjust to your Python executable path if needed
        string arguments = "-m http.server 8080";
        string workingDirectory = Application.streamingAssetsPath;

        pythonProcess = new Process();
        pythonProcess.StartInfo.FileName = pythonPath;
        pythonProcess.StartInfo.Arguments = arguments;
        pythonProcess.StartInfo.WorkingDirectory = workingDirectory;
        pythonProcess.StartInfo.CreateNoWindow = true;
        pythonProcess.StartInfo.UseShellExecute = false;
        pythonProcess.StartInfo.RedirectStandardOutput = true;
        pythonProcess.StartInfo.RedirectStandardError = true;

        pythonProcess.OutputDataReceived += (sender, args) => UnityEngine.Debug.Log(args.Data);
        pythonProcess.ErrorDataReceived += (sender, args) => UnityEngine.Debug.LogError(args.Data);

        pythonProcess.Start();
        pythonProcess.BeginOutputReadLine();
        pythonProcess.BeginErrorReadLine();

        UnityEngine.Debug.Log("Python server started on http://localhost:8080");
        serverStarted = true;
    }

    private void StopPythonServer()
    {
        if (pythonProcess != null && !pythonProcess.HasExited)
        {
            pythonProcess.Kill();
            pythonProcess.Dispose();
            UnityEngine.Debug.Log("Python server stopped.");
            serverStarted = false;
        }
    }

    public void LoadHTML(string filePath)
    {
        StartPythonServer(); // Start server if it hasn't started

        // Construct the URL and load it in the browser
        string url = $"http://localhost:8080/{filePath}";
        if (browser != null)
        {
            browser.LoadURL(url, true);
            browser.gameObject.SetActive(true); // Show the browser

            // Set the browser size after loading the URL
            RectTransform rt = browser.GetComponent<RectTransform>();
            rt.sizeDelta = new Vector2(460, 380); // Set your desired width and height

            UnityEngine.Debug.Log($"Loaded HTML file at {url}");
        }
        else
        {
            UnityEngine.Debug.LogError("Browser component is not attached.");
        }
    }

    public void CloseHTML()
    {
        if (browser != null)
        {
            browser.LoadURL("about:blank", true); // Stop any active content in the browser
            browser.gameObject.SetActive(false); // Hide the browser
            UnityEngine.Debug.Log("HTML file closed.");
        }

        StopPythonServer(); // Optionally stop the server if needed
    }
}
