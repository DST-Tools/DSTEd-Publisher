using System;
using System.IO;
using DSTEd.Publisher.SteamWorkshop;
using Steamworks;
using static DSTEd.Publisher.SteamWorkshop.Steam;

namespace DSTEd.Publisher.Actions {
    class Upload : ActionClass {
        private bool UploadFinished = false;
        
        private string ContentDirectory;
        private string PreviewFile;//necessary
        private string Title;

        private int RetryTimes = 0;
        private int ExitCode;
        private UGCUpdateHandle_t updateHandle;
        public Upload() {
            this.Name           = "upload";
            this.Description    = "Uploads a new Steam-WorkShop item.";
            this.Arguments      = "<Content Directory | Preview File | Title>";
        }

        private string GetCreateErrorText(EResult error)
        {
            return error switch
            {
                EResult.k_EResultInsufficientPrivilege => 
                "You are currently restricted from uploading content due to a hub ban, " +
                 "account lock, or community ban. You may need to contact Steam Support.",

                EResult.k_EResultBanned => 
                "You don't have permission to upload content " +
                "to this hub because they have an active VAC or Game ban.",

                EResult.k_EResultTimeout =>
                "Retried 3 times, but still timed out while publishing.",

                EResult.k_EResultNotLoggedOn => 
                "You haven't log on yet.",

                EResult.k_EResultServiceUnavailable => 
                "The workshop server hosting the content is having issues. Retry later.",

                EResult.k_EResultInvalidParam => 
                "One of the submission fields contains something not being accepted by that field.",

                EResult.k_EResultAccessDenied => 
                "There was a problem trying to save the title and description. Access was denied.",

                EResult.k_EResultLimitExceeded => 
                "You have exceeded their Steam Cloud quota. Remove some items and try again.",

                EResult.k_EResultFileNotFound => 
                "The uploaded file could not be found.",

                EResult.k_EResultDuplicateRequest => 
                "The file was already successfully uploaded.",

                EResult.k_EResultDuplicateName => 
                "You already have a Steam Workshop item with that name.",

                EResult.k_EResultServiceReadOnly => 
                "Due to a recent password or email change, " +
                "you are not allowed to upload new content. " +
                "Usually this restriction will expire in 5 days, " +
                "but can last up to 30 days if your account has been inactive recently.",
                _ => error.ToString(),
            };
        }
        private string GetSubmitErrorText(EResult error)
        {
            return error switch
            {
                EResult.k_EResultFail => "Failed",

                EResult.k_EResultInvalidParam =>
                "The preview file is smaller than 16 bytes.",

                EResult.k_EResultAccessDenied =>
                "Youd don't own a license for Don't Strave Together.",

                EResult.k_EResultFileNotFound =>
                "Failed to get the workshop info for the item or failed to read the preview file, " +
                "or your mod folder provided is not valid.",

                EResult.k_EResultLimitExceeded =>
                "The preview image is too large, it must be less than 1MB; " +
                "or there is not enough space available on your Steam Cloud",

                _ => error.ToString(),
            };
        }
        private void OnSubmitFinished(SubmitItemUpdateResult_t result, bool ioFail)
        {
            if (ioFail)
            {
                Console.WriteLine("Failed to communicate with workshop.");
                ExitCode = (int)ExitCodes.SteamIOError;
                return;
            }

            if (result.m_eResult != EResult.k_EResultOK)
            {
                Console.WriteLine("Some error happened while submitting.\n" +
                    GetSubmitErrorText(result.m_eResult)
                    );
                ExitCode = (int)ExitCodes.SubmitWorkshopFail;
                return;
            }

            UploadFinished = true;
        }

        private void OnUploadContentFinished(RemoteStorageUpdatePublishedFileResult_t result, bool ioFail)
        {
            if (ioFail)
            {
                Console.WriteLine("Failed to communicate with workshop.");
                ExitCode = (int)ExitCodes.SteamIOError;
                return;
            }

            if(result.m_eResult!=EResult.k_EResultOK)
            {
                Console.WriteLine($"Upload content failed, {GetSubmitErrorText(result.m_eResult)}");
                ExitCode = (int)ExitCodes.UploadNewContentFail;
                return;
            }
            Console.WriteLine("Successfully uploaded your mod.");
            if(result.m_bUserNeedsToAcceptWorkshopLegalAgreement)
                Console.WriteLine("Finally you just have to agree Workshop Legal Agreement");
        }

