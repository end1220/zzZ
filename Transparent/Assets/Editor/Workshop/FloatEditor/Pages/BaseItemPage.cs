using UnityEngine;
using UnityEditor;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using Steamworks;

namespace Float
{
	public enum CategoryType
	{
		Unspecified,
		Scene,
		Video,
		Web,
		App
	}

	public enum Genre
	{
		Unspecified,
		Abstract,
		Animal,
		Cartoon,
		Games,
		Girls,
		Guys,
		Fantacy,
		Nature,
		Music
	}

	public enum Rating
	{
		Unspecified,
		Everybody,
		Suspicious,
		Mature
	}

	public enum PublishedVisibility
	{
		Unspecified,
		Public,
		FriendsOnly,
		Private,
	}

	public abstract class BaseItemPage : BasePage
	{
		protected ModelAssetBuilder modelBuilder = new ModelAssetBuilder();

		private string LayoutSavePath = System.Environment.CurrentDirectory.Replace("\\", "/") + "/ProjectSettings/ItemLayout";

		[JsonObject(MemberSerialization.OptIn)]
		public class Context
		{
			[JsonProperty]
			public string itemTitle = "";

			[JsonProperty]
			public string itemDesc = "";

			[JsonProperty]
			public string previewPath = "";

			[JsonProperty]
			public string contentPath = "";

			[JsonProperty]
			public bool agreeWorkshopPolicy = true;

			[JsonProperty]
			public PublishedVisibility visibility;

			[JsonProperty]
			public CategoryType category = CategoryType.Unspecified;

			[JsonProperty]
			public Genre genre = Genre.Unspecified;

			[JsonProperty]
			public Rating rating = Rating.Unspecified;

			[JsonIgnore]
			public bool dirty;

			[JsonIgnore]
			public string ItemTitle
			{
				get
				{
					return itemTitle;
				}
				set
				{
					if (itemTitle != value)
						dirty = true;
					itemTitle = value;
				}
			}

			[JsonIgnore]
			public string ItemDesc
			{
				get
				{
					return itemDesc;
				}
				set
				{
					if (itemDesc != value)
						dirty = true;
					itemDesc = value;
				}
			}

			[JsonIgnore]
			public string PreviewPath
			{
				get
				{
					return previewPath;
				}
				set
				{
					if (previewPath != value)
						dirty = true;
					previewPath = value;
				}
			}

			[JsonIgnore]
			public string ContentPath
			{
				get
				{
					return contentPath;
				}
				set
				{
					if (contentPath != value)
						dirty = true;
					contentPath = value;
				}
			}

			[JsonIgnore]
			public bool AgreeWorkshopPolicy
			{
				get
				{
					return agreeWorkshopPolicy;
				}
				set
				{
					if (agreeWorkshopPolicy != value)
						dirty = true;
					agreeWorkshopPolicy = value;
				}
			}

			[JsonIgnore]
			public PublishedVisibility Visibility
			{
				get
				{
					return visibility;
				}
				set
				{
					if (visibility != value)
						dirty = true;
					visibility = value;
				}
			}

			[JsonIgnore]
			public CategoryType Category
			{
				get
				{
					return category;
				}
				set
				{
					if (category != value)
						dirty = true;
					category = value;
				}
			}

			[JsonIgnore]
			public Genre Genre
			{
				get
				{
					return genre;
				}
				set
				{
					if (genre != value)
						dirty = true;
					genre = value;
				}
			}

			[JsonIgnore]
			public Rating Rating
			{
				get
				{
					return rating;
				}
				set
				{
					if (rating != value)
						dirty = true;
					rating = value;
				}
			}
		}

		protected Context context = new Context();

		// steam api
		private CallResult<CreateItemResult_t> OnCreateItemResultCallResult;
		private CallResult<SubmitItemUpdateResult_t> OnSubmitItemUpdateResultCallResult;
		protected PublishedFileId_t mPublishedFileId;
		private UGCUpdateHandle_t mUGCUpdateHandle;
		private bool isUpdatingItem = false;
		private EItemUpdateStatus lastItemUpdateStatus = EItemUpdateStatus.k_EItemUpdateStatusInvalid;

