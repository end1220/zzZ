using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using Newtonsoft.Json;
using Steamworks;


namespace Lite
{
	public partial class FloatCreatorWindow : EditorWindow
	{
		private void OnSubmitGUI()
		{
			GUILayout.BeginHorizontal();
			GUILayout.Space(spaceSize);
			GUILayout.Label(Language.Get(TextID.Title), FloatGUIStyle.boldLabel, GUILayout.Width(titleLen));
			itemTitle = GUILayout.TextField(itemTitle, FloatGUIStyle.textField, GUILayout.Width(textLen));
			GUILayout.EndHorizontal();
			GUILayout.Space(leftSpace);

			GUILayout.BeginHorizontal();
			GUILayout.Space(spaceSize);
			GUILayout.Label(Language.Get(TextID.Desc), FloatGUIStyle.boldLabel, GUILayout.Width(titleLen));
			itemDesc = GUILayout.TextArea(itemDesc, FloatGUIStyle.textArea, GUILayout.Width(textLen), GUILayout.Height(140));
			GUILayout.EndHorizontal();
			GUILayout.Space(leftSpace);

			GUILayout.BeginHorizontal();
			GUILayout.Space(spaceSize);
			GUILayout.Label(Language.Get(TextID.Preview), FloatGUIStyle.boldLabel, GUILayout.Width(titleLen));
			string savedPreviewPath = EditorPrefs.GetString("BMW_PreviewPath");
			previewFilePath = string.IsNullOrEmpty(savedPreviewPath) ? previewFilePath : savedPreviewPath;
			GUILayout.Box(Resources.Load(Path.GetFileNameWithoutExtension(previewFilePath)) as Texture, GUILayout.Width(128), GUILayout.Height(128));
			GUILayout.BeginVertical();
			GUILayout.Space(120);
			//previewFilePath = GUILayout.TextField(previewFilePath, GUILayout.Width(textLen));
			if (GUILayout.Button("Select", GUILayout.Width(buttonLen2)))
			{
				previewFilePath = EditorUtility.OpenFilePanel("Select Preview File", String.Empty, "jpg,png");
				if (!string.IsNullOrEmpty(previewFilePath))
					EditorPrefs.SetString("BMW_PreviewPath", previewFilePath);
				if (File.Exists(previewFilePath))
				{
					File.Copy(previewFilePath, Application.dataPath + "/Editor/Resources/" + Path.GetFileName(previewFilePath));
					AssetDatabase.Refresh();
				}
			}
			GUILayout.EndVertical();
			GUILayout.EndHorizontal();
			GUILayout.Space(leftSpace);

			// steam info begin
			GUILayout.BeginHorizontal();
			//SteamUser.GetSteamID(), SteamUtils.GetAppID()
			GUILayout.Label("SteamID:", FloatGUIStyle.boldLabel, GUILayout.Width(titleLen));
			GUILayout.Label(SteamUser.GetSteamID().m_SteamID.ToString(), FloatGUIStyle.boldLabel, GUILayout.Width(textLen));
			GUILayout.EndHorizontal();

			GUILayout.BeginHorizontal();
			GUILayout.Label("AppID:", FloatGUIStyle.boldLabel, GUILayout.Width(titleLen));
			GUILayout.Label(SteamUtils.GetAppID().ToString(), FloatGUIStyle.boldLabel, GUILayout.Width(textLen));
			GUILayout.EndHorizontal();

			GUILayout.BeginHorizontal();
			GUILayout.Space(leftSpace);
			agreeWorkshopPolicy = GUILayout.Toggle(agreeWorkshopPolicy, Language.Get(TextID.accept), GUILayout.Width(40));
			if (GUILayout.Button(Language.Get(TextID.legal), FloatGUIStyle.link, GUILayout.Width(130)))
			{
				Application.OpenURL("http://steamcommunity.com/sharedfiles/workshoplegalagreement");
			}
			GUILayout.EndHorizontal();
			GUILayout.Space(spaceSize);

			GUILayout.BeginHorizontal();
			GUILayout.Space(leftSpace);
			if (GUILayout.Button(Language.Get(TextID.createItem), GUILayout.Width(200), GUILayout.Height(buttonHeight)))
			{
				if (EditorUtility.DisplayDialog(Language.Get(TextID.workshopPolicy),
					Language.Get(TextID.ackWorkshopPolicy),
					Language.Get(TextID.ok),
					Language.Get(TextID.cancel)))
				{
					CreateItem();
				}
			}

			if (GUILayout.Button(Language.Get(TextID.submitToWorkshop), GUILayout.Width(200), GUILayout.Height(buttonHeight)))
			{
				UpdateItem();
			}
			GUILayout.EndHorizontal();
			GUILayout.Space(spaceSize);

			OnUpdateItemGUI();
		}

		// steam api

		private CallResult<CreateItemResult_t> OnCreateItemResultCallResult;
		private CallResult<SubmitItemUpdateResult_t> OnSubmitItemUpdateResultCallResult;
		private PublishedFileId_t m_PublishedFileId;
		private UGCUpdateHandle_t m_UGCUpdateHandle;

		void InitSteamAPI()
		{
			if (!SteamManager.Instance.Initialized)
				SteamManager.Instance.Init();

			OnCreateItemResultCallResult = CallResult<CreateItemResult_t>.Create(OnCreateItemResult);
			OnSubmitItemUpdateResultCallResult = CallResult<SubmitItemUpdateResult_t>.Create(OnSubmitItemUpdateResult);

			m_ItemInstalled = Callback<ItemInstalled_t>.Create(OnItemInstalled);
			m_DownloadItemResult = Callback<DownloadItemResult_t>.Create(OnDownloadItemResult);
		}

