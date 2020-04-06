using DSTEd.Publisher.SteamWorkshop;
using System;
using System.Collections.Generic;
using System.Text;
using Steamworks;
namespace DSTEd.Publisher.DSTEdActions
{
    class List : ActionBase
    {
        private QueryPublishedModResult QueryResult;
        private CallResult<SteamUGCQueryCompleted_t> QueryCallResult;
        public override Type DataType => typeof(QueryPublishData);
        public override string Name => "List";

        private void OnQueryResultReceived(SteamUGCQueryCompleted_t result, bool ioFail)
        {
            QueryResult = new QueryPublishedModResult()
            {
                ResultCount = result.m_unNumResultsReturned,
                TotalMatchingResultCount = result.m_unTotalMatchingResults
            };

            if (ioFail)
            {
                ExitCode = Steam.ExitCodes.SteamIOError;
                QueryResult.IsIOFailure = true;
                QueryResult.FailureReason = result.m_eResult;
                AllAsyncOperationsFinished = true;
                return;
            }
            if (result.m_eResult != EResult.k_EResultOK)
            {
                QueryResult.FailureReason = result.m_eResult;
                AllAsyncOperationsFinished = true;
                return;
            }

            QueryResult.ModData = new PublishedModData[result.m_unNumResultsReturned];
            for (uint i = 0; i < result.m_unNumResultsReturned; i++)
            {
                SteamUGC.GetQueryUGCResult(result.m_handle, i, out SteamUGCDetails_t detail);
                QueryResult.ModData[i] = new PublishedModData()
                {
                    Description = detail.m_rgchDescription,
                    FileSize = detail.m_nFileSize,
                    PreviewFileSize = detail.m_nPreviewFileSize,
                    HFile = detail.m_hFile,
                    HPreviewFile = detail.m_hPreviewFile,
                    ID = detail.m_nPublishedFileId,
                    IsBanned = detail.m_bBanned,
                    IsTagsTruncated = detail.m_bTagsTruncated,
                    NumChildren = detail.m_unNumChildren,
                    Owner = new CSteamID(detail.m_ulSteamIDOwner),
                    Score = detail.m_flScore,
                    Tags = detail.m_rgchTags,
                    TimeAddedToUserList = detail.m_rtimeAddedToUserList,
                    TimeCreated = detail.m_rtimeCreated,
                    TimeUpdated = detail.m_rtimeUpdated,
                    Title = detail.m_rgchTitle,
                    URL = new Uri(detail.m_rgchURL, UriKind.Absolute),
                    Visibility = detail.m_eVisibility,
                    VotesDown = detail.m_unVotesDown,
                    VotesUp = detail.m_unVotesUp
                };
            }

            ExitCode = Steam.ExitCodes.NoError;
            AllAsyncOperationsFinished = true;
        }


        public override Steam.ExitCodes Do(object _DATA_)
        {
            QueryPublishData data;
            try { data = (QueryPublishData)_DATA_; }
            catch (InvalidCastException)
            {
                Console.WriteLine("Invalid data.");
                return Steam.ExitCodes.InvalidArgument;
            }

            if (!Steam.Start(Steam.APP_ID))
                return Steam.ExitCodes.InitSteamFailed;

            UGCQueryHandle_t handle;
            if (data.UserToQuery != 0)
            {
                handle = SteamUGC.CreateQueryUserUGCRequest(
                    new AccountID_t(data.UserToQuery),
                    EUserUGCList.k_EUserUGCList_Published,
                    EUGCMatchingUGCType.k_EUGCMatchingUGCType_Items_ReadyToUse, //normal mod, not needet to be voted.
                    EUserUGCListSortOrder.k_EUserUGCListSortOrder_CreationOrderAsc,
                    Steam.APP_ID, Steam.APP_GAME,
                    data.Page
                    );
            }
            else
            {
                handle = SteamUGC.CreateQueryAllUGCRequest(
                    EUGCQuery.k_EUGCQuery_RankedByPublicationDate,
                    EUGCMatchingUGCType.k_EUGCMatchingUGCType_Items_ReadyToUse,
                    Steam.APP_ID, Steam.APP_GAME,
                    data.Page
                    );
                SteamUGC.SetSearchText(handle, data.SearchText);
            }

            SteamUGC.SetReturnAdditionalPreviews(handle, true);
            SteamUGC.SetReturnKeyValueTags(handle,true);
            SteamUGC.SetReturnLongDescription(handle, true);
            SteamUGC.SetLanguage(handle, data.Language ?? "english");
            SteamUGC.SetAllowCachedResponse(handle, 0);

            SteamAPICall_t apiCallHandle = SteamUGC.SendQueryUGCRequest(handle);
            QueryCallResult = new CallResult<SteamUGCQueryCompleted_t>(OnQueryResultReceived);
            QueryCallResult.Set(apiCallHandle);

            Steam.Run();

            do
            {
                System.Threading.Thread.Sleep(100);
            } while (!AllAsyncOperationsFinished);

            ResultObject = QueryResult;

            return ExitCode;
        }
    }
}
