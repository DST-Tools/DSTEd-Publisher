using System;

namespace DSTEd.Publisher.Actions
{
	/// <summary>
	/// Defined all exit error codes
	/// </summary>
	enum ExitCodes : int
	{
		/// <summary>
		/// everything ok
		/// </summary>
		NoError = 0,
		ErrorCodeBase = unchecked((int)0xA7FF0000),
		SteamIOError = ErrorCodeBase+1,
		QueryWorkshopFail = ErrorCodeBase+2,
		ArgumentsMissing  = ErrorCodeBase + 3,
		CreateWorkshopFileFail = ErrorCodeBase + 4,
		TimedOut = ErrorCodeBase + 5,
		SubmitWorkshopFail = ErrorCodeBase + 5,
	}
}