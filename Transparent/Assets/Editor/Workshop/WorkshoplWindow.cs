using UnityEngine;
using UnityEditor;
using System;
using Steamworks;


namespace Lite
{
	public class WorkshopWindow : EditorWindow
	{

		[MenuItem(AppDefine.AppName + "/Workshop")]
		public static void ShowExcelWindow()
		{
			Rect wr = new Rect(100, 100, 640, 480);
			var window = (WorkshopWindow)EditorWindow.GetWindowWithRect(typeof(WorkshopWindow), wr, true, "Upload");
			window.Show();
		}

		string zipFilePath = "";
		string itemTitle = "No title";
		string previewFilePath = "";
		bool agreeWorkshopPolicy = false;

		void Awake()
		{
			InitSteamAPI();
		}

		private void OnDestroy()
		{
			DestroySteamAPI();
		}

		void OnGUI()
		{
			if (SteamManager.Instance.Initialized)
			{
				float spaceSize = 10f;
				float leftSpace = 10;
				float titleLen = 70;
				float textLen = 450;
				float buttonLen2 = 50;
				float buttonHeight = 40;

				GUILayout.Label("Workshop", EditorStyles.helpBox);
				GUILayout.Space(spaceSize);

				GUILayout.BeginHorizontal();
				GUILayout.Space(leftSpace);
				GUILayout.Label("Zip File", EditorStyles.label, GUILayout.Width(titleLen));
				string savedZipPath = EditorPrefs.GetString("BMW_ZipPath");
				zipFilePath = string.IsNullOrEmpty(savedZipPath) ? zipFilePath : savedZipPath;
				zipFilePath = GUILayout.TextField(zipFilePath, GUILayout.Width(textLen));
				if (GUILayout.Button("Select", GUILayout.Width(buttonLen2)))
				{
					zipFilePath = EditorUtility.OpenFilePanel("Select Zip File", String.Empty, "*zip");
					if (!string.IsNullOrEmpty(zipFilePath))
						EditorPrefs.SetString("BMW_ZipPath", zipFilePath);
				}
				GUILayout.EndHorizontal();
				GUILayout.Space(spaceSize);

				GUILayout.BeginHorizontal();
				GUILayout.Space(spaceSize);
				GUILayout.Label("Title", EditorStyles.label, GUILayout.Width(titleLen));
				itemTitle = GUILayout.TextField(itemTitle, GUILayout.Width(textLen));
				GUILayout.EndHorizontal();
				GUILayout.Space(leftSpace);

				GUILayout.BeginHorizontal();
				GUILayout.Space(spaceSize);
				GUILayout.Label("Preview", EditorStyles.label, GUILayout.Width(titleLen));
				string savedPreviewPath = EditorPrefs.GetString("BMW_PreviewPath");
				previewFilePath = string.IsNullOrEmpty(savedPreviewPath) ? previewFilePath : savedPreviewPath;
				previewFilePath = GUILayout.TextField(previewFilePath, GUILayout.Width(textLen));
				if (GUILayout.Button("Select", GUILayout.Width(buttonLen2)))
				{
					zipFilePath = EditorUtility.OpenFilePanel("Select Preview File", String.Empty, "*.jpg");
					if (!string.IsNullOrEmpty(zipFilePath))
						EditorPrefs.SetString("BMW_PreviewPath", zipFilePath);
				}
				GUILayout.EndHorizontal();
				GUILayout.Space(leftSpace);

				GUILayout.BeginHorizontal();
				GUILayout.Space(leftSpace);
				agreeWorkshopPolicy = GUILayout.Toggle(agreeWorkshopPolicy, "同意", GUILayout.Width(40));
				if (GUILayout.Button("《创意工坊服务条款》", EditorStyles.label, GUILayout.Width(130)))
				{
					Application.OpenURL("http://steamcommunity.com/sharedfiles/workshoplegalagreement");
				}
				GUILayout.EndHorizontal();
				GUILayout.Space(spaceSize);

				GUILayout.BeginHorizontal();
				GUILayout.Space(leftSpace);
				if (GUILayout.Button("提交至创意工坊", GUILayout.Width(200), GUILayout.Height(buttonHeight)))
				{
					if (EditorUtility.DisplayDialog("创意工坊法律协议",
						"提交物品的同时也表示您同意了《创意工坊服务条款》:http://steamcommunity.com/sharedfiles/workshoplegalagreement",
						"确定",
						"取消"))
					{
						CreateItem();
					}
					else
					{

					}
				}

				if (GUILayout.Button("提交至创意工坊", GUILayout.Width(200), GUILayout.Height(buttonHeight)))
				{
					UpdateItem();
				}
				GUILayout.EndHorizontal();
				GUILayout.Space(spaceSize);

				OnUpdateItemGUI();
			}
			else
			{
				GUILayout.Label("SteamAPI Initialized failed", EditorStyles.label);
			}
		}

