using System;
using DSTEd.Publisher.SteamWorkshop;
using System.IO.Compression;
using Steamworks;
namespace DSTEd.Publisher.Actions {
    class Update : ActionClass {
        private int ExitCode = 0;
        private bool Finished = false;
		private CallResult<RemoteStorageUpdatePublishedFileResult_t> update_result;
        //private string Preview;
        public Update() {
            this.Name           = "update";
            this.Description    = "Update a local Steam-WorkShop item. ie. publisher update k:\\mod 1234";
            this.Arguments      = "<Directory | ID>";
        }

        /// <summary>
        /// Returns path of content zip file.
        /// </summary>
        private string CreateModZipFile(string directory)
        {
            string temp = System.IO.Path.GetTempFileName();
            System.IO.File.Delete(temp);
            ZipFile.CreateFromDirectory(directory, temp, CompressionLevel.Optimal, false, System.Text.Encoding.UTF8);
            return temp;
        }

        private void OnUpdateFinished(RemoteStorageUpdatePublishedFileResult_t result, bool ioFail)
        {
            if(ioFail)
            {
                Console.WriteLine("Failed to communicate with steam workshop.");
                ExitCode = (int)Steam.ExitCodes.SteamIOError;
                Finished = true;
                return;
            }

            if(result.m_eResult != EResult.k_EResultOK)
            {
                Console.WriteLine($"Failed to update your mod. EResult is {result.m_eResult}");
                ExitCode = (int)Steam.ExitCodes.SubmitWorkshopFail;
                Finished = true;
                return;
            }

            if(result.m_bUserNeedsToAcceptWorkshopLegalAgreement)
                Console.WriteLine("You need to accept Steam Workshop Legal Aggrement.");

			Console.WriteLine($"Update mod (ID: {result.m_nPublishedFileId}) finished");

            Finished = true;
        }

        public override int Run(string[] arguments) {
            if(!Steam.Start(Steam.APP_ID)) {
                Console.WriteLine("Steam client is not running...");
                return (int)Steam.ExitCodes.InitSteamFailed;
            }

            if (arguments.Length < 2)
            {
                Console.WriteLine("Argument #1\"Directory\" and #2\"ID\" are necessary.");
                return (int)Steam.ExitCodes.ArgumentsMissing;
            }

            string directory = arguments[0];
            PublishedFileId_t ID;

            //string preview = null;
            if (!uint.TryParse(arguments[1], out uint uintID))
            {
                Console.WriteLine("ID should be a number");
                return (int)Steam.ExitCodes.InvalidArgument;
            }
            ID = new PublishedFileId_t(uintID);
            //if (arguments.Length >= 3)
            //    preview = arguments[2];

            string zip = CreateModZipFile(directory);

            byte[] content = System.IO.File.ReadAllBytes(zip);

            if(!SteamRemoteStorage.FileWrite(
                "mod_publish_data_file.zip",
                content,
                content.Length))
            {
                Console.WriteLine("Upload mod content file failed. Updating aborted.");
                return (int)Steam.ExitCodes.UploadNewContentFail;
            }
            // ----------------- Real update--------------------
            var updateHandle = SteamRemoteStorage.CreatePublishedFileUpdateRequest(ID);

            if (!SteamRemoteStorage.UpdatePublishedFileFile(updateHandle, "mod_publish_data_file.zip"))
            {
                Console.WriteLine("Failed to set new content, aborted.");
                return (int)Steam.ExitCodes.SetNewContentFail;
            }

            var commitHandle = SteamRemoteStorage.CommitPublishedFileUpdate(updateHandle);

            update_result = new CallResult<RemoteStorageUpdatePublishedFileResult_t>(OnUpdateFinished);
            update_result.Set(commitHandle);
            // -------------------end----------------------------

            Steam.Run();

            do
            {
                System.Threading.Thread.Sleep(100);
            } while (!Finished);

            Steam.Stop();

            return ExitCode;
        }
    }
}