		public BaseItemPage(FloatEditorWindow creator) :
			base(creator)
		{
			
		}

		protected override void OnDestroy()
		{
			modelBuilder.OnDestroy();

			if (File.Exists(LayoutSavePath))
				File.Delete(LayoutSavePath);

			ClearTempDirectory();
		}

		protected override void OnUpdate()
		{
			if (isUpdatingItem)
			{
				ulong BytesProcessed;
				ulong BytesTotal;
				EItemUpdateStatus ret = SteamUGC.GetItemUpdateProgress(mUGCUpdateHandle, out BytesProcessed, out BytesTotal);
				if (ret != EItemUpdateStatus.k_EItemUpdateStatusInvalid)
				{
					lastItemUpdateStatus = ret;
					int textId = 0;
					switch (ret)
					{
						case EItemUpdateStatus.k_EItemUpdateStatusPreparingConfig:
							textId = TextID.PreparingConfig;
							break;
						case EItemUpdateStatus.k_EItemUpdateStatusPreparingContent:
							textId = TextID.PreparingContent;
							break;
						case EItemUpdateStatus.k_EItemUpdateStatusUploadingContent:
							textId = TextID.UploadingContent;
							break;
						case EItemUpdateStatus.k_EItemUpdateStatusUploadingPreviewFile:
							textId = TextID.ploadingPreviewFile;
							break;
						case EItemUpdateStatus.k_EItemUpdateStatusCommittingChanges:
							textId = TextID.CommittingChanges;
							break;
					}
					EditorUtility.DisplayCancelableProgressBar(Language.Get(TextID.submitting), Language.Get(textId), (float)((double)BytesProcessed / (double)BytesTotal));
				}
				else
				{
					isUpdatingItem = false;
					EditorUtility.ClearProgressBar();
					if (lastItemUpdateStatus == EItemUpdateStatus.k_EItemUpdateStatusCommittingChanges)
					{
						lastItemUpdateStatus = EItemUpdateStatus.k_EItemUpdateStatusInvalid;
						EditorUtility.DisplayDialog(Language.Get(TextID.complete), Language.Get(TextID.submitDone), Language.Get(TextID.ok));
					}
				}
			}
		}

		bool foldoutModel = true;
		bool foldoutSubmit = true;
		protected override void OnGUI()
		{
			GUILayout.Space(10);// to top
			GUILayout.BeginVertical();
			GUILayout.BeginHorizontal();
			GUILayout.Space(10);

			GUILayout.BeginVertical();
			foldoutModel = EditorGUILayout.Foldout(foldoutModel, Language.Get(TextID.step1), true, FloatGUIStyle.foldout);
			GUILayout.Space(5);
			if (foldoutModel)
			{
				GUILayout.BeginHorizontal();
				GUILayout.Space(10);
				GUILayout.Label("", FloatGUIStyle.helpBox, GUILayout.Width(750), GUILayout.Height(170));
				GUILayout.EndHorizontal();
				GUILayout.BeginArea(new Rect(25, 50, 750, 170));
				modelBuilder.OnGUI();
				GUILayout.EndArea();
			}
			GUILayout.Space(10);

			foldoutSubmit = EditorGUILayout.Foldout(foldoutSubmit, Language.Get(TextID.step2), true, FloatGUIStyle.foldout);
			GUILayout.Space(5);
			if (foldoutSubmit)
			{
				GUILayout.BeginHorizontal();
				GUILayout.Space(10);
				GUILayout.Label("", FloatGUIStyle.helpBox, GUILayout.Width(750), GUILayout.Height(440));
				GUILayout.EndHorizontal();
				GUILayout.BeginArea(new Rect(25, foldoutModel ? 260 : 85, 750, 420));
				OnItemGUI();
				GUILayout.EndArea();
			}
			GUILayout.EndVertical();

			GUILayout.EndHorizontal();
			GUILayout.EndVertical();
		}

