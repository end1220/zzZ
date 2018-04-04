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
		string previewPath = "";
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
			
			GUILayout.Box(Resources.Load(Path.GetFileNameWithoutExtension(previewPath)) as Texture, GUILayout.Width(128), GUILayout.Height(128));
			GUILayout.BeginVertical();
			GUILayout.Space(120);
			if (GUILayout.Button("Select", GUILayout.Width(buttonLen2)))
			{
				previewPath = EditorUtility.OpenFilePanel("Select Preview File", string.Empty, "jpg,png");
				if (!string.IsNullOrEmpty(previewPath) && File.Exists(previewPath))
				{
					File.Copy(previewPath, contentPath + "/" + FormatPreviewFileName(previewPath));
					File.Copy(previewPath, Application.dataPath + "/Editor/Resources/" + FormatPreviewFileName(previewPath));
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
				Application.OpenURL(AppConst.workshopPolicyUrl);
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

			{
				ulong BytesProcessed;
				ulong BytesTotal;
				EItemUpdateStatus reti = SteamUGC.GetItemUpdateProgress(m_UGCUpdateHandle, out BytesProcessed, out BytesTotal);
				if (reti != EItemUpdateStatus.k_EItemUpdateStatusInvalid)
				{
					EditorUtility.DisplayProgressBar("Uploading", reti.ToString(), (float)((double)BytesProcessed / (double)BytesTotal));
					//GUILayout.Label("GetItemUpdateProgress : ret " + reti + " -- " + BytesProcessed + " -- " + BytesTotal);
				}
				/*else
				{
					EditorUtility.ClearProgressBar();
				}*/
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
			SteamUGC.SetItemPreview(m_UGCUpdateHandle, GetContentPreviewPath());

			SteamAPICall_t handle = SteamUGC.SubmitItemUpdate(m_UGCUpdateHandle, "submit content");
			OnSubmitItemUpdateResultCallResult.Set(handle);
		}

		void OnCreateItemResult(CreateItemResult_t pCallback, bool bIOFailure)
		{
			//Debug.Log("[" + CreateItemResult_t.k_iCallback + " - CreateItemResult] - " + pCallback.m_eResult + " -- " + pCallback.m_nPublishedFileId + " -- " + pCallback.m_bUserNeedsToAcceptWorkshopLegalAgreement);
			if (pCallback.m_eResult == EResult.k_EResultOK)
			{
				m_PublishedFileId = pCallback.m_nPublishedFileId;
				ReplaceContentWithWorkshopID(m_PublishedFileId.m_PublishedFileId);
				UpdateItem();
			}
			else
			{
				string text = "Create workshop item failed. Error code : " + pCallback.m_eResult;
				EditorUtility.DisplayDialog("Error", text, "OK");
			}
		}

		void OnSubmitItemUpdateResult(SubmitItemUpdateResult_t pCallback, bool bIOFailure)
		{
			//Debug.Log("[" + SubmitItemUpdateResult_t.k_iCallback + " - SubmitItemUpdateResult] - " + pCallback.m_eResult + " -- " + pCallback.m_bUserNeedsToAcceptWorkshopLegalAgreement);
			if (pCallback.m_eResult == EResult.k_EResultOK)
			{
				EditorUtility.DisplayDialog("Complete", "Done", "OK");
			}
			else
			{
				EditorUtility.ClearProgressBar();
				string text = "Submit workshop item failed. Error code : " + pCallback.m_eResult;
				EditorUtility.DisplayDialog("Error", text, "OK");
			}
		}

		bool CheckInputInfo()
		{
			if (string.IsNullOrEmpty(itemTitle))
			{
				EditorUtility.DisplayDialog("Error", "Title cannot be empty.", "OK");
				return false;
			}
			if (string.IsNullOrEmpty(itemDesc))
			{
				EditorUtility.DisplayDialog("Error", "Description cannot be empty.", "OK");
				return false;
			}
			if (string.IsNullOrEmpty(contentPath))
			{
				EditorUtility.DisplayDialog("Error", "Description cannot be empty.", "OK");
				return false;
			}
			if (!Directory.Exists(contentPath))
			{
				EditorUtility.DisplayDialog("Error", "Content directory does not exist.", "OK");
				return false;
			}
			if (Directory.GetFiles(contentPath).Length < 3)
			{
				EditorUtility.DisplayDialog("Error", "Content directory cannot be empty.", "OK");
				return false;
			}
			if (!File.Exists(previewPath))
			{
				EditorUtility.DisplayDialog("Error", "Preview file does not exist.", "OK");
				return false;
			}
			if (!File.Exists(GetContentPreviewPath()))
			{
				EditorUtility.DisplayDialog("Error", "Preview file copy does not exist.", "OK");
				return false;
			}
			return true;
		}

		private string FormatPreviewFileName(string rawFileName)
		{
			return AppConst.previewName + Path.GetExtension(rawFileName);
		}

		private string GetContentPreviewPath()
		{
			return contentPath + FormatPreviewFileName(previewPath);
		}

		private void ReplaceContentWithWorkshopID(ulong workshopID)
		{
			contentPath = contentPath.Replace("\\", "/");
			string oldContentPath = contentPath;
			int endIdx = contentPath.LastIndexOf("/");
			string subContentPath = contentPath.Substring(0, endIdx);
			string oldFolderName = contentPath.Substring(endIdx);
			string newContentPath = subContentPath + "/" + workshopID;
			Directory.Move(oldContentPath, newContentPath);

			string newFolderName = workshopID.ToString();
			contentPath = newContentPath;

			// model data
			string modelDataPath = contentPath + "/" + oldFolderName + ".json";
			string text = File.ReadAllText(modelDataPath);
			ModelData data = JsonUtility.FromJson<ModelData>(text);
			SaveModelDataToFile(
					modelDataPath,
					workshopID.ToString(),
					data.title,
					data.description,
					data.preview,
					data.bundle,
					data.asset
					);

			// sbm
			string sbmPath = contentPath + "/" + oldFolderName + ".sbm";
			string jsonStr = File.ReadAllText(sbmPath);
			SubAssetBundleManifest subManifest = JsonConvert.DeserializeObject<SubAssetBundleManifest>(jsonStr);
			subManifest.ReplaceFolderInfo(oldFolderName, newFolderName);
			jsonStr = JsonConvert.SerializeObject(subManifest, Formatting.Indented);
			File.WriteAllText(sbmPath, jsonStr, Encoding.UTF8);
		}

	}
}