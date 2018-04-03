
using Steamworks;


namespace Float
{
	public class SteamManager : IManager
	{
		public bool Initialized { get; private set; }

		private SteamAPIWarningMessageHook_t m_SteamAPIWarningMessageHook;
		private CallResult<RemoteStorageSubscribePublishedFileResult_t> OnRemoteStorageSubscribePublishedFileResultCallResult;
		private CallResult<RemoteStorageUnsubscribePublishedFileResult_t> OnRemoteStorageUnsubscribePublishedFileResultCallResult;
		protected Callback<ItemInstalled_t> m_ItemInstalled;
		protected Callback<DownloadItemResult_t> m_DownloadItemResult;

		public static string WorkshopInstallPath { get; private set; }

		public delegate void ItemOperationCallback(PublishedFileId_t publishedFileId);

		public ItemOperationCallback ItemInstalledCallback;

		public ItemOperationCallback ItemDownloadedCallback;


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

			Initialized = SteamAPI.Init();
			if (!Initialized)
			{
				Log.Error("[Steamworks.NET] SteamAPI_Init() failed. Please make sure your steam client is running.");
				return;
			}

			// You must launch with "-debug_steamapi" in the launch args to recieve warnings.
			m_SteamAPIWarningMessageHook = new SteamAPIWarningMessageHook_t(SteamAPIDebugTextHook);
			SteamClient.SetWarningMessageHook(m_SteamAPIWarningMessageHook);

			OnRemoteStorageSubscribePublishedFileResultCallResult = CallResult<RemoteStorageSubscribePublishedFileResult_t>.Create(OnRemoteStorageSubscribePublishedFileResult);
			OnRemoteStorageUnsubscribePublishedFileResultCallResult = CallResult<RemoteStorageUnsubscribePublishedFileResult_t>.Create(OnRemoteStorageUnsubscribePublishedFileResult);

			m_ItemInstalled = Callback<ItemInstalled_t>.Create(OnItemInstalled);
			m_DownloadItemResult = Callback<DownloadItemResult_t>.Create(OnDownloadItemResult);
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
			{
				if (FindWorkshopInstallPath())
					FloatApp.MsgSystem.Push(AppConst.MSG_WORKSHOP_PATH_FOUND);
			}
		}

		private void SteamAPIDebugTextHook(int nSeverity, System.Text.StringBuilder pchDebugText)
		{
			Log.Warning(pchDebugText.ToString());
		}

		void OnRemoteStorageSubscribePublishedFileResult(RemoteStorageSubscribePublishedFileResult_t pCallback, bool bIOFailure)
		{
			Log.Info("[" + RemoteStorageSubscribePublishedFileResult_t.k_iCallback + " - RemoteStorageSubscribePublishedFileResult] - " + pCallback.m_eResult + " -- " + pCallback.m_nPublishedFileId);
			//m_PublishedFileId = pCallback.m_nPublishedFileId;
		}

		void OnRemoteStorageUnsubscribePublishedFileResult(RemoteStorageUnsubscribePublishedFileResult_t pCallback, bool bIOFailure)
		{
			Log.Info("[" + RemoteStorageUnsubscribePublishedFileResult_t.k_iCallback + " - RemoteStorageUnsubscribePublishedFileResult] - " + pCallback.m_eResult + " -- " + pCallback.m_nPublishedFileId);
			//m_PublishedFileId = pCallback.m_nPublishedFileId;
		}

		void OnItemInstalled(ItemInstalled_t pCallback)
		{
			if (SteamUtils.GetAppID() != pCallback.m_unAppID)
				return;

			Log.Info("[" + ItemInstalled_t.k_iCallback + " - ItemInstalled] - " + pCallback.m_unAppID + " -- " + pCallback.m_nPublishedFileId);

			ItemInstalledCallback?.Invoke(pCallback.m_nPublishedFileId);

			FloatApp.MsgSystem.Push(AppConst.MSG_ITEM_INSTALLED, pCallback.m_nPublishedFileId);
		}

		void OnDownloadItemResult(DownloadItemResult_t pCallback)
		{
			if (SteamUtils.GetAppID() != pCallback.m_unAppID)
				return;

			Log.Info("[" + DownloadItemResult_t.k_iCallback + " - DownloadItemResult] - " + pCallback.m_unAppID + " -- " + pCallback.m_nPublishedFileId + " -- " + pCallback.m_eResult);

			ItemDownloadedCallback?.Invoke(pCallback.m_nPublishedFileId);

			FloatApp.MsgSystem.Push(AppConst.MSG_ITEM_DOWNLOADED, pCallback.m_nPublishedFileId);
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

		public bool FindWorkshopInstallPath()
		{
			if (!string.IsNullOrEmpty(WorkshopInstallPath))
				return true;

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
						return true;
					}
				}
			}
			Log.Error("SteamManager.GetWorkshopInstallPath: invalid.");
			return false;
		}

	}

}