		void DestroySteamAPI()
		{
			if (SteamManager.Instance.Initialized)
				SteamManager.Instance.Destroy();
		}

		private void CreateItem()
		{
			SteamAPICall_t handle = SteamUGC.CreateItem(SteamUtils.GetAppID(), EWorkshopFileType.k_EWorkshopFileTypeCommunity);
			OnCreateItemResultCallResult.Set(handle);
			//Debug.Log("SteamUGC.CreateItem(" + SteamUtils.GetAppID() + ", " + EWorkshopFileType.k_EWorkshopFileTypeCommunity + ") : " + handle);
		}

		private void UpdateItem()
		{
			m_UGCUpdateHandle = SteamUGC.StartItemUpdate(SteamUtils.GetAppID(), m_PublishedFileId);
			//Debug.Log("SteamUGC.StartItemUpdate(" + SteamUtils.GetAppID() + ", " + m_PublishedFileId + ") : " + m_UGCUpdateHandle);

			SteamUGC.SetItemTitle(m_UGCUpdateHandle, itemTitle);
			SteamUGC.SetItemDescription(m_UGCUpdateHandle, itemDesc);
			SteamUGC.SetItemUpdateLanguage(m_UGCUpdateHandle, "english");
			SteamUGC.SetItemMetadata(m_UGCUpdateHandle, "This is the test metadata.");
			SteamUGC.SetItemVisibility(m_UGCUpdateHandle, ERemoteStoragePublishedFileVisibility.k_ERemoteStoragePublishedFileVisibilityPublic);
			SteamUGC.SetItemTags(m_UGCUpdateHandle, new string[] { "Tag One", "Tag Two", "Test Tags", "Sorry" });
			SteamUGC.SetItemContent(m_UGCUpdateHandle, "C:/Users/admin/Desktop/Content");
			//SteamUGC.SetItemPreview(m_UGCUpdateHandle, "C:/Users/admin/Desktop/DefaultPreviewImage.png");

			#region backup
			/*SteamUGC.RemoveItemKeyValueTags(m_UGCUpdateHandle, "TestKey");
			SteamUGC.AddItemKeyValueTag(m_UGCUpdateHandle, "TestKey", "TestValue");
			SteamUGC.AddItemPreviewFile(m_UGCUpdateHandle, Application.dataPath + "/PreviewImage.jpg", EItemPreviewType.k_EItemPreviewType_Image);
			SteamUGC.AddItemPreviewVideo(m_UGCUpdateHandle, "jHgZh4GV9G0");
			SteamUGC.UpdateItemPreviewFile(m_UGCUpdateHandle, 0, Application.dataPath + "/PreviewImage.jpg");
			SteamUGC.UpdateItemPreviewVideo(m_UGCUpdateHandle, 0, "jHgZh4GV9G0");
			SteamUGC.RemoveItemPreview(m_UGCUpdateHandle, 0);*/
			#endregion

			SteamAPICall_t handle = SteamUGC.SubmitItemUpdate(m_UGCUpdateHandle, "submit content");
			OnSubmitItemUpdateResultCallResult.Set(handle);
		}

		void OnUpdateItemGUI()
		{
			{
				ulong BytesProcessed;
				ulong BytesTotal;
				EItemUpdateStatus reti = SteamUGC.GetItemUpdateProgress(m_UGCUpdateHandle, out BytesProcessed, out BytesTotal);
				GUILayout.Label("GetItemUpdateProgress : ret " + reti + " -- " + BytesProcessed + " -- " + BytesTotal);
			}

			GUILayout.Label("GetNumSubscribedItems() : " + SteamUGC.GetNumSubscribedItems());

			if (GUILayout.Button("GetSubscribedItems(PublishedFileID, (uint)PublishedFileID.Length)"))
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

			if (GUILayout.Button("BInitWorkshopForGameServer((DepotId_t)481, \"C:/UGCTest\")"))
			{
				bool ret = SteamUGC.BInitWorkshopForGameServer((DepotId_t)481, "C:/UGCTest");
				Debug.Log("SteamUGC.BInitWorkshopForGameServer(" + (DepotId_t)481 + ", " + "\"C:/UGCTest\"" + ") : " + ret);
			}

			if (GUILayout.Button("SuspendDownloads(true)"))
			{
				SteamUGC.SuspendDownloads(true);
				Debug.Log("SteamUGC.SuspendDownloads(" + true + ")");
			}
		}

		void OnCreateItemResult(CreateItemResult_t pCallback, bool bIOFailure)
		{
			Debug.Log("[" + CreateItemResult_t.k_iCallback + " - CreateItemResult] - " + pCallback.m_eResult + " -- " + pCallback.m_nPublishedFileId + " -- " + pCallback.m_bUserNeedsToAcceptWorkshopLegalAgreement);

			m_PublishedFileId = pCallback.m_nPublishedFileId;
		}

		void OnSubmitItemUpdateResult(SubmitItemUpdateResult_t pCallback, bool bIOFailure)
		{
			Debug.Log("[" + SubmitItemUpdateResult_t.k_iCallback + " - SubmitItemUpdateResult] - " + pCallback.m_eResult + " -- " + pCallback.m_bUserNeedsToAcceptWorkshopLegalAgreement);


			m_UGCUpdateHandle = SteamUGC.StartItemUpdate(SteamUtils.GetAppID(), m_PublishedFileId);
			SteamUGC.SetItemPreview(m_UGCUpdateHandle, "C:/Users/admin/Desktop/DefaultPreviewImage.gif");
			SteamAPICall_t handle = SteamUGC.SubmitItemUpdate(m_UGCUpdateHandle, "submit img");
			OnSubmitItemUpdateResultCallResult.Set(handle);
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