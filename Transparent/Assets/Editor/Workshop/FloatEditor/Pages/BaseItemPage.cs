using UnityEngine;
using UnityEditor;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using Steamworks;

namespace Float
{
	public class BaseItemPage : FloatEditorPage
	{
		protected ModelAssetBuilder modelBuilder = new ModelAssetBuilder();

		protected string itemTitle = "p0";
		protected string itemDesc = "hahahaha";
		protected string previewPath = "";
		protected string contentPath = "";
		protected bool agreeWorkshopPolicy = true;

		// steam api
		private CallResult<CreateItemResult_t> OnCreateItemResultCallResult;
		private CallResult<SubmitItemUpdateResult_t> OnSubmitItemUpdateResultCallResult;
		protected PublishedFileId_t mPublishedFileId;
		private UGCUpdateHandle_t mUGCUpdateHandle;


		public BaseItemPage(FloatEditorWindow creator) :
			base(creator)
		{
			
		}

		protected override void OnDestroy()
		{
			modelBuilder.OnDestroy();
		}

		protected override void OnGUI()
		{
			GUILayout.BeginVertical();

			modelBuilder.OnGUI();

			OnSubmitGUI();

			GUILayout.EndVertical();
		}

		protected override void OnShow(object param)
		{
			if (OnCreateItemResultCallResult == null)
				OnCreateItemResultCallResult = CallResult<CreateItemResult_t>.Create(OnCreateItemResult);
			if (OnSubmitItemUpdateResultCallResult == null)
				OnSubmitItemUpdateResultCallResult = CallResult<SubmitItemUpdateResult_t>.Create(OnSubmitItemUpdateResult);
		}

		private void OnSubmitGUI()
		{
			GUILayout.BeginHorizontal();
			GUILayout.Space(FloatGUIStyle.spaceSize);
			GUILayout.Label(Language.Get(TextID.Title), FloatGUIStyle.boldLabel, GUILayout.Width(FloatGUIStyle.titleLen));
			itemTitle = GUILayout.TextField(itemTitle, FloatGUIStyle.textField, GUILayout.Width(FloatGUIStyle.textLen));
			GUILayout.EndHorizontal();
			GUILayout.Space(FloatGUIStyle.leftSpace);

			GUILayout.BeginHorizontal();
			GUILayout.Space(FloatGUIStyle.spaceSize);
			GUILayout.Label(Language.Get(TextID.Desc), FloatGUIStyle.boldLabel, GUILayout.Width(FloatGUIStyle.titleLen));
			itemDesc = GUILayout.TextArea(itemDesc, FloatGUIStyle.textArea, GUILayout.Width(FloatGUIStyle.textLen), GUILayout.Height(140));
			GUILayout.EndHorizontal();
			GUILayout.Space(FloatGUIStyle.leftSpace);

			GUILayout.BeginHorizontal();
			GUILayout.Space(FloatGUIStyle.spaceSize);
			GUILayout.Label(Language.Get(TextID.Preview), FloatGUIStyle.boldLabel, GUILayout.Width(FloatGUIStyle.titleLen));

			GUILayout.Box(Resources.Load(AppConst.previewName) as Texture, GUILayout.Width(128), GUILayout.Height(128));
			GUILayout.BeginVertical();
			GUILayout.Space(120);
			if (GUILayout.Button(Language.Get(TextID.select), GUILayout.Width(FloatGUIStyle.buttonLen2)))
			{
				previewPath = EditorUtility.OpenFilePanel(Language.Get(TextID.selectPreview), string.Empty, "jpg,png");
				CopyPreviewFile(previewPath);
			}
			GUILayout.EndVertical();
			GUILayout.EndHorizontal();
			GUILayout.Space(FloatGUIStyle.leftSpace);

			GUILayout.BeginHorizontal();
			GUILayout.Space(FloatGUIStyle.spaceSize);
			GUILayout.Label("Content", FloatGUIStyle.boldLabel, GUILayout.Width(FloatGUIStyle.titleLen));
			GUILayout.Label(contentPath, FloatGUIStyle.boldLabel, GUILayout.Width(FloatGUIStyle.textLen));
			GUILayout.EndHorizontal();
			GUILayout.Space(FloatGUIStyle.leftSpace);

			GUILayout.BeginHorizontal();
			GUILayout.Space(FloatGUIStyle.leftSpace);
			agreeWorkshopPolicy = GUILayout.Toggle(agreeWorkshopPolicy, Language.Get(TextID.accept), GUILayout.Width(60));
			if (GUILayout.Button(Language.Get(TextID.legal), FloatGUIStyle.link, GUILayout.Width(130)))
				Application.OpenURL(AppConst.workshopPolicyUrl);
			GUILayout.EndHorizontal();
			GUILayout.Space(FloatGUIStyle.spaceSize);

			GUILayout.BeginHorizontal();
			GUILayout.Space(FloatGUIStyle.leftSpace);
			EditorGUI.BeginDisabledGroup(!agreeWorkshopPolicy);
			OnOperateGUI();
			EditorGUI.EndDisabledGroup();
			GUILayout.EndHorizontal();
			GUILayout.Space(FloatGUIStyle.spaceSize);

			/*SteamUser.GetSteamID().m_SteamID  SteamUtils.GetAppID()*/

			{
				ulong BytesProcessed;
				ulong BytesTotal;
				EItemUpdateStatus reti = SteamUGC.GetItemUpdateProgress(mUGCUpdateHandle, out BytesProcessed, out BytesTotal);
				if (reti != EItemUpdateStatus.k_EItemUpdateStatusInvalid)
				{
					EditorUtility.DisplayCancelableProgressBar("Submiting", reti.ToString(), (float)((double)BytesProcessed / (double)BytesTotal));
				}
			}
		}