        private void FailDelete(PublishedFileId_t id)
        {
            var result = new CallResult<DeleteItemResult_t>(delegate (DeleteItemResult_t result, bool ioFail)
            {
                UploadFinished = true;
            });
            result.Set(SteamUGC.DeleteItem(id));
        }

        private void OnCreateItemFinished(CreateItemResult_t result, bool ioFail)
        {
            if (ioFail)
            {
                Console.WriteLine("Failed to communicate with workshop.");
                ExitCode = (int)ExitCodes.SteamIOError;
            }

            if (result.m_eResult != EResult.k_EResultOK)
            {
                //retry 3 times.
                if (result.m_eResult == EResult.k_EResultTimeout && RetryTimes <= 3)
                {
                    RetryTimes++;
                    var handle = SteamUGC.CreateItem(APP_GAME, EWorkshopFileType.k_EWorkshopFileTypeCommunity);
                    var createResult = new CallResult<CreateItemResult_t>(OnCreateItemFinished);
                    createResult.Set(handle);
                    return;
                }
                else
                {
                    Console.WriteLine(GetCreateErrorText(EResult.k_EResultTimeout));
                    ExitCode = (int)ExitCodes.TimedOut;
                    UploadFinished = true;
                }

                Console.WriteLine("Some error happened while creating your mod.\n" +
                    GetCreateErrorText(result.m_eResult)
                    );
                ExitCode = (int)ExitCodes.CreateWorkshopFileFail;
            }

            RetryTimes = 0;

            updateHandle = SteamUGC.StartItemUpdate(APP_GAME, result.m_nPublishedFileId);

            // steam reports "No workshop depots found."
            //if (!SteamUGC.SetItemContent(updateHandle, Path.GetFullPath(ContentDirectory)))
            //    Console.WriteLine("Failed to set files to update");
            if (PreviewFile != string.Empty)
            {
                if (!SteamUGC.SetItemPreview(updateHandle, Path.GetFullPath(PreviewFile)))
                    Console.WriteLine(
                        "Failed to set preview file\n" +
                        "You can to upload your preview by mod webpage later."
                        );
            }
            else
            {
                Console.WriteLine("You can to upload your preview by mod webpage later.");
            }

            if (!SteamUGC.SetItemTitle(updateHandle, Title))
                Console.WriteLine("Failed to set mod title");

            if (!SteamUGC.SetItemVisibility(updateHandle, ERemoteStoragePublishedFileVisibility.k_ERemoteStoragePublishedFileVisibilityPublic))
                Console.WriteLine("Failed to Set Item visibility");

            {
                var contentUpdateHandle = SteamRemoteStorage.CreatePublishedFileUpdateRequest(result.m_nPublishedFileId);
                if(!SteamRemoteStorage.UpdatePublishedFileFile(contentUpdateHandle, "mod_publish_data_file.zip"))
                {
                    Console.WriteLine("Update mod content failed, upload aborted.");
                    
                    //these may release some steam resources.
                    SteamUGC.SubmitItemUpdate(updateHandle,"FAIL");
                    SteamRemoteStorage.CommitPublishedFileUpdate(contentUpdateHandle);
                    
                    ExitCode = (int)ExitCodes.SetNewContentFail;
                    FailDelete(result.m_nPublishedFileId);
                    return;
                }
                var apicallHandle = SteamRemoteStorage.CommitPublishedFileUpdate(contentUpdateHandle);
                new CallResult<RemoteStoragePublishFileProgress_t>()
            }

            var submitHandle = SteamUGC.SubmitItemUpdate(updateHandle, "Initial commit");
            var submitResult = new CallResult<SubmitItemUpdateResult_t>(OnSubmitFinished);
            submitResult.Set(submitHandle);
        }
        public override int Run(string[] arguments) {
            if (arguments.Length < 3)
            {
                Console.WriteLine("Argument #1\"Content Directory\", #2\"Preview File\" and #3\"Title\" are necessary");
                return (int)ExitCodes.ArgumentsMissing;
            }

            ContentDirectory = arguments[0];
            PreviewFile = arguments[1];
            Title = arguments[2];

            if (!Start(APP_ID)) {
                Console.WriteLine("Steam is not running...");
                return (int)ExitCodes.InitSteamFailed;
            }

            var handle = SteamUGC.CreateItem(APP_GAME,
                EWorkshopFileType.k_EWorkshopFileTypeCommunity);

            new CallResult<CreateItemResult_t>(OnCreateItemFinished).Set(handle);

            Steam.Run();

            

            

            return ExitCode;
        }
    }
}
