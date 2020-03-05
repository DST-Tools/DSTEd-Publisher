using System;
using System.IO;
using System.IO.Compression;

using DSTEd.Publisher.SteamWorkshop;
using Steamworks;
namespace DSTEd.Publisher.Actions {
    class Download : ActionClass {
        private int ExitCode = (int)Steam.ExitCodes.NoError;
        private string Location;
        public Download() {
            this.Name           = "download";
            this.Description    = "Download a Steam-WorkShop item.";
            this.Arguments      = "<ID | Location(optional)>";
        }

        private void OnDownloadFinished(DownloadItemResult_t result)
        {
            if(result.m_eResult != EResult.k_EResultOK)
            {
                Console.WriteLine($"Download failed, EResult is {result.m_eResult}.");
                ExitCode = (int)Steam.ExitCodes.DownLoadFail;
            }

            Console.WriteLine($"Download mod {result.m_nPublishedFileId} succeed.");

            if (!SteamUGC.GetItemInstallInfo(result.m_nPublishedFileId, out ulong size, out string zipPath, 65536, out uint timestamp))
            {
                Console.WriteLine("But failed to get info about downloaded mod due to some unknown reason.");
                ExitCode = (int)Steam.ExitCodes.DownLoadFail;
            }

            using var modZip = new ZipArchive(new FileStream(zipPath, FileMode.Open));
            Location = Path.GetFullPath(Location);
            modZip.ExtractToDirectory(Location);

            Console.WriteLine($"Successfully downloaded mod {result.m_nPublishedFileId} to {Location}.");
        }

        public override int Run(string[] arguments) {
            if(!Steam.Start(Steam.APP_GAME)) {
                Console.WriteLine("Steam is not running...");
                return (int)Steam.ExitCodes.InitSteamFailed;
            }

            if(arguments.Length == 0)
            {
                Console.WriteLine("Argument \"ID\"(#1) is necessary");
                return (int)Steam.ExitCodes.ArgumentsMissing;
            }

            if(!uint.TryParse(arguments[0],out uint ID))
            {
                Console.WriteLine("Argument \"ID\"(#1) should be a number");
                return (int)Steam.ExitCodes.InvalidArgument;
            }

            SteamApps.GetAppInstallDir(Steam.APP_GAME, out Location, 1024);

            if (arguments.Length >= 2)
                Location = arguments[1];

            if (!SteamUGC.DownloadItem(new PublishedFileId_t(ID), false))
                ExitCode = (int)Steam.ExitCodes.DownLoadFail;

            var callback = new Callback<DownloadItemResult_t>(OnDownloadFinished);
            callback.Register(OnDownloadFinished);

            System.Threading.Thread.Sleep(1500);
            new System.Threading.Thread(() => SteamAPI.RunCallbacks()).Start();

            SteamAPI.RunCallbacks();

            Steam.Stop();

            return ExitCode;
        }
    }
}
