using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using Newtonsoft.Json;


namespace Float
{
	public class BuildModelWindow : EditorWindow
	{

		[MenuItem(AppConst.AppName + "/BuildModel")]
		public static void ShowExcelWindow()
		{
			Rect wr = new Rect(100, 100, 640, 480);
			var window = (BuildModelWindow)EditorWindow.GetWindowWithRect(typeof(BuildModelWindow), wr, true, "Build Model");
			window.Show();
		}

		string modelPath = "";
		string outputPath = "Output/" + AppConst.AppName;
		string prefabPath = "";
		ModelPrefab prefab;
		string titleDesc = "No title";
		string author = "No author";
		string preview = "";


		void OnEnable()
		{
			AppUtils.SetRandomSeed(DateTime.Now.Millisecond * DateTime.Now.Second);
		}

		void OnGUI()
		{
			float spaceSize = 10f;
			float leftSpace = 10;
			float titleLen = 70;
			float textLen = 450;
			float buttonLen1 = 100;
			float buttonLen2 = 50;
			float buttonHeight = 40;

			GUILayout.Label("Build your model", EditorStyles.helpBox);
			GUILayout.Space(spaceSize);

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
			GUILayout.Space(spaceSize);
			GUILayout.Label("Title", EditorStyles.label, GUILayout.Width(titleLen));
			titleDesc = GUILayout.TextField(titleDesc, GUILayout.Width(textLen));
			GUILayout.EndHorizontal();
			GUILayout.Space(leftSpace);

			GUILayout.BeginHorizontal();
			GUILayout.Space(spaceSize);
			GUILayout.Label("Author", EditorStyles.label, GUILayout.Width(titleLen));
			author = GUILayout.TextField(author, GUILayout.Width(textLen));
			GUILayout.EndHorizontal();
			GUILayout.Space(leftSpace);

			GUILayout.BeginHorizontal();
			GUILayout.Space(spaceSize);
			GUILayout.Label("Preview", EditorStyles.label, GUILayout.Width(titleLen));
			preview = GUILayout.TextField(preview, GUILayout.Width(textLen));
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
				RebuildMyManifest(outputPath, outputPath + "/" + AppConst.manifestName);
				RebuildModelList(outputPath, outputPath + "/" + AppConst.manifestName);
				EditorUtility.DisplayDialog("Floating", "Refresh success!", "OK");
			}
			GUILayout.EndHorizontal();
			GUILayout.Space(spaceSize);
		}

		public void BuildSingleAB(string sourcePath, string outputPath, string assetName)
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
				modelData.workshopId = uid.ToString();
				//modelData.name = abName;
				modelData.bundle = abName;
				modelData.asset = assetName.Substring(assetName.IndexOf("Assets/"));
				modelData.title = titleDesc;
				//modelData.author = author;
				modelData.preview = preview;
				jsonStr = JsonConvert.SerializeObject(modelData, Formatting.Indented);
				string subfolderPath = outputPath + "/" + subfolderName;
				File.WriteAllText(subfolderPath + "/" + AppConst.subModelDataName, jsonStr, Encoding.UTF8);

				AssetDatabase.Refresh();
				EditorUtility.DisplayDialog("Floating", "Done! Output: " + subfolderPath, "OK");
			}
			catch (Exception e)
			{
				AssetDatabase.Refresh();
				Log.Error(e.ToString());
			}
		}

		public static void RebuildMyManifest(string rootPath, string modelManifestPath)
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

		public static void RebuildModelList(string modelPath, string modelManifestPath)
		{
			try
			{
				string[] subDirs = Directory.GetDirectories(modelPath);
				ModelDataArray modelArray = new ModelDataArray();
				List<ModelData> dataList = new List<ModelData>();
				for (int i = 0; i < subDirs.Length; ++i)
				{
					string filePath = subDirs[i] + "/" + AppConst.subModelDataName;
					if (File.Exists(filePath))
					{
						string txt = File.ReadAllText(filePath);
						var data = JsonConvert.DeserializeObject<ModelData>(txt);
						dataList.Add(data);
					}
				}
				modelArray.models = dataList.ToArray();
				string arrayStr = JsonConvert.SerializeObject(modelArray, Formatting.Indented);
				File.WriteAllText(modelPath + "/" + AppConst.modelListName, arrayStr, Encoding.UTF8);

				// copy to streamingsassets
				CopyAssetBundles(modelPath, AppConst.PersistentDataPath);

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

	}

}