		//===================================================================================
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
			Debug.Log("SteamUGC.CreateItem(" + SteamUtils.GetAppID() + ", " + EWorkshopFileType.k_EWorkshopFileTypeCommunity + ") : " + handle);
		}

		void OnCreateItemResult(CreateItemResult_t pCallback, bool bIOFailure)
		{
			Debug.Log("[" + CreateItemResult_t.k_iCallback + " - CreateItemResult] - " + pCallback.m_eResult + " -- " + pCallback.m_nPublishedFileId + " -- " + pCallback.m_bUserNeedsToAcceptWorkshopLegalAgreement);

			m_PublishedFileId = pCallback.m_nPublishedFileId;
		}

		private void UpdateItem()
		{
			m_UGCUpdateHandle = SteamUGC.StartItemUpdate(SteamUtils.GetAppID(), m_PublishedFileId);
			Debug.Log("SteamUGC.StartItemUpdate(" + SteamUtils.GetAppID() + ", " + m_PublishedFileId + ") : " + m_UGCUpdateHandle);

			if (SteamUGC.SetItemTitle(m_UGCUpdateHandle, itemTitle))
				Debug.Log("SteamUGC.SetItemTitle(" + m_UGCUpdateHandle + ", " + itemTitle + ")");
			else
				Debug.Log("SteamUGC.SetItemTitle failed");

			if ( SteamUGC.SetItemDescription(m_UGCUpdateHandle, "This is the test description."))
				Debug.Log("SteamUGC.SetItemDescription(" + m_UGCUpdateHandle + ", " + "\"This is the test description.\"" + ") : ");
			else
				Debug.Log("SteamUGC.SetItemDescription failed");

			if ( SteamUGC.SetItemUpdateLanguage(m_UGCUpdateHandle, "english"))
				Debug.Log("SteamUGC.SetItemUpdateLanguage(" + m_UGCUpdateHandle + ", " + "\"english\"" + ") : ");
			else
				Debug.Log("SteamUGC.SetItemUpdateLanguage failed");

			if ( SteamUGC.SetItemMetadata(m_UGCUpdateHandle, "This is the test metadata."))
				Debug.Log("SteamUGC.SetItemMetadata(" + m_UGCUpdateHandle + ", " + "\"This is the test metadata.\"" + ") : ");
			else
				Debug.Log("SteamUGC.SetItemMetadata failed");

			if ( SteamUGC.SetItemVisibility(m_UGCUpdateHandle, ERemoteStoragePublishedFileVisibility.k_ERemoteStoragePublishedFileVisibilityPublic))
				Debug.Log("SteamUGC.SetItemVisibility(" + m_UGCUpdateHandle + ", " + ERemoteStoragePublishedFileVisibility.k_ERemoteStoragePublishedFileVisibilityPublic + ") : ");
			else
				Debug.Log("SteamUGC.SetItemVisibility failed");

			if ( SteamUGC.SetItemTags(m_UGCUpdateHandle, new string[] { "Tag One", "Tag Two", "Test Tags", "Sorry" }))
				Debug.Log("SteamUGC.SetItemTags(" + m_UGCUpdateHandle + ", " + new string[] { "Tag One", "Tag Two", "Test Tags", "Sorry" } + ") : ");
			else
				Debug.Log("SteamUGC.SetItemTags failed");

			if ( SteamUGC.SetItemContent(m_UGCUpdateHandle, Application.dataPath + "/Scenes"))
				Debug.Log("SteamUGC.SetItemContent(" + m_UGCUpdateHandle + ", " + Application.dataPath + "/Scenes" + ") : ");
			else
				Debug.Log("SteamUGC.SetItemContent failed");

			if ( SteamUGC.SetItemPreview(m_UGCUpdateHandle, Application.dataPath + "/PreviewImage.jpg"))
				Debug.Log("SteamUGC.SetItemPreview(" + m_UGCUpdateHandle + ", " + Application.dataPath + "/PreviewImage.jpg" + ") : ");
			else
				Debug.Log("SteamUGC.SetItemPreview failed");

			/*if ( SteamUGC.RemoveItemKeyValueTags(m_UGCUpdateHandle, "TestKey");
			Debug.Log("SteamUGC.RemoveItemKeyValueTags(" + m_UGCUpdateHandle + ", " + "\"TestKey\"" + ") : ");

			if ( SteamUGC.AddItemKeyValueTag(m_UGCUpdateHandle, "TestKey", "TestValue");
			Debug.Log("SteamUGC.AddItemKeyValueTag(" + m_UGCUpdateHandle + ", " + "\"TestKey\"" + ", " + "\"TestValue\"" + ") : ");

			if ( SteamUGC.AddItemPreviewFile(m_UGCUpdateHandle, Application.dataPath + "/PreviewImage.jpg", EItemPreviewType.k_EItemPreviewType_Image);
			Debug.Log("SteamUGC.AddItemPreviewFile(" + m_UGCUpdateHandle + ", " + Application.dataPath + "/PreviewImage.jpg" + ", " + EItemPreviewType.k_EItemPreviewType_Image + ") : ");

			if ( SteamUGC.AddItemPreviewVideo(m_UGCUpdateHandle, "jHgZh4GV9G0");
			Debug.Log("SteamUGC.AddItemPreviewVideo(" + m_UGCUpdateHandle + ", " + "\"jHgZh4GV9G0\"" + ") : ");

			if ( SteamUGC.UpdateItemPreviewFile(m_UGCUpdateHandle, 0, Application.dataPath + "/PreviewImage.jpg");
			Debug.Log("SteamUGC.UpdateItemPreviewFile(" + m_UGCUpdateHandle + ", " + 0 + ", " + Application.dataPath + "/PreviewImage.jpg" + ") : ");

			if ( SteamUGC.UpdateItemPreviewVideo(m_UGCUpdateHandle, 0, "jHgZh4GV9G0");
			Debug.Log("SteamUGC.UpdateItemPreviewVideo(" + m_UGCUpdateHandle + ", " + 0 + ", " + "\"jHgZh4GV9G0\"" + ") : ");

			if ( SteamUGC.RemoveItemPreview(m_UGCUpdateHandle, 0);
			Debug.Log("SteamUGC.RemoveItemPreview(" + m_UGCUpdateHandle + ", " + 0 + ") : ");*/


			SteamAPICall_t handle = SteamUGC.SubmitItemUpdate(m_UGCUpdateHandle, "Test Changenote");
			OnSubmitItemUpdateResultCallResult.Set(handle);
			Debug.Log("SteamUGC.SubmitItemUpdate(" + m_UGCUpdateHandle + ", " + "\"Test Changenote\"" + ") : " + handle);
		}

		void OnUpdateItemGUI()
		{
			ulong BytesProcessed;
			ulong BytesTotal;
			EItemUpdateStatus ret = SteamUGC.GetItemUpdateProgress(m_UGCUpdateHandle, out BytesProcessed, out BytesTotal);
			GUILayout.Label("GetItemUpdateProgress : ret " + ret + " -- " + BytesProcessed + " -- " + BytesTotal);
		}

		void OnSubmitItemUpdateResult(SubmitItemUpdateResult_t pCallback, bool bIOFailure)
		{
			Debug.Log("[" + SubmitItemUpdateResult_t.k_iCallback + " - SubmitItemUpdateResult] - " + pCallback.m_eResult + " -- " + pCallback.m_bUserNeedsToAcceptWorkshopLegalAgreement);
		}

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