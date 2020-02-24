using System;
using System.Collections.Generic;
using System.Text;
using RemoteStorage = Steamworks.SteamRemoteStorage;
using Steamworks;
namespace DSTEd.Publisher.Actions {
    class List : ActionClass {
        public List() {
            this.Name               = "list";
            this.Description       = "Displays a list of all published Steam-WorkShop items.";
            this.Arguments        = "<Begin page (optional)>";
        }

        private static int exitcode = 0;//everything ok

        private static void OnGetList(RemoteStorageEnumerateWorkshopFilesResult_t result, bool ioFail)
        {
            if (ioFail)
            {
                Console.WriteLine("Failed to communicate with steam workshop");
                exitcode = (int)ExitCodes.SteamIOError;
            }

            if(result.m_eResult != EResult.k_EResultOK)
            {
                Console.WriteLine("Query failed, reason: " + result.m_eResult.ToString());
                exitcode = (int)ExitCodes.QueryWorkshopFail;
            }
            Console.WriteLine("You have {0} mod published on DST workshop", result.m_nTotalResultCount);

            var callResults = new CallResult<RemoteStorageGetPublishedFileDetailsResult_t>[result.m_rgPublishedFileId.Length];
            for (int i = 0; i < result.m_rgPublishedFileId.Length; i++)
            {
                callResults[i] = new CallResult<RemoteStorageGetPublishedFileDetailsResult_t>(
                    delegate (RemoteStorageGetPublishedFileDetailsResult_t modDetails, bool ioFail)
                    {
                        if(ioFail)
                            Console.WriteLine("Failed to get details from steam. The mod ID is " + modDetails.m_nPublishedFileId);

                        if(modDetails.m_nConsumerAppID == new AppId_t(322330))
                            Console.WriteLine(
                                "Title: {0}\n" +
                                "Tags: {1}\n" +
                                "Description:\n{2}\n" +
                                "ID: {3}\n",
                                modDetails.m_rgchTitle,
                                modDetails.m_rgchTags,
                                modDetails.m_rgchDescription,
                                modDetails.m_nPublishedFileId
                                );
                    });
            }
            
        }

        public override int Run(string[] arguments) {
            //Console.WriteLine("--list is currently not implemented.");
            uint begin = 0;

            if (arguments.Length > 1)
                begin = uint.Parse(arguments[1]);
            
            var handle = RemoteStorage.EnumerateUserPublishedFiles(begin);

            var callResult = new CallResult<RemoteStorageEnumerateWorkshopFilesResult_t>();
            callResult.Set(handle, OnGetList);
           
            SteamAPI.RunCallbacks();

            do
            {
                System.Threading.Thread.Sleep(100);
            } while (callResult.IsActive());

            return exitcode;
        }
    }
}