		protected virtual void OnOperateGUI()
		{

		}

		protected void CreateItem()
		{
			SteamAPICall_t handle = SteamUGC.CreateItem(SteamUtils.GetAppID(), EWorkshopFileType.k_EWorkshopFileTypeCommunity);
			OnCreateItemResultCallResult.Set(handle);
		}

		protected void UpdateItem()
		{
			mUGCUpdateHandle = SteamUGC.StartItemUpdate(SteamUtils.GetAppID(), mPublishedFileId);

			SteamUGC.SetItemTitle(mUGCUpdateHandle, itemTitle);
			SteamUGC.SetItemDescription(mUGCUpdateHandle, itemDesc);
			SteamUGC.SetItemUpdateLanguage(mUGCUpdateHandle, "english");
			SteamUGC.SetItemMetadata(mUGCUpdateHandle, "This is the test metadata.");
			SteamUGC.SetItemVisibility(mUGCUpdateHandle, ERemoteStoragePublishedFileVisibility.k_ERemoteStoragePublishedFileVisibilityPublic);
			SteamUGC.SetItemTags(mUGCUpdateHandle, new string[] { "Tag One", "Tag Two", "Test Tags", "Sorry" });
			SteamUGC.SetItemContent(mUGCUpdateHandle, contentPath);
			SteamUGC.SetItemPreview(mUGCUpdateHandle, GetContentPreviewPath());

			SteamAPICall_t handle = SteamUGC.SubmitItemUpdate(mUGCUpdateHandle, "submit content");
			OnSubmitItemUpdateResultCallResult.Set(handle);
		}

		void OnCreateItemResult(CreateItemResult_t pCallback, bool bIOFailure)
		{
			if (pCallback.m_eResult == EResult.k_EResultOK)
			{
				mPublishedFileId = pCallback.m_nPublishedFileId;
				MakeContent(mPublishedFileId.m_PublishedFileId, modelBuilder.AssetBundlePath);

				EditorUtility.DisplayCancelableProgressBar("Submiting", "Update item", 0.3f);

				UpdateItem();
			}
			else
			{
				EditorUtility.ClearProgressBar();
				string text = "Create workshop item failed. Error code : " + pCallback.m_eResult;
				EditorUtility.DisplayDialog("Error", text, "OK");
			}
		}

		void OnSubmitItemUpdateResult(SubmitItemUpdateResult_t pCallback, bool bIOFailure)
		{
			if (pCallback.m_eResult == EResult.k_EResultOK)
			{
				EditorUtility.ClearProgressBar();
				EditorUtility.DisplayDialog("Complete", "Submit successfully", "OK");
			}
			else
			{
				EditorUtility.ClearProgressBar();
				string text = "Submit workshop item failed. Error code : " + pCallback.m_eResult;
				EditorUtility.DisplayDialog("Error", text, "OK");
			}
		}

