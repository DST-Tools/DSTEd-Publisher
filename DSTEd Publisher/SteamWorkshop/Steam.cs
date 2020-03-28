using System;
using System.Collections.Generic;
using System.IO;
using Steamworks;

namespace DSTEd.Publisher.SteamWorkshop {
    public class Steam {
        //public static event Action failHandlers; //Use this? This can't be invoked outside.
        private static Action[] handles; // TODO handle Observing!
        internal static AppId_t APP_ID       = new AppId_t(245850);
        internal static AppId_t APP_GAME     = new AppId_t(322330);
        private static bool TimeToStop = false;
        public enum ExitCodes : int {
            NoError                        = 0,
            ErrorCodeBase              = unchecked((int) 0xA7FF0000),
            InitSteamFailed             = ErrorCodeBase + 1,
            SteamIOError                = ErrorCodeBase + 2,
            QueryWorkshopFail       = ErrorCodeBase + 3,
            ArgumentsMissing         = ErrorCodeBase + 4,
            CreateWorkshopFileFail  = ErrorCodeBase + 5,
            TimedOut                      = ErrorCodeBase + 6,
            SubmitWorkshopFail      = ErrorCodeBase + 7,
            InvalidArgument            = ErrorCodeBase + 8, 
            DownLoadFail                = ErrorCodeBase + 9,
            SetNewContentFail        = ErrorCodeBase + 10,
            UploadNewContentFail  = ErrorCodeBase + 11,
            SizeTooLarge                 = ErrorCodeBase + 12,
        }

        public static bool Start(AppId_t appId) {
            try
            {
                using var appid_txt = new StreamWriter(new FileStream(@".\steam_appid.txt", FileMode.Truncate));
                appid_txt.WriteLine(appId.m_AppId);// to avoid boxing
                appid_txt.Flush();
            }
            catch (FileNotFoundException)
            {
                using var appid_txt = new StreamWriter(new FileStream(@".\steam_appid.txt", FileMode.CreateNew));
                appid_txt.WriteLine(appId.m_AppId);
                appid_txt.Flush();
                throw;
            }

            bool result = SteamAPI.Init();

            if(result) {
                // ToDo start an Observer, that check if a new handle is set
            }

            return result;
        }

        public static void Stop() {
            TimeToStop = true;
            SteamAPI.Shutdown();
        }

        public static void Run() {
            System.Threading.Thread.Sleep(1500);
            new System.Threading.Thread(delegate() {
                while (!TimeToStop)
                {
                    System.Threading.Thread.Sleep(100);
                    SteamAPI.RunCallbacks();
                }}).Start();
        }

        public static void GetWorkShopItems(uint page = 1, Action<ExitCodes, List<WorkshopItem>, uint, uint> Callback = null) {
            var ugcQueryhandle = SteamUGC.CreateQueryUserUGCRequest(SteamUser.GetSteamID().GetAccountID(), EUserUGCList.k_EUserUGCList_Published, EUGCMatchingUGCType.k_EUGCMatchingUGCType_Items, EUserUGCListSortOrder.k_EUserUGCListSortOrder_CreationOrderDesc, APP_ID, APP_GAME, page);
            
            SteamUGC.SetReturnLongDescription(ugcQueryhandle, true);

            var handle      = SteamUGC.SendQueryUGCRequest(ugcQueryhandle);
            var callResult  = new CallResult<SteamUGCQueryCompleted_t>(delegate(SteamUGCQueryCompleted_t queryResult, bool failure) {
                
                if(failure) {
                    Callback?.Invoke(ExitCodes.SteamIOError, null, 0, 0);
                    return;
                }

                if(queryResult.m_eResult != EResult.k_EResultOK) {
                    Callback?.Invoke(ExitCodes.QueryWorkshopFail, null, 0, 0);
                    return;
                }

                uint total  = queryResult.m_unTotalMatchingResults;
                uint count  = queryResult.m_unNumResultsReturned;
                var items   = new List<WorkshopItem>((int)count);//never overflow, max 50

                for(uint i = 0; i < count; i++) {
                    SteamUGC.GetQueryUGCResult(queryResult.m_handle, i, out SteamUGCDetails_t details);

                    if(details.m_nConsumerAppID == APP_GAME && details.m_eFileType == EWorkshopFileType.k_EWorkshopFileTypeCommunity) {
                        items.Add(new WorkshopItem {
                            ID          = Int32.Parse(details.m_nPublishedFileId.ToString()),
                            Title       = details.m_rgchTitle,
                            Description = details.m_rgchDescription,
                            Tags        = details.m_rgchTags // @ToDo Split & Trim Tags!
                        });
                    }
                }

                Callback?.Invoke(ExitCodes.NoError, items, count, total);

                SteamUGC.ReleaseQueryUGCRequest(ugcQueryhandle);
            });

            Console.WriteLine("callResult");
            callResult.Set(handle);

            // @ToDo Following lines by handle Observer!
            Run();
            do {
                System.Threading.Thread.Yield();
            }  while(true);
            //  SteamUGC.ReleaseQueryUGCRequest(ugcQueryhandle);
        }
    }
}
