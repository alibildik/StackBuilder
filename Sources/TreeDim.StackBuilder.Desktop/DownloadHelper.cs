#region Using directives
using System;
using System.IO;
using System.Windows;
using System.Net;
using System.Diagnostics;
using Syroot.Windows.IO;

using log4net;

using treeDiM.StackBuilder.Desktop.Properties;
#endregion

namespace treeDiM.StackBuilder.Desktop
{
    public class DownloadHelper
    {
        public static bool DownloadFile(string fileURL, bool openFile = false, string downloadPath = "")
        {
            try
            {
                var knownFolder = new KnownFolder(KnownFolderType.Downloads);

                // set default download path
                if (string.IsNullOrEmpty(downloadPath))
                    downloadPath = Path.Combine(knownFolder.Path, Path.GetFileName(fileURL));
                // download file
                using (var client = new WebClient())
                { client.DownloadFile(fileURL, downloadPath); }
                // try and open file
                if (File.Exists(downloadPath))
                {
                    if (openFile)
                        Process.Start(downloadPath);
                    return true;
                }
            }
            catch (WebException ex) { _log.Error(ex.Message); }
            catch (Exception ex) { _log.Error(ex.Message); }
            MessageBox.Show(string.Format(Resources.ID_ERROR_FAILEDTODOWNLOADFILE, fileURL, downloadPath));
            return false;
        }

        static ILog _log = LogManager.GetLogger(typeof(DownloadHelper));
    }
}
