using System.Diagnostics;
using System.Reflection;
using System.Text.Json;

internal sealed class VersionCheckerGitHub : IDisposable
{
    public enum MSGType
    {
        /// <summary>
        /// Used when information about a new update is received
        /// Expected buttons: Yes, No, Cancel
        /// Yes: Open the update link
        /// No: Skip version
        /// Cancel: Close the message window
        /// </summary>
        InfoUpdateAvailable,
        /// <summary>
        /// Used when no new version is found
        /// </summary>
        InfoUsingLatestVersion,
        /// <summary>
        /// Used when a request to GitHub failed
        /// </summary>
        FailedToRequestInfo,
        /// <summary>
        /// Used when the request failed with an error
        /// </summary>
        FailedToGetInfo,
        /// <summary>
        /// Used when the received data could not be processed
        /// </summary>
        FailedToProcessData,
        /// <summary>
        /// Already requesting
        /// </summary>
        FailedAlreadyRequesting,
    }

    public enum MSGDialogResult
    {
        OK = 0,
        Yes = 6,
        No = 7,
        Cancel = 2,
    }

    public Version? SkipVersion = null;
    /// <summary>
    /// Emits after the user has chosen to skip the version. Also this class will automatically update <see cref="SkipVersion"/>
    /// </summary>
    public event EventHandler<VersionSkipByUserData>? VersionSkippedByUser;

    public delegate MSGDialogResult MessageBoxDelegate(MSGType type, Dictionary<string, string> customData, Exception? ex);

    HttpClient? updateClient = null;
    bool _isSilentCheck = true;
    MessageBoxDelegate? show_msg_box = null;

    CancellationTokenSource? cts = null;
    Task? async_get = null;

    readonly Uri githubLink;
    readonly string appName;

    /// <summary>
    /// Init version checker with <paramref name="profile"/> name and <paramref name="repo"/> name.
    /// It will be formated like this: <code>$"https://api.github.com/repos/{profile}/{repo}/releases/latest"</code>
    /// </summary>
    /// <param name="profile"></param>
    /// <param name="repo"></param>
    public VersionCheckerGitHub(string profile, string repo, string appName, MessageBoxDelegate showMSG)
    {
        githubLink = new Uri($"https://api.github.com/repos/{profile}/{repo}/releases/latest");
        this.appName = appName;
        this.show_msg_box = showMSG;
    }

    public void CheckForUpdates(bool isSilentCheck = false)
    {
        // Skip if currently checking
        if (updateClient != null)
        {
            if (!_isSilentCheck)
                ShowMessageBox(MSGType.FailedAlreadyRequesting);
            return;
        }

        updateClient = new HttpClient();
        updateClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
        updateClient.DefaultRequestHeaders.UserAgent.Add(new System.Net.Http.Headers.ProductInfoHeaderValue(appName, Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? "N/A"));

        try
        {
            KillAsyncTask();

            _isSilentCheck = isSilentCheck;
            cts = new();
            updateClient.GetAsync(githubLink, cts.Token).ContinueWith(UpdateClient_DownloadStringCompleted, cts.Token);
        }
        catch (Exception ex)
        {
            if (!_isSilentCheck)
                ShowMessageBox(MSGType.FailedToRequestInfo, ex: ex);
            ClearUpdateData();
        }
    }

    private void UpdateClient_DownloadStringCompleted(Task<HttpResponseMessage> taskRes)
    {
        var res = taskRes.Result;
        if (!res.IsSuccessStatusCode)
        {
            if (!_isSilentCheck)
                ShowMessageBox(MSGType.FailedToGetInfo, customData: new Dictionary<string, string> { { "respose", res.StatusCode.ToString() } });

            ClearUpdateData();
            return;
        }

        try
        {
            JsonDocument resultObject = JsonDocument.Parse(new StreamReader(res.Content.ReadAsStream()).ReadToEnd()) ?? throw new NullReferenceException();
            var json = resultObject.RootElement;

            Version newVersion = new(json.GetProperty("tag_name").GetString() ?? throw new NullReferenceException());
            Version currentVersion = Assembly.GetExecutingAssembly().GetName().Version ?? new Version();
            string updateUrl = json.GetProperty("html_url").GetString() ?? throw new NullReferenceException();

            // Skip if the new version matches the skip version, or don't skip if checking manually
            if (newVersion != SkipVersion || !_isSilentCheck)
            {
                // New release
                if (newVersion > currentVersion)
                {
                    var updateDialog = ShowMessageBox(MSGType.InfoUpdateAvailable, new Dictionary<string, string> { { "new_version", newVersion.ToString() }, { "current_version", currentVersion.ToString() } });
                    if (updateDialog == MSGDialogResult.Yes)
                    {
                        // Open the download page
                        Process.Start(new ProcessStartInfo(updateUrl) { UseShellExecute = true });
                    }
                    else if (updateDialog == MSGDialogResult.No)
                    {
                        SkipVersion = newVersion;
                        VersionSkippedByUser?.Invoke(this, new VersionSkipByUserData(newVersion));
                    }
                }
                else
                {
                    // Don't show this on startup
                    if (!_isSilentCheck)
                    {
                        ShowMessageBox(MSGType.InfoUsingLatestVersion, new Dictionary<string, string> { { "current_version", currentVersion.ToString() } });
                    }
                }
            }
        }
        catch (Exception ex)
        {
            // Don't show this on startup
            if (!_isSilentCheck)
            {
                ShowMessageBox(MSGType.FailedToProcessData, ex: ex);
            }
        }
        finally
        {
            ClearUpdateData();
        }
    }

    void ClearUpdateData()
    {
        updateClient?.Dispose();
        updateClient = null;
        SkipVersion = null;
    }

    void KillAsyncTask()
    {
        if (async_get != null && cts != null)
        {
            cts.Cancel();
            async_get.Wait();
            async_get.Dispose();
            cts.Dispose();
            cts = null;
            async_get = null;
        }
    }

    public void Dispose()
    {
        show_msg_box = null;
        VersionSkippedByUser = null;

        ClearUpdateData();
        KillAsyncTask();
    }

    MSGDialogResult ShowMessageBox(MSGType type, Dictionary<string, string>? customData = null, Exception? ex = null)
    {
        if (show_msg_box != null)
        {
            return show_msg_box.Invoke(type, customData ?? [], ex);
        }
        return MSGDialogResult.Cancel;
    }

    public class VersionSkipByUserData(Version skippedVersion) : EventArgs
    {
        public Version SkippedVersion = skippedVersion;
    }
}
