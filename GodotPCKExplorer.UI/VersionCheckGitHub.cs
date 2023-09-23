using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Reflection;

internal class VersionCheckerGitHub : IDisposable
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
    }

    public enum MSGDialogResult
    {
        OK = 0,
        Yes = 6,
        No = 7,
        Cancel = 2,
    }

    public Version SkipVersion = null;
    /// <summary>
    /// Emits after the user has chosen to skip the version. Also this class will automatically update <see cref="SkipVersion"/>
    /// </summary>
    public event EventHandler<VersionSkipByUserData> VersionSkippedByUser;

    public delegate MSGDialogResult MessageBoxDelegate(MSGType type, Dictionary<string, string> customData, Exception ex);

    WebClient updateClient = null;
    bool _isSilentCheck = true;
    MessageBoxDelegate show_msg_box = null;

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
            return;

        updateClient = new WebClient();
        updateClient.DownloadStringCompleted += UpdateClient_DownloadStringCompleted;
        updateClient.Headers.Add("Content-Type", "application/json");
        updateClient.Headers.Add("User-Agent", appName);

        try
        {
            _isSilentCheck = isSilentCheck;
            updateClient.DownloadStringAsync(githubLink);
        }
        catch (Exception ex)
        {
            if (!_isSilentCheck)
                ShowMessageBox(MSGType.FailedToRequestInfo, ex: ex);
            ClearUpdateData();
        }
    }

    private void UpdateClient_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
    {
        if (e.Error is WebException webExp)
        {
            if (!_isSilentCheck)
                ShowMessageBox(MSGType.FailedToGetInfo, ex: webExp);

            ClearUpdateData();
            return;
        }

        try
        {
            dynamic resultObject = JsonConvert.DeserializeObject(e.Result);
            Version newVersion = new Version(resultObject.tag_name.Value);
            Version currentVersion = Assembly.GetExecutingAssembly().GetName().Version;
            string updateUrl = resultObject.html_url.Value;

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
                        Process.Start(updateUrl);
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

        ClearUpdateData();
    }

    void ClearUpdateData()
    {
        updateClient?.Dispose();
        updateClient = null;
        SkipVersion = null;

        show_msg_box = null;
        VersionSkippedByUser = null;
    }

    public void Dispose()
    {
        ClearUpdateData();
    }

    MSGDialogResult ShowMessageBox(MSGType type, Dictionary<string, string> customData = null, Exception ex = null)
    {
        if (show_msg_box != null)
        {
            return show_msg_box.Invoke(type, customData, ex);
        }
        return MSGDialogResult.Cancel;
    }

    public class VersionSkipByUserData : EventArgs
    {
        public Version SkippedVersion;

        public VersionSkipByUserData(Version skippedVersion)
        {
            SkippedVersion = skippedVersion;
        }
    }
}
