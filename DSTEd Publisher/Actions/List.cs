using System;
using System.Collections.Generic;
//using System.Text;
using Steamworks;
namespace DSTEd.Publisher.Actions {
    class List : ActionClass {
        public List() {
            this.Name               = "list";
            this.Description       = "Displays a list of all published Steam-WorkShop items.";
            this.Arguments        = "<Begin page (optional)>";
        }

        private int exitcode = 0;//everything ok
        private bool finished = false;
        private uint page = 1;
        private void OnGetListUGC(SteamUGCQueryCompleted_t queryResult, bool ioFail)
        {
            if(ioFail)
            {
                Console.WriteLine("Failed to communicate with steam workshop.");
                exitcode = (int)ExitCodes.SteamIOError;
                finished = true;
                return;
            }

            if(queryResult.m_eResult != EResult.k_EResultOK)
            {
                Console.WriteLine($"Query Failed. EResult is {queryResult.m_eResult}");
                exitcode = (int)ExitCodes.QueryWorkshopFail;
                finished = true;
                return;
            }

            Console.WriteLine($"You have published {queryResult.m_unTotalMatchingResults} mod(s) in total.\n" +
                $"This is page {page} (50 per page)");

            for (uint i = 0; i < queryResult.m_unNumResultsReturned; i++)
            {
                SteamUGC.GetQueryUGCResult(queryResult.m_handle, i, out SteamUGCDetails_t details);
                if (details.m_nConsumerAppID == new AppId_t(322330) && details.m_eFileType == EWorkshopFileType.k_EWorkshopFileTypeCommunity)
                {
                    WriteFieldName("mod ID");
                    WriteFieldValue(details.m_nPublishedFileId.ToString());

                    WriteFieldName("Title");
                    WriteFieldValue(details.m_rgchTitle);

                    WriteFieldName("Description");
                    WriteFieldValue(details.m_rgchDescription);

                    WriteFieldName("Tags");
                    WriteFieldValue(details.m_rgchTags);

                    Console.WriteLine();
                }

            }

            finished = true;
        }

        private static void WriteFieldName(string field)
        {
            var foreground = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write(field + " :\n");
            Console.ForegroundColor = foreground;
        }

        private static void WriteFieldValue(string val)
        {
            var foreground = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write(val + '\n');
            Console.ForegroundColor = foreground;
        }

        public override int Run(string[] arguments) {
            SteamAPI.Init();

            if (arguments.Length > 0)
                page = uint.Parse(arguments[0]);

            var ugcQueryhandle = SteamUGC.CreateQueryUserUGCRequest(
                SteamUser.GetSteamID().GetAccountID(),
                EUserUGCList.k_EUserUGCList_Published,
                EUGCMatchingUGCType.k_EUGCMatchingUGCType_Items,
                EUserUGCListSortOrder.k_EUserUGCListSortOrder_CreationOrderDesc,
                new AppId_t(245850), new AppId_t(322330),
                page
                );

            SteamUGC.SetReturnLongDescription(ugcQueryhandle, true);

            var handle = SteamUGC.SendQueryUGCRequest(ugcQueryhandle);

            var callResult = new CallResult<SteamUGCQueryCompleted_t>(OnGetListUGC);
            callResult.Set(handle);

            //[Akarinnnnn(Fa)]
            //This is very important to setup Steamworks API. DO NOT remove it.
            //I don't know why, but it works.
            System.Threading.Thread.Sleep(1500);

            new System.Threading.Thread(()=>
            SteamAPI.RunCallbacks())
                .Start();

            do
                System.Threading.Thread.Yield();
            while (!finished);

            SteamUGC.ReleaseQueryUGCRequest(ugcQueryhandle);
            SteamAPI.Shutdown();

            return exitcode;
        }
    }
}
