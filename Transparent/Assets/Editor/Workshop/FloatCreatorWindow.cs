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
	public class FloatCreatorWindow : EditorWindow
	{

		[MenuItem(AppDefine.AppName + "/Float Creator")]
		public static void ShowExcelWindow()
		{
			Rect wr = new Rect(100, 100, 960, 640);
			var window = (FloatCreatorWindow)EditorWindow.GetWindowWithRect(typeof(FloatCreatorWindow), wr, true, "Upload");
			window.Show();
		}

        string modelPath = "";
        string outputPath = "Output/" + AppDefine.AppName;
        string prefabPath = "";
        ModelPrefab prefab;

		string itemTitle = "Test title";
		string itemDesc = "This is the test description.";
		string previewImagePath = "";
		bool agreeWorkshopPolicy = false;

		void Awake()
		{
			InitSteamAPI();
		}

        void OnEnable()
        {
            AppUtils.SetRandomSeed(DateTime.Now.Millisecond * DateTime.Now.Second);
        }

        private void OnDestroy()
		{
			DestroySteamAPI();
		}

		private void Update()
		{
			SteamManager.Instance.Update();
		}

		float spaceSize = 10f;
		float leftSpace = 10;
		float titleLen = 70;
		float textLen = 450;
		float buttonLen1 = 100;
		float buttonLen2 = 50;
		float buttonHeight = 40;

		void OnGUI()
		{
			GUILayout.Label("Build your own model and submit to steam workshop", EditorStyles.helpBox);
			GUILayout.Space(20);

			OnBuildModelGUI();

			if (SteamManager.Instance.Initialized)
			{
				GUILayout.BeginHorizontal();
				GUILayout.Space(spaceSize);
				GUILayout.Label("Title", EditorStyles.label, GUILayout.Width(titleLen));
				itemTitle = GUILayout.TextField(itemTitle, GUILayout.Width(textLen));
				GUILayout.EndHorizontal();
				GUILayout.Space(leftSpace);

				GUILayout.BeginHorizontal();
				GUILayout.Space(spaceSize);
				GUILayout.Label("Description", EditorStyles.label, GUILayout.Width(titleLen));
				itemDesc = GUILayout.TextArea(itemDesc, GUILayout.Width(textLen), GUILayout.Height(60));
				GUILayout.EndHorizontal();
				GUILayout.Space(leftSpace);

				GUILayout.BeginHorizontal();
				GUILayout.Space(spaceSize);
				GUILayout.Label("Preview", EditorStyles.label, GUILayout.Width(titleLen));
				string savedPreviewPath = EditorPrefs.GetString("BMW_PreviewPath");
				previewImagePath = string.IsNullOrEmpty(savedPreviewPath) ? previewImagePath : savedPreviewPath;
				GUILayout.Box(Resources.Load(Path.GetFileNameWithoutExtension(previewImagePath)) as Texture, GUILayout.Width(128), GUILayout.Height(128));
				/*previewFilePath = GUILayout.TextField(previewFilePath, GUILayout.Width(textLen));*/
				if (GUILayout.Button("Select", GUILayout.Width(buttonLen2)))
				{
					previewImagePath = EditorUtility.OpenFilePanel("Select Preview File", String.Empty, "jpg,png");
					if (!string.IsNullOrEmpty(previewImagePath))
						EditorPrefs.SetString("BMW_PreviewPath", previewImagePath);
					if (File.Exists(previewImagePath))
					{
						File.Copy(previewImagePath, Application.dataPath + "/Editor/Resources/" + Path.GetFileName(previewImagePath));
						AssetDatabase.Refresh();
					}
				}
				GUILayout.EndHorizontal();
				GUILayout.Space(leftSpace);

				// steam info begin
				GUILayout.BeginHorizontal();
				//SteamUser.GetSteamID(), SteamUtils.GetAppID()
				GUILayout.Label("SteamID:", EditorStyles.label, GUILayout.Width(titleLen));
				GUILayout.Label(SteamUser.GetSteamID().m_SteamID.ToString(), GUILayout.Width(textLen));
				GUILayout.EndHorizontal();

				GUILayout.BeginHorizontal();
				GUILayout.Label("AppID:", EditorStyles.label, GUILayout.Width(titleLen));
				GUILayout.Label(SteamUtils.GetAppID().ToString(), GUILayout.Width(textLen));
				GUILayout.EndHorizontal();

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

        #region Build Model Asset

		void OnBuildModelGUI()
		{
			GUILayout.BeginHorizontal();
			GUILayout.Space(spaceSize);
			GUILayout.Label("Output Path", EditorStyles.label, GUILayout.Width(titleLen));
			GUILayout.TextField(Environment.CurrentDirectory.Replace("\\", "/") + "/" + outputPath, GUILayout.Width(textLen));
			GUILayout.EndHorizontal();
			GUILayout.Space(leftSpace);

			GUILayout.BeginHorizontal();
			GUILayout.Space(leftSpace);
			GUILayout.Label("Model Path", EditorStyles.label, GUILayout.Width(titleLen));
			string savedModelPath = EditorPrefs.GetString("BMW_ModelPath");
			modelPath = string.IsNullOrEmpty(savedModelPath) ? Application.dataPath : savedModelPath;
			modelPath = GUILayout.TextField(modelPath, GUILayout.Width(textLen));
			if (GUILayout.Button("Select", GUILayout.Width(buttonLen2)))
			{
				modelPath = EditorUtility.OpenFolderPanel("Select Model Folder", String.Empty, "");
				if (!string.IsNullOrEmpty(modelPath))
					EditorPrefs.SetString("BMW_ModelPath", modelPath);
			}
			GUILayout.EndHorizontal();
			GUILayout.Space(spaceSize);

			GUILayout.BeginHorizontal();
			GUILayout.Space(spaceSize);
			GUILayout.Label("Prefab Path", EditorStyles.label, GUILayout.Width(titleLen));
			prefab = EditorGUILayout.ObjectField(prefab, typeof(ModelPrefab), false, GUILayout.Width(textLen)) as ModelPrefab;
			if (prefab != null)
				prefabPath = AssetDatabase.GetAssetPath(prefab.gameObject);
			GUILayout.EndHorizontal();
			GUILayout.Space(leftSpace);

			GUILayout.BeginHorizontal();
			GUILayout.Space(leftSpace);
			if (GUILayout.Button("build", GUILayout.Width(buttonLen1), GUILayout.Height(buttonHeight)))
			{
				if (prefab != null)
				{
					prefabPath = AssetDatabase.GetAssetPath(prefab.gameObject);
					BuildSingleAB(modelPath, outputPath, prefabPath);
				}
			}
			if (GUILayout.Button("refresh", GUILayout.Width(buttonLen1), GUILayout.Height(buttonHeight)))
			{
				RebuildMyManifest(outputPath, outputPath + "/" + AppDefine.manifestName);
				RebuildModelList(outputPath, outputPath + "/" + AppDefine.manifestName);
				EditorUtility.DisplayDialog("Floating", "Refresh success!", "OK");
			}
			GUILayout.EndHorizontal();
			GUILayout.Space(spaceSize);
		}

		private void BuildSingleAB(string sourcePath, string outputPath, string assetName)
		{
			try
			{
				//long uid = AppUtils.GenUniqueGUIDLong();
				int uid = AppUtils.RandomInt(0, int.MaxValue);
				string subfolderName = uid.ToString();
				string abName = subfolderName + "/" + subfolderName;
				CreateNewOutputPath(outputPath, false);
				AssetBundleBuild abb = CollectBuildInfo(sourcePath, abName);
				AssetBundleManifest manifest = BuildPipeline.BuildAssetBundles(outputPath, new AssetBundleBuild[] { abb },
					BuildAssetBundleOptions.None, EditorUserBuildSettings.activeBuildTarget);
				AssetDatabase.Refresh();

				string subManifestName = abName + ".sbm";
				SubAssetBundleManifest subManifest = new SubAssetBundleManifest(subManifestName, abName);
				subManifest.SetUnityManifest(outputPath + "/" + abName + ".manifest");
				string jsonStr = JsonConvert.SerializeObject(subManifest, Formatting.Indented);
				File.WriteAllText(outputPath + "/" + subManifestName, jsonStr, Encoding.UTF8);

				ModelData modelData = new ModelData();
				modelData.id = uid;
				modelData.name = abName;
				modelData.bundleName = abName;
				modelData.assetName = assetName.Substring(assetName.IndexOf("Assets/"));
				modelData.title = itemTitle;
				modelData.author = "default";
				modelData.preview = "default";
				jsonStr = JsonConvert.SerializeObject(modelData, Formatting.Indented);
				string subfolderPath = outputPath + "/" + subfolderName;
				File.WriteAllText(subfolderPath + "/" + AppDefine.subModelDataName, jsonStr, Encoding.UTF8);

				AssetDatabase.Refresh();
				EditorUtility.DisplayDialog("Floating", "Done! Output: " + subfolderPath, "OK");
			}
			catch (Exception e)
			{
				AssetDatabase.Refresh();
				Log.Error(e.ToString());
			}
		}

		private static void RebuildMyManifest(string rootPath, string modelManifestPath)
		{
			MyAssetBundleManifest manifest = new MyAssetBundleManifest();
			rootPath = rootPath.Replace("\\", "/");
			WriteDefaultResRecur(manifest, rootPath);

			string mainJsonStr = JsonConvert.SerializeObject(manifest, Formatting.Indented);
			File.WriteAllText(modelManifestPath, mainJsonStr, Encoding.UTF8);
		}

		private static void WriteDefaultResRecur(MyAssetBundleManifest manifest, string path)
		{
			string[] files = Directory.GetFiles(path);
			for (int i = 0; i < files.Length; ++i)
			{
				string fileName = files[i].Replace("\\", "/");
				if (fileName.EndsWith(".sbm"))
				{
					string jsonStr = File.ReadAllText(fileName);
					SubAssetBundleManifest sub = JsonConvert.DeserializeObject<SubAssetBundleManifest>(jsonStr);
					manifest.AddSubManifest(sub);
				}
			}
			string[] dirs = Directory.GetDirectories(path);
			for (int i = 0; i < dirs.Length; ++i)
			{
				string dir = dirs[i].Replace("\\", "/");
				WriteDefaultResRecur(manifest, dir);
			}
		}

		private static void RebuildModelList(string modelPath, string modelManifestPath)
		{
			try
			{
				string[] subDirs = Directory.GetDirectories(modelPath);
				ModelDataArray modelArray = new ModelDataArray();
				List<ModelData> dataList = new List<ModelData>();
				for (int i = 0; i < subDirs.Length; ++i)
				{
					string filePath = subDirs[i] + "/" + AppDefine.subModelDataName;
					if (File.Exists(filePath))
					{
						string txt = File.ReadAllText(filePath);
						var data = JsonConvert.DeserializeObject<ModelData>(txt);
						dataList.Add(data);
					}
				}
				modelArray.models = dataList.ToArray();
				string arrayStr = JsonConvert.SerializeObject(modelArray, Formatting.Indented);
				File.WriteAllText(modelPath + "/" + AppDefine.modelListName, arrayStr, Encoding.UTF8);

				// copy to streamingsassets
				CopyAssetBundles(modelPath, AppDefine.PersistentDataPath);

				AssetDatabase.Refresh();
			}
			catch (Exception e)
			{
				AssetDatabase.Refresh();
				Log.Error(e.ToString());
			}
		}

		private static string CreateNewOutputPath(string outputPath, bool deleteOld)
		{
			if (Directory.Exists(outputPath))
			{
				if (deleteOld)
				{
					Directory.Delete(outputPath, true);
					Directory.CreateDirectory(outputPath);
				}
			}
			else
			{
				Directory.CreateDirectory(outputPath);
			}
			return outputPath;
		}

		private static AssetBundleBuild CollectBuildInfo(string sourceDir, string ABName)
		{
			AssetBundleBuild abb = new AssetBundleBuild();
			abb.assetBundleName = ABName;
			List<string> assetNames = new List<string>();

			// root dir
			string[] files = Directory.GetFiles(sourceDir);
			for (int i = 0; i < files.Length; ++i)
				if (!files[i].EndsWith(".meta") && !files[i].EndsWith(".cs") && !files[i].EndsWith(".unity"))
					assetNames.Add(files[i].Substring(files[i].IndexOf("Assets")).Replace("\\", "/"));

			// sub dir
			string[] folders = Directory.GetDirectories(sourceDir);
			for (int i = 0; i < folders.Length; ++i)
			{
				if (folders[i].Contains("."))
					continue;
				folders[i] = folders[i].Replace("\\", "/");
				CollectAssetNamesRecur(folders[i], assetNames);
			}

			abb.assetNames = assetNames.ToArray();

			return abb;
		}

		private static void CollectAssetNamesRecur(string source, List<string> assetNames)
		{
			string[] files = Directory.GetFiles(source);
			for (int i = 0; i < files.Length; ++i)
			{
				if (!files[i].EndsWith(".meta") && !files[i].EndsWith(".cs") && !files[i].EndsWith(".unity"))
					assetNames.Add(files[i].Substring(files[i].IndexOf("Assets")).Replace("\\", "/"));
			}

			string[] folders = Directory.GetDirectories(source);
			for (int i = 0; i < folders.Length; ++i)
			{
				CollectAssetNamesRecur(folders[i], assetNames);
			}
		}

		private static void CopyAssetBundles(string sourcePath, string outputPath)
		{
			Directory.CreateDirectory(outputPath);

			var source = Path.Combine(System.Environment.CurrentDirectory, sourcePath);

			var destination = System.IO.Path.Combine(System.Environment.CurrentDirectory, outputPath);
			if (System.IO.Directory.Exists(destination))
				FileUtil.DeleteFileOrDirectory(destination);

			FileUtil.CopyFileOrDirectory(source, destination);
		}

		#endregion

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