		protected bool CheckInputInfo()
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
			string rawContentPath = modelBuilder.AssetBundlePath;
			if (string.IsNullOrEmpty(rawContentPath))
			{
				EditorUtility.DisplayDialog("Error", "Content directory cannot be empty.", "OK");
				return false;
			}
			if (!Directory.Exists(rawContentPath))
			{
				EditorUtility.DisplayDialog("Error", "Content directory does not exist.", "OK");
				return false;
			}
			if (Directory.GetFiles(rawContentPath).Length < 3)
			{
				EditorUtility.DisplayDialog("Error", "Content directory cannot be empty.", "OK");
				return false;
			}
			if (!File.Exists(previewPath))
			{
				EditorUtility.DisplayDialog("Error", "Preview file does not exist.", "OK");
				return false;
			}
			return true;
		}

		protected string FormatPreviewFileName(string rawFileName)
		{
			return AppConst.previewName + Path.GetExtension(rawFileName);
		}

		protected string GetContentPreviewPath()
		{
			return contentPath + "/" + FormatPreviewFileName(previewPath);
		}

		protected void MakeContent(ulong workshopID, string modelAssetbundlePath)
		{
			if (!Directory.Exists(modelAssetbundlePath))
				return;

			modelAssetbundlePath = modelAssetbundlePath.Replace("\\", "/");
			int endIdx = modelAssetbundlePath.LastIndexOf("/");
			//string subContentPath = modelAssetbundlePath.Substring(0, endIdx);
			string oldFolderName = modelAssetbundlePath.Substring(endIdx + 1);
			string newContentPath = Path.Combine(AppConst.projectsPath, workshopID.ToString());//subContentPath + "/temporary";
			if (Directory.Exists(newContentPath))
				FileUtil.DeleteFileOrDirectory(newContentPath);
			//Directory.CreateDirectory(newContentPath);

			FileUtil.CopyFileOrDirectory(modelAssetbundlePath, newContentPath);

			contentPath = newContentPath;

			ReplaceContentWithWorkshopID(workshopID, oldFolderName);
		}

		protected void ReplaceContentWithWorkshopID(ulong workshopID, string oldFolderName)
		{
			string newFolderName = workshopID.ToString();

			// model data
			string modelDataPath = contentPath + "/" + AppConst.subModelDataName;
			string text = File.ReadAllText(modelDataPath);
			ModelData data = JsonUtility.FromJson<ModelData>(text);
			ModelAssetBuilder.SaveModelDataToFile(
					modelDataPath,
					workshopID.ToString(),
					itemTitle,
					itemDesc,
					FormatPreviewFileName(previewPath),
					data.bundle,
					data.asset
					);

			// sbm
			string sbmPath = contentPath + "/" + AppConst.assetbundleName + ".sbm";
			string jsonStr = File.ReadAllText(sbmPath);
			SubAssetBundleManifest subManifest = JsonConvert.DeserializeObject<SubAssetBundleManifest>(jsonStr);
			subManifest.ReplaceFolderInfo(oldFolderName, newFolderName);
			jsonStr = JsonConvert.SerializeObject(subManifest, Formatting.Indented);
			File.WriteAllText(sbmPath, jsonStr, Encoding.UTF8);
		}

		protected void CopyPreviewFile(string previewPath)
		{
			if (!string.IsNullOrEmpty(previewPath) && File.Exists(previewPath))
			{
				File.Copy(previewPath, contentPath + "/" + FormatPreviewFileName(previewPath), true);
				File.Copy(previewPath, Application.dataPath + "/Editor/Resources/" + FormatPreviewFileName(previewPath), true);
				AssetDatabase.Refresh();
			}
		}

		protected void ClearTempDirectory()
		{
			string path2 = Application.dataPath + "/Editor/Resources/" + FormatPreviewFileName(previewPath);
			if (File.Exists(path2))
				File.Delete(path2);

			AssetDatabase.Refresh();
		}

	}
}