		protected override void OnShow(object param)
		{
			if (File.Exists(LayoutSavePath))
			{
				context = JsonConvert.DeserializeObject<Context>(File.ReadAllText(LayoutSavePath));
			}

			// open old project
			if (param != null)
			{
				ProjectItemData project = (ProjectItemData)param;
				ModelData data = project.modeldata;
				context.itemTitle = data.title;
				context.itemDesc = data.description;
				context.previewPath = Path.Combine(project.directory, data.preview).Replace("\\", "/");
				context.contentPath = Path.Combine(project.directory, AppConst.contentFolderName).Replace("\\", "/");
				CopyPreviewFile(context.PreviewPath);
			}

			if (OnCreateItemResultCallResult == null)
				OnCreateItemResultCallResult = CallResult<CreateItemResult_t>.Create(OnCreateItemResult);
			if (OnSubmitItemUpdateResultCallResult == null)
				OnSubmitItemUpdateResultCallResult = CallResult<SubmitItemUpdateResult_t>.Create(OnSubmitItemUpdateResult);
		}

		protected override void SaveContext()
		{
			modelBuilder.SaveContext();

			if (context.dirty)
			{
				string str = JsonConvert.SerializeObject(context);
				File.WriteAllText(LayoutSavePath, str);
				context.dirty = false;
			}
		}

