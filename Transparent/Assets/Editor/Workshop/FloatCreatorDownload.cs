using UnityEngine;
using UnityEditor;
using Steamworks;


namespace Float
{
	public partial class FloatCreatorWindow : EditorWindow
	{
		void OnDownloadGUI()
		{
			GUILayout.Label("Subscribed Items Count: " + SteamUGC.GetNumSubscribedItems());

			{
				PublishedFileId_t[] PublishedFileID = new PublishedFileId_t[1];
				uint ret = SteamUGC.GetSubscribedItems(PublishedFileID, (uint)PublishedFileID.Length);
				m_PublishedFileId = PublishedFileID[0];
				//Debug.Log("GetSubscribedItems(" + PublishedFileID + ", " + (uint)PublishedFileID.Length + ") : " + ret);
				//Debug.Log(m_PublishedFileId);
				GUILayout.Label("ItemState : " + (EItemState)SteamUGC.GetItemState(m_PublishedFileId));

				{
					ulong SizeOnDisk;
					string Folder;
					uint punTimeStamp;
					bool rets = SteamUGC.GetItemInstallInfo(m_PublishedFileId, out SizeOnDisk, out Folder, 1024, out punTimeStamp);
					GUILayout.Label("ItemInstallInfo : " + rets + " -- " + SizeOnDisk + " -- " + punTimeStamp);
					GUILayout.Label("    Folder : " + Folder);
				}
			}

			if (GUILayout.Button("download", GUILayout.Width(100)))
			{
				bool ret = SteamUGC.DownloadItem(m_PublishedFileId, true);
				Debug.Log("SteamUGC.DownloadItem(" + m_PublishedFileId + ", " + true + ") : " + ret);
			}

			{
				ulong BytesDownloaded;
				ulong BytesTotal;
				bool ret = SteamUGC.GetItemDownloadInfo(m_PublishedFileId, out BytesDownloaded, out BytesTotal);
				Debug.Log("GetItemDownloadInfo(" + m_PublishedFileId + ", " + "out BytesDownloaded" + ", " + "out BytesTotal" + ") : " + ret + " -- " + BytesDownloaded + " -- " + BytesTotal);
			}
			/*if (GUILayout.Button("GetSubscribedItems(PublishedFileID, (uint)PublishedFileID.Length)"))
			{
				PublishedFileId_t[] PublishedFileID = new PublishedFileId_t[1];
				uint ret = SteamUGC.GetSubscribedItems(PublishedFileID, (uint)PublishedFileID.Length);
				m_PublishedFileId = PublishedFileID[0];
				Debug.Log("SteamUGC.GetSubscribedItems(" + PublishedFileID + ", " + (uint)PublishedFileID.Length + ") : " + ret);
				Debug.Log(m_PublishedFileId);
			}

			GUILayout.Label("GetItemState(PublishedFileID) : " + (EItemState)SteamUGC.GetItemState(m_PublishedFileId));

			{
				ulong SizeOnDisk;
				string Folder;
				uint punTimeStamp;
				bool ret = SteamUGC.GetItemInstallInfo(m_PublishedFileId, out SizeOnDisk, out Folder, 1024, out punTimeStamp);
				GUILayout.Label("GetItemInstallInfo(m_PublishedFileId, out SizeOnDisk, out Folder, 1024, out punTimeStamp) : " + ret + " -- " + SizeOnDisk + " -- " + Folder + " -- " + punTimeStamp);
			}

			if (GUILayout.Button("GetItemDownloadInfo(m_PublishedFileId, out BytesDownloaded, out BytesTotal)"))
			{
				ulong BytesDownloaded;
				ulong BytesTotal;
				bool ret = SteamUGC.GetItemDownloadInfo(m_PublishedFileId, out BytesDownloaded, out BytesTotal);
				Debug.Log("SteamUGC.GetItemDownloadInfo(" + m_PublishedFileId + ", " + "out BytesDownloaded" + ", " + "out BytesTotal" + ") : " + ret + " -- " + BytesDownloaded + " -- " + BytesTotal);
			}

			if (GUILayout.Button("DownloadItem(m_PublishedFileId, true)"))
			{
				bool ret = SteamUGC.DownloadItem(m_PublishedFileId, true);
				Debug.Log("SteamUGC.DownloadItem(" + m_PublishedFileId + ", " + true + ") : " + ret);
			}

			if (GUILayout.Button("SuspendDownloads(true)"))
			{
				SteamUGC.SuspendDownloads(true);
				Debug.Log("SteamUGC.SuspendDownloads(" + true + ")");
			}*/
		}

		protected Callback<ItemInstalled_t> m_ItemInstalled;
		protected Callback<DownloadItemResult_t> m_DownloadItemResult;

		void OnItemInstalled(ItemInstalled_t pCallback)
		{
			Debug.Log("[" + ItemInstalled_t.k_iCallback + " - ItemInstalled] - " + pCallback.m_unAppID + " -- " + pCallback.m_nPublishedFileId);
		}

		void OnDownloadItemResult(DownloadItemResult_t pCallback)
		{
			Debug.Log("[" + DownloadItemResult_t.k_iCallback + " - DownloadItemResult] - " + pCallback.m_unAppID + " -- " + pCallback.m_nPublishedFileId + " -- " + pCallback.m_eResult);
		}
	}
}