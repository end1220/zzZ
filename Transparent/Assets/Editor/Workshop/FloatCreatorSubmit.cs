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
		string itemTitle = "";
		string itemDesc = "";
		string previewFilePath = "";
		string contentPath = "E:\\Locke\\GitHub\\zzZ\\Transparent\\Output\\Floating\\357893792";
		bool agreeWorkshopPolicy = false;

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

			GUILayout.BeginHorizontal();
			GUILayout.Space(spaceSize);
			GUILayout.Label("Content", FloatGUIStyle.boldLabel, GUILayout.Width(titleLen));
			GUILayout.Label(contentPath, FloatGUIStyle.boldLabel, GUILayout.Width(textLen));
			GUILayout.EndHorizontal();
			GUILayout.Space(leftSpace);

			GUILayout.BeginHorizontal();
			GUILayout.Space(leftSpace);
			agreeWorkshopPolicy = GUILayout.Toggle(agreeWorkshopPolicy, Language.Get(TextID.accept), GUILayout.Width(60));
			if (GUILayout.Button(Language.Get(TextID.legal), FloatGUIStyle.link, GUILayout.Width(130)))
			{
				Application.OpenURL("http://steamcommunity.com/sharedfiles/workshoplegalagreement");
			}
			GUILayout.EndHorizontal();
			GUILayout.Space(spaceSize);

			// submit button
			GUILayout.BeginHorizontal();
			GUILayout.Space(leftSpace);
			if (GUILayout.Button(Language.Get(TextID.submitToWorkshop), FloatGUIStyle.button, GUILayout.Width(200), GUILayout.Height(buttonHeight)))
			{
				if (CheckInputInfo())
					CreateItem();
			}
			GUILayout.EndHorizontal();
			GUILayout.Space(spaceSize);

			// steam info begin
			GUILayout.BeginHorizontal();
			GUILayout.Label("SteamID:", FloatGUIStyle.boldLabel, GUILayout.Width(titleLen));
			GUILayout.Label(SteamUser.GetSteamID().m_SteamID.ToString(), FloatGUIStyle.boldLabel, GUILayout.Width(textLen));
			GUILayout.EndHorizontal();

			GUILayout.BeginHorizontal();
			GUILayout.Label("AppID:", FloatGUIStyle.boldLabel, GUILayout.Width(titleLen));
			GUILayout.Label(SteamUtils.GetAppID().ToString(), FloatGUIStyle.boldLabel, GUILayout.Width(textLen));
			GUILayout.EndHorizontal();

			// test
			{
				ulong BytesProcessed;
				ulong BytesTotal;
				EItemUpdateStatus reti = SteamUGC.GetItemUpdateProgress(m_UGCUpdateHandle, out BytesProcessed, out BytesTotal);
				GUILayout.Label("GetItemUpdateProgress : ret " + reti + " -- " + BytesProcessed + " -- " + BytesTotal);
			}
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
			SteamUGC.SetItemContent(m_UGCUpdateHandle, contentPath);
			SteamUGC.SetItemPreview(m_UGCUpdateHandle, previewFilePath);

			SteamAPICall_t handle = SteamUGC.SubmitItemUpdate(m_UGCUpdateHandle, "submit content");
			OnSubmitItemUpdateResultCallResult.Set(handle);
		}

		void OnCreateItemResult(CreateItemResult_t pCallback, bool bIOFailure)
		{
			Debug.Log("[" + CreateItemResult_t.k_iCallback + " - CreateItemResult] - " + pCallback.m_eResult + " -- " + pCallback.m_nPublishedFileId + " -- " + pCallback.m_bUserNeedsToAcceptWorkshopLegalAgreement);
			if (pCallback.m_eResult == EResult.k_EResultOK)
			{
				m_PublishedFileId = pCallback.m_nPublishedFileId;
				UpdateItem();
			}
		}

		void OnSubmitItemUpdateResult(SubmitItemUpdateResult_t pCallback, bool bIOFailure)
		{
			Debug.Log("[" + SubmitItemUpdateResult_t.k_iCallback + " - SubmitItemUpdateResult] - " + pCallback.m_eResult + " -- " + pCallback.m_bUserNeedsToAcceptWorkshopLegalAgreement);
		}

		bool CheckInputInfo()
		{
			if (string.IsNullOrEmpty(itemTitle))
				return false;
			if (string.IsNullOrEmpty(itemDesc))
				return false;
			if (string.IsNullOrEmpty(itemTitle))
				return false;
			return true;
		}

	}
}