		private void OnItemGUI()
		{
			GUILayout.BeginHorizontal();
			GUILayout.Space(FloatGUIStyle.leftSpace);
			GUILayout.Label(Language.Get(TextID.Title), FloatGUIStyle.boldLabel, GUILayout.Width(FloatGUIStyle.titleLen));
			context.ItemTitle = FloatGUIStyle.TextField(context.ItemTitle, FloatGUIStyle.textField, GUILayout.Width(FloatGUIStyle.textLen));
			GUILayout.Label(Language.Get(TextID.required), FloatGUIStyle.red);
			GUILayout.EndHorizontal();
			GUILayout.Space(FloatGUIStyle.spaceSize);

			GUILayout.BeginHorizontal();
			GUILayout.Space(FloatGUIStyle.leftSpace);
			GUILayout.Label(Language.Get(TextID.Desc), FloatGUIStyle.boldLabel, GUILayout.Width(FloatGUIStyle.titleLen));
			context.ItemDesc = FloatGUIStyle.TextArea(context.ItemDesc, FloatGUIStyle.textArea, GUILayout.Width(FloatGUIStyle.textLen), GUILayout.Height(100));
			GUILayout.EndHorizontal();
			GUILayout.Space(FloatGUIStyle.spaceSize);

			GUILayout.BeginHorizontal();

			GUILayout.Space(FloatGUIStyle.leftSpace);
			GUILayout.Label(Language.Get(TextID.Preview), FloatGUIStyle.boldLabel, GUILayout.Width(FloatGUIStyle.titleLen));
			GUILayout.Box(Resources.Load(AppConst.previewName) as Texture, GUILayout.Width(128), GUILayout.Height(128));

			GUILayout.BeginVertical();
			GUILayout.Label(Language.Get(TextID.required), FloatGUIStyle.red);
			GUILayout.Space(100);
			if (GUILayout.Button(Language.Get(TextID.select), GUILayout.Width(FloatGUIStyle.buttonLen2)))
			{
				context.PreviewPath = EditorUtility.OpenFilePanel(Language.Get(TextID.selectPreview), string.Empty, "jpg,png");
				CopyPreviewFile(context.PreviewPath);
			}
			GUILayout.EndVertical();

			GUILayout.BeginVertical();

			GUILayout.BeginHorizontal();
			GUILayout.Label(Language.Get(TextID.category), FloatGUIStyle.boldLabel, GUILayout.Width(60));
			context.Category = (CategoryType)EditorGUILayout.EnumPopup(context.Category, GUILayout.Width(150));
			GUILayout.Label(Language.Get(TextID.required), FloatGUIStyle.red);
			GUILayout.EndHorizontal();

			GUILayout.Space(10);
			GUILayout.BeginHorizontal();
			GUILayout.Label(Language.Get(TextID.genre), FloatGUIStyle.boldLabel, GUILayout.Width(60));
			context.Genre = (Genre)EditorGUILayout.EnumPopup(context.Genre, GUILayout.Width(150));
			GUILayout.Label(Language.Get(TextID.required), FloatGUIStyle.red);
			GUILayout.EndHorizontal();

			GUILayout.Space(10);
			GUILayout.BeginHorizontal();
			GUILayout.Label(Language.Get(TextID.rating), FloatGUIStyle.boldLabel, GUILayout.Width(60));
			context.Rating = (Rating)EditorGUILayout.EnumPopup(context.Rating, GUILayout.Width(150));
			GUILayout.Label(Language.Get(TextID.required), FloatGUIStyle.red);
			GUILayout.EndHorizontal();

			GUILayout.Space(10);
			GUILayout.BeginHorizontal();
			GUILayout.Label(Language.Get(TextID.visibility), FloatGUIStyle.boldLabel, GUILayout.Width(60));
			context.Visibility = (PublishedVisibility)EditorGUILayout.EnumPopup(context.Visibility, GUILayout.Width(150));
			GUILayout.Label(Language.Get(TextID.required), FloatGUIStyle.red);
			GUILayout.EndHorizontal();

			GUILayout.EndVertical();
			GUILayout.EndHorizontal();
			GUILayout.Space(FloatGUIStyle.spaceSize);

			GUILayout.BeginHorizontal();
			GUILayout.Space(FloatGUIStyle.leftSpace);
			GUILayout.Label(Language.Get(TextID.content), FloatGUIStyle.boldLabel, GUILayout.Width(FloatGUIStyle.titleLen));
			GUILayout.Label(context.ContentPath, FloatGUIStyle.label, GUILayout.Width(FloatGUIStyle.textLen));
			GUILayout.EndHorizontal();
			GUILayout.Space(FloatGUIStyle.spaceSize);

			GUILayout.BeginHorizontal();
			GUILayout.Space(FloatGUIStyle.leftSpace);
			context.AgreeWorkshopPolicy = GUILayout.Toggle(context.AgreeWorkshopPolicy, Language.Get(TextID.accept), GUILayout.Width(60));
			if (GUILayout.Button(Language.Get(TextID.legal), FloatGUIStyle.link, GUILayout.Width(130)))
				Application.OpenURL(AppConst.workshopPolicyUrl);
			GUILayout.EndHorizontal();
			GUILayout.Space(FloatGUIStyle.spaceSize);

			GUILayout.BeginHorizontal();
			GUILayout.Space(FloatGUIStyle.leftSpace);
			EditorGUI.BeginDisabledGroup(!context.AgreeWorkshopPolicy);
			OnOperateGUI();
			EditorGUI.EndDisabledGroup();
			GUILayout.EndHorizontal();
			GUILayout.Space(FloatGUIStyle.spaceSize);

			/*SteamUser.GetSteamID().m_SteamID  SteamUtils.GetAppID()*/
		}

		protected virtual void OnOperateGUI() { }

		protected void CreateItem()
		{
			SteamAPICall_t handle = SteamUGC.CreateItem(SteamUtils.GetAppID(), EWorkshopFileType.k_EWorkshopFileTypeCommunity);
			OnCreateItemResultCallResult.Set(handle);
		}

