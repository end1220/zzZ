
using Float;
using Steamworks;

namespace Float
{
	public class SteamManager : IManager
	{
		public bool Initialized { get; private set; }

		private SteamAPIWarningMessageHook_t m_SteamAPIWarningMessageHook;

		public static string WorkshopInstallPath { get; private set; }

		public override void Init()
		{
			if (Initialized)
				throw new System.Exception("Tried to Initialize the SteamAPI twice in one session!");

			if (!Packsize.Test())
				Log.Error("[Steamworks.NET] Packsize Test returned false, the wrong version of Steamworks.NET is being run in this platform.");

			if (!DllCheck.Test())
				Log.Error("[Steamworks.NET] DllCheck Test returned false, One or more of the Steamworks binaries seems to be the wrong version.");

			try
			{
				// If Steam is not running or the game wasn't started through Steam, SteamAPI_RestartAppIfNecessary starts the
				// Steam client and also launches this game again if the User owns it. This can act as a rudimentary form of DRM.

				// Once you get a Steam AppID assigned by Valve, you need to replace AppId_t.Invalid with it and
				// remove steam_appid.txt from the game depot. eg: "(AppId_t)480" or "new AppId_t(480)".
				// See the Valve documentation for more information: https://partner.steamgames.com/documentation/drm#FAQ
				if (SteamAPI.RestartAppIfNecessary(AppId_t.Invalid))
				{
					App.Instance.Quit();
					return;
				}
			}
			catch (System.DllNotFoundException e)
			{
				Log.Error("[Steamworks.NET] Could not load [lib]steam_api.dll/so/dylib. It's likely not in the correct location. Refer to the README for more details.\n" + e);
				App.Instance.Quit();
				return;
			}

			// Initialize the SteamAPI, if Init() returns false this can happen for many reasons.
			// Some examples include:
			// Steam Client is not running.
			// Launching from outside of steam without a steam_appid.txt file in place.
			// Running under a different OS User or Access level (for example running "as administrator")
			// Ensure that you own a license for the AppId on your active Steam account
			// If your AppId is not completely set up. Either in Release State: Unavailable, or if it's missing default packages.
			// Valve's documentation for this is located here:
			// https://partner.steamgames.com/documentation/getting_started
			// https://partner.steamgames.com/documentation/example // Under: Common Build Problems
			// https://partner.steamgames.com/documentation/bootstrap_stats // At the very bottom

			Initialized = SteamAPI.Init();
			if (!Initialized)
			{
				//Log.Error("[Steamworks.NET] SteamAPI_Init() failed. Refer to Valve's documentation or the comment above this line for more information.");
				Log.Error("[Steamworks.NET] SteamAPI_Init() failed. Please make sure your steam client is running.");
				return;
			}

			if (m_SteamAPIWarningMessageHook == null)
			{
				// You must launch with "-debug_steamapi" in the launch args to recieve warnings.
				m_SteamAPIWarningMessageHook = new SteamAPIWarningMessageHook_t(SteamAPIDebugTextHook);
				SteamClient.SetWarningMessageHook(m_SteamAPIWarningMessageHook);
			}
		}


		public override void Destroy()
		{
			if (!Initialized)
				return;

			Initialized = false;
			SteamAPI.Shutdown();
		}

		public override void Tick()
		{
			if (!Initialized)
				return;

			SteamAPI.RunCallbacks();

			if (string.IsNullOrEmpty(WorkshopInstallPath))
				FindWorkshopInstallPath();
		}

		private static void SteamAPIDebugTextHook(int nSeverity, System.Text.StringBuilder pchDebugText)
		{
			Log.Warning(pchDebugText.ToString());
		}

		PublishedFileId_t[] publishedItemIDs;
		private void UpdateDownload()
		{
			uint totalCount = SteamUGC.GetNumSubscribedItems();
			if (publishedItemIDs == null || publishedItemIDs.Length != totalCount)
				publishedItemIDs = new PublishedFileId_t[totalCount];
			/*uint ret = */SteamUGC.GetSubscribedItems(publishedItemIDs, (uint)publishedItemIDs.Length);
	
			for (int i = 0; i < publishedItemIDs.Length; ++i)
			{
				var publishId = publishedItemIDs[i];
				var state = /*(EItemState)*/SteamUGC.GetItemState(publishId);
				if ((state & (uint)EItemState.k_EItemStateSubscribed) == 0)
				{
					Log.Error("Not subscribed");
					continue;
				}

				if ((state & (uint)EItemState.k_EItemStateInstalled) == 0)
				{
					SteamUGC.DownloadItem(publishId, true);
				}
				else
				{
					if ((state & (uint)EItemState.k_EItemStateNeedsUpdate) > 0)
					{
						SteamUGC.DownloadItem(publishId, true);
					}
					else
						continue;
				}

				if ((state & (uint)EItemState.k_EItemStateDownloading) > 0)
				{
					ulong BytesDownloaded;
					ulong BytesTotal;
					bool ret = SteamUGC.GetItemDownloadInfo(publishId, out BytesDownloaded, out BytesTotal);
				}
				else if ((state & (uint)EItemState.k_EItemStateDownloadPending) > 0)
				{

				}
				
			}
		}

		public string FindWorkshopInstallPath()
		{
			if (!string.IsNullOrEmpty(WorkshopInstallPath))
				return WorkshopInstallPath;

			uint totalCount = SteamUGC.GetNumSubscribedItems();
			if (publishedItemIDs == null || publishedItemIDs.Length != totalCount)
				publishedItemIDs = new PublishedFileId_t[totalCount];
			SteamUGC.GetSubscribedItems(publishedItemIDs, (uint)publishedItemIDs.Length);
			for (int i = 0; i < publishedItemIDs.Length; ++i)
			{
				var publishId = publishedItemIDs[i];
				var state = /*(EItemState)*/SteamUGC.GetItemState(publishId);
				if ((state & (uint)EItemState.k_EItemStateInstalled) > 0)
				{
					ulong SizeOnDisk;
					string folder;
					uint punTimeStamp;
					if (SteamUGC.GetItemInstallInfo(publishId, out SizeOnDisk, out folder, 1024, out punTimeStamp))
					{
						folder = folder.Replace("\\", "/");
						WorkshopInstallPath = folder.Substring(0, folder.LastIndexOf("/"));
						return WorkshopInstallPath;
					}
				}
			}
			Log.Error("SteamManager.GetWorkshopInstallPath: invalid.");
			return "";
		}

	}

}
