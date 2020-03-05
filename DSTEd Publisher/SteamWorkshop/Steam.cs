using System;
using System.Collections.Generic;
using Steamworks;

namespace DSTEd.Publisher.SteamWorkshop {
    public class Steam {
        private static Action[] handles; // @ToDo handle Observing!
        private static AppId_t APP_ID       = new AppId_t(245850);
        private static AppId_t APP_GAME     = new AppId_t(322330);

        public enum ExitCodes : int {
            NoError                 = 0,
            ErrorCodeBase           = unchecked((int) 0xA7FF0000),
            SteamIOError            = ErrorCodeBase + 1,
            QueryWorkshopFail       = ErrorCodeBase + 2,
            ArgumentsMissing        = ErrorCodeBase + 3,
            CreateWorkshopFileFail  = ErrorCodeBase + 4,
            TimedOut                = ErrorCodeBase + 5,
            SubmitWorkshopFail      = ErrorCodeBase + 5,
        }

        public static bool Start() {
            bool result = SteamAPI.Init();

            if(result) {
                // @ToDo start an Observer, that check if a new handle is set
            }

            return result;
        }

        public static void Stop() {
            SteamAPI.Shutdown();
        }

        private static void Run() {
            System.Threading.Thread.Sleep(1500);
            new System.Threading.Thread(() => SteamAPI.RunCallbacks()).Start();
        }

        public static void GetWorkShopItems(uint page = 1, Action<int, List<WorkshopItem>, uint, uint> Callback = null) {
            var ugcQueryhandle = SteamUGC.CreateQueryUserUGCRequest(SteamUser.GetSteamID().GetAccountID(), EUserUGCList.k_EUserUGCList_Published, EUGCMatchingUGCType.k_EUGCMatchingUGCType_Items, EUserUGCListSortOrder.k_EUserUGCListSortOrder_CreationOrderDesc, APP_ID, APP_GAME, page);
            
            SteamUGC.SetReturnLongDescription(ugcQueryhandle, true);

            var handle      = SteamUGC.SendQueryUGCRequest(ugcQueryhandle);
            var callResult  = new CallResult<SteamUGCQueryCompleted_t>(delegate(SteamUGCQueryCompleted_t queryResult, bool failure) {
                
                if(failure) {
                    Callback?.Invoke((int) ExitCodes.SteamIOError, null, 0, 0);
                    return;
                }

                if(queryResult.m_eResult != EResult.k_EResultOK) {
                    Callback?.Invoke((int) ExitCodes.QueryWorkshopFail, null, 0, 0);
                    return;
                }

                uint total  = queryResult.m_unTotalMatchingResults;
                uint count  = queryResult.m_unNumResultsReturned;
                var items   = new List<WorkshopItem>();

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

                Callback?.Invoke((int) ExitCodes.NoError, items, count, total);
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