		protected void UpdateItem()
		{
			mUGCUpdateHandle = SteamUGC.StartItemUpdate(SteamUtils.GetAppID(), mPublishedFileId);

			SteamUGC.SetItemTitle(mUGCUpdateHandle, context.ItemTitle);
			SteamUGC.SetItemDescription(mUGCUpdateHandle, context.ItemDesc);
			string lang = "english";
			switch (Language.langType)
			{
				case LangType.Chinese:lang = "simplified chinese";break;
				case LangType.English:lang = "english";break;
				case LangType.Japanese:lang = "japanese";break;
				case LangType.Korean:lang = "korean";break;
			}
			SteamUGC.SetItemUpdateLanguage(mUGCUpdateHandle, lang);
			//SteamUGC.SetItemMetadata(mUGCUpdateHandle, "null");
			ERemoteStoragePublishedFileVisibility visb = ERemoteStoragePublishedFileVisibility.k_ERemoteStoragePublishedFileVisibilityPublic;
			switch (context.Visibility)
			{
				case PublishedVisibility.FriendsOnly:
					visb = ERemoteStoragePublishedFileVisibility.k_ERemoteStoragePublishedFileVisibilityFriendsOnly;
					break;
				case PublishedVisibility.Private:
					visb = ERemoteStoragePublishedFileVisibility.k_ERemoteStoragePublishedFileVisibilityPrivate;
					break;
				case PublishedVisibility.Public:
					visb = ERemoteStoragePublishedFileVisibility.k_ERemoteStoragePublishedFileVisibilityPublic;
					break;
			}
			SteamUGC.SetItemVisibility(mUGCUpdateHandle, visb);
			//SteamUGC.SetItemTags(mUGCUpdateHandle, new string[] { "Tag One", "Tag Two", "Test Tags", "Sorry" });
			SteamUGC.RemoveItemKeyValueTags(mUGCUpdateHandle, "Type");
			SteamUGC.AddItemKeyValueTag(mUGCUpdateHandle, "Type", context.Category.ToString());
			SteamUGC.RemoveItemKeyValueTags(mUGCUpdateHandle, "Rating");
			SteamUGC.AddItemKeyValueTag(mUGCUpdateHandle, "Rating", context.Rating.ToString());
			SteamUGC.RemoveItemKeyValueTags(mUGCUpdateHandle, "Genre");
			SteamUGC.AddItemKeyValueTag(mUGCUpdateHandle, "Genre", context.Genre.ToString());
			SteamUGC.SetItemContent(mUGCUpdateHandle, context.ContentPath);
			SteamUGC.SetItemPreview(mUGCUpdateHandle, GetContentPreviewPath());

			SteamAPICall_t handle = SteamUGC.SubmitItemUpdate(mUGCUpdateHandle, "");
			OnSubmitItemUpdateResultCallResult.Set(handle);

			isUpdatingItem = true;
		}

		void OnCreateItemResult(CreateItemResult_t pCallback, bool bIOFailure)
		{
			if (pCallback.m_eResult == EResult.k_EResultOK)
			{
				mPublishedFileId = pCallback.m_nPublishedFileId;
				MakeContent(mPublishedFileId.m_PublishedFileId, modelBuilder.AssetbundlePath);

				EditorUtility.DisplayCancelableProgressBar(Language.Get(TextID.submitting), "", 0.3f);

				UpdateItem();
			}
			else
			{
				EditorUtility.ClearProgressBar();
				string text = Language.Get(TextID.createFailed) + pCallback.m_eResult;
				EditorUtility.DisplayDialog(Language.Get(TextID.error), text, Language.Get(TextID.ok));
			}
		}

		void OnSubmitItemUpdateResult(SubmitItemUpdateResult_t pCallback, bool bIOFailure)
		{
			isUpdatingItem = false;
			lastItemUpdateStatus = EItemUpdateStatus.k_EItemUpdateStatusInvalid;

			if (pCallback.m_eResult == EResult.k_EResultOK)
			{
				EditorUtility.ClearProgressBar();
				EditorUtility.DisplayDialog(Language.Get(TextID.complete), Language.Get(TextID.submitDone), Language.Get(TextID.ok));
			}
			else
			{
				EditorUtility.ClearProgressBar();
				string text = Language.Get(TextID.submitFailed) + pCallback.m_eResult;
				EditorUtility.DisplayDialog(Language.Get(TextID.error), text, Language.Get(TextID.ok));
			}
		}

