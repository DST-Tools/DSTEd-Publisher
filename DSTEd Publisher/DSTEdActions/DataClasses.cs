using System;
using System.Collections.Generic;

using Steamworks;
namespace DSTEd.Publisher.DSTEdActions
{

    #region OutputData
    /// <summary>
    /// This will be serialized to send it back to DSTEd main program
    /// </summary>
    [Serializable]
    internal class PublishedModData
    {
        public PublishedFileId_t ID;
        public uint NumChildren;
        public float Score;
        public uint VotesDown;
        public uint VotesUp;
        public int PreviewFileSize;
        public int FileSize;
        public UGCHandle_t HPreviewFile;
        public UGCHandle_t HFile;
        public bool IsTagsTruncated;
        public ERemoteStoragePublishedFileVisibility Visibility;
        public bool IsBanned;
        public CSteamID Owner;
        public uint TimeCreated;
        public uint TimeUpdated;
        public uint TimeAddedToUserList;

        public string Title;
        public string Description;
        public string Tags;
        public string URL;

    }
    [Serializable]
    internal class QueryPublishedModResult
    {
        public uint TotalMatchingResultCount;
        public uint ResultCount;
        public PublishedModData[] ModData;
        public EResult FailureReason = EResult.k_EResultOK;
        public bool IsIOFailure = false;
    }

    #endregion

    #region InputData
#pragma warning disable CS0649
    [Serializable]
    internal class QueryPublishData
    {
        public uint Page;
        public uint UserToQuery;// set it to 0 if query whole workshop
        public string SearchText;// only works while querying whole workshop
        public string Language;// can be null, english result will be returned if null. DO NOT SET string.Empty
    }

    [Serializable]
    internal class UploadData
    {
        public readonly string Title;
        public readonly string Description;
        public readonly string PreviewImage;
        public readonly string ContentDirectory;
        public readonly string ItemLanguage;

        public readonly List<string> tags;

    }

    #endregion
}