		protected bool CheckInputInfo()
		{
			if (string.IsNullOrEmpty(context.ItemTitle))
			{
				EditorUtility.DisplayDialog(Language.Get(TextID.error), Language.Get(TextID.titleEmpty), Language.Get(TextID.ok));
				return false;
			}
			if (Encoding.Default.GetBytes(context.ItemTitle).Length >= Constants.k_cchPublishedDocumentTitleMax)
			{
				EditorUtility.DisplayDialog(Language.Get(TextID.error), Language.Get(TextID.titleToLong), Language.Get(TextID.ok));
				return false;
			}
			if (Encoding.Default.GetBytes(context.ItemDesc).Length >= Constants.k_cchPublishedDocumentDescriptionMax)
			{
				EditorUtility.DisplayDialog(Language.Get(TextID.error), Language.Get(TextID.descToLong), Language.Get(TextID.ok));
				return false;
			}
			string rawContentPath = modelBuilder.AssetbundlePath;
			if (string.IsNullOrEmpty(rawContentPath))
			{
				EditorUtility.DisplayDialog(Language.Get(TextID.error), Language.Get(TextID.contentEmpty), Language.Get(TextID.ok));
				return false;
			}
			if (!Directory.Exists(rawContentPath))
			{
				EditorUtility.DisplayDialog(Language.Get(TextID.error), Language.Get(TextID.contentMissing), Language.Get(TextID.ok));
				return false;
			}
			if (Directory.GetFiles(rawContentPath).Length < 3)
			{
				EditorUtility.DisplayDialog(Language.Get(TextID.error), Language.Get(TextID.contentInvalid), Language.Get(TextID.ok));
				return false;
			}
			if (!File.Exists(context.PreviewPath))
			{
				EditorUtility.DisplayDialog(Language.Get(TextID.error), Language.Get(TextID.previewMissing), Language.Get(TextID.ok));
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
			return context.ContentPath + "/" + FormatPreviewFileName(context.PreviewPath);
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

			context.ContentPath = newContentPath;

			ReplaceContentWithWorkshopID(workshopID, oldFolderName);
		}

		protected void ReplaceContentWithWorkshopID(ulong workshopID, string oldFolderName)
		{
			string newFolderName = workshopID.ToString();

			string modelDataPath = context.ContentPath + "/" + AppConst.subModelDataName;
			string text = File.ReadAllText(modelDataPath);
			ModelData data = JsonUtility.FromJson<ModelData>(text);

			// root content data
			int endIdx = context.ContentPath.LastIndexOf("/");
			string rootPath = context.ContentPath.Substring(0, endIdx);
			string rootModelPath = rootPath + "/" + AppConst.subModelDataName;
			ModelAssetBuilder.SaveModelDataToFile(
					rootModelPath,
					workshopID.ToString(),
					context.ItemTitle,
					context.ItemDesc,
					FormatPreviewFileName(context.PreviewPath),
					data.bundle,
					data.asset
					);

			// content model data
			ModelAssetBuilder.SaveModelDataToFile(
					modelDataPath,
					workshopID.ToString(),
					context.ItemTitle,
					context.ItemDesc,
					FormatPreviewFileName(context.PreviewPath),
					data.bundle,
					data.asset
					);

			// sbm
			string sbmPath = context.ContentPath + "/" + AppConst.assetbundleName + ".sbm";
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
				int endIdx = context.ContentPath.LastIndexOf("/");
				string rootPath = context.ContentPath.Substring(0, endIdx);
				string targetRoot = rootPath + "/" + FormatPreviewFileName(previewPath);
				if (previewPath != targetRoot)
					File.Copy(previewPath, rootPath + "/" + FormatPreviewFileName(previewPath), true);
				File.Copy(previewPath, context.ContentPath + "/" + FormatPreviewFileName(previewPath), true);
				File.Copy(previewPath, Application.dataPath + "/Editor/Resources/" + FormatPreviewFileName(previewPath), true);
				AssetDatabase.Refresh();
			}
		}

		protected void ClearTempDirectory()
		{
			string path2 = Application.dataPath + "/Editor/Resources/" + FormatPreviewFileName(context.PreviewPath);
			if (File.Exists(path2))
				File.Delete(path2);

			AssetDatabase.Refresh();
		}

	}
}