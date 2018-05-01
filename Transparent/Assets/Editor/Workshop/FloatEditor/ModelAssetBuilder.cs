using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using Newtonsoft.Json;
using Steamworks;


namespace Float
{
	public class ModelAssetBuilder
	{
		public string AssetbundlePath { get { return context.AssetbundlePath; } }

		string layoutSavePath = Environment.CurrentDirectory.Replace("\\", "/") + "/ProjectSettings/ModelBuilder";

		[JsonObject(MemberSerialization.OptIn)]
		private class Context
		{
			[JsonProperty]
			private string modelPath = "";

			[JsonProperty]
			private string outputPath = Environment.CurrentDirectory.Replace("\\", "/") + "/projects/ModelOutput/";

			[JsonProperty]
			private string prefabPath = "";

			[JsonProperty]
			private string assetbundlePath = Environment.CurrentDirectory.Replace("\\", "/") + "/projects/ModelOutput/2063520985";

			[JsonIgnore]
			public bool dirty;

			[JsonIgnore]
			public string ModelPath
			{
				get
				{
					return modelPath;
				}
				set
				{
					if (modelPath != value)
						dirty = true;
					modelPath = value;
				}
			}

			[JsonIgnore]
			public string OutputPath
			{
				get
				{
					return outputPath;
				}
				set
				{
					if (outputPath != value)
						dirty = true;
					outputPath = value;
				}
			}

			[JsonIgnore]
			public string PrefabPath
			{
				get
				{
					return prefabPath;
				}
				set
				{
					if (prefabPath != value)
						dirty = true;
					prefabPath = value;
				}
			}

			[JsonIgnore]
			public string AssetbundlePath
			{
				get
				{
					return assetbundlePath;
				}
				set
				{
					if (assetbundlePath != value)
						dirty = true;
					assetbundlePath = value;
				}
			}
		}

		Context context = new Context();

		ModelPrefab prefab;

		public ModelAssetBuilder()
		{
			if (File.Exists(layoutSavePath))
			{
				context = JsonConvert.DeserializeObject<Context>(File.ReadAllText(layoutSavePath));
			}
		}

		public void OnDestroy()
		{
			if (File.Exists(layoutSavePath))
			{
				File.Delete(layoutSavePath);
			}
		}

		public void SaveContext()
		{
			if (context.dirty)
			{
				string str = JsonConvert.SerializeObject(context);
				File.WriteAllText(layoutSavePath, str);
				context.dirty = false;
			}
		}

		public void OnGUI()
		{
			GUILayout.BeginHorizontal();
			GUILayout.Space(FloatGUIStyle.leftSpace);
			GUILayout.Label(Language.Get(TextID.Output), FloatGUIStyle.boldLabel, GUILayout.Width(FloatGUIStyle.titleLen));
			GUILayout.Label(context.OutputPath, FloatGUIStyle.label, GUILayout.Width(FloatGUIStyle.textLen));
			GUILayout.EndHorizontal();
			GUILayout.Space(FloatGUIStyle.spaceSize);

			GUILayout.BeginHorizontal();
			GUILayout.Space(FloatGUIStyle.leftSpace);
			GUILayout.Label(Language.Get(TextID.model), FloatGUIStyle.boldLabel, GUILayout.Width(FloatGUIStyle.titleLen));
			context.ModelPath = GUILayout.TextField(context.ModelPath, FloatGUIStyle.textFieldPath, GUILayout.Width(FloatGUIStyle.textLen));
			if (GUILayout.Button(Language.Get(TextID.select), GUILayout.Width(FloatGUIStyle.buttonLen2)))
				context.ModelPath = EditorUtility.OpenFolderPanel(Language.Get(TextID.selectModelFolder), string.Empty, "");
			GUILayout.EndHorizontal();
			GUILayout.Space(FloatGUIStyle.spaceSize);

			GUILayout.BeginHorizontal();
			GUILayout.Space(FloatGUIStyle.leftSpace);
			GUILayout.Label(Language.Get(TextID.prefab), FloatGUIStyle.boldLabel, GUILayout.Width(FloatGUIStyle.titleLen));
			prefab = EditorGUILayout.ObjectField(prefab, typeof(ModelPrefab), false, GUILayout.Width(FloatGUIStyle.textLen)) as ModelPrefab;
			if (prefab != null)
				context.PrefabPath = AssetDatabase.GetAssetPath(prefab.gameObject);
			GUILayout.EndHorizontal();
			GUILayout.Space(FloatGUIStyle.spaceSize);

			GUILayout.BeginHorizontal();
			GUILayout.Space(FloatGUIStyle.leftSpace);
			if (GUILayout.Button(Language.Get(TextID.build), FloatGUIStyle.button, GUILayout.Width(FloatGUIStyle.buttonLen1), GUILayout.Height(FloatGUIStyle.buttonHeight)))
			{
				if (prefab != null)
				{
					context.PrefabPath = AssetDatabase.GetAssetPath(prefab.gameObject);
					context.AssetbundlePath = BuildSingleAB(context.ModelPath, context.OutputPath, context.PrefabPath);
				}
			}
			if (GUILayout.Button(Language.Get(TextID.refresh), FloatGUIStyle.button, GUILayout.Width(FloatGUIStyle.buttonLen1), GUILayout.Height(FloatGUIStyle.buttonHeight)))
			{
				RebuildMyManifest(context.OutputPath, context.OutputPath + "/" + AppConst.manifestName);
				RebuildModelList(context.OutputPath, context.OutputPath + "/" + AppConst.manifestName);
				EditorUtility.DisplayDialog("Floating", "Refresh success!", "OK");
			}
			GUILayout.EndHorizontal();
			GUILayout.Space(FloatGUIStyle.spaceSize);

			GUILayout.Label(context.AssetbundlePath, FloatGUIStyle.label, GUILayout.Width(FloatGUIStyle.textLen));
		}

		private string BuildSingleAB(string sourcePath, string outputPath, string assetName)
		{
			try
			{
				int uid = AppUtils.RandomInt(0, int.MaxValue);
				string subfolderName = uid.ToString();
				string abName = subfolderName + "/" + AppConst.assetbundleName;
				string assetBundlePath = outputPath + "/" + subfolderName;
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

				SaveModelDataToFile(
					assetBundlePath + "/" + AppConst.subModelDataName,
					uid.ToString(),
					"no title",
					"no desc",
					"no preview",
					abName,
					assetName.Substring(assetName.IndexOf("Assets/"))
					);

				AssetDatabase.Refresh();
				EditorUtility.DisplayDialog("Float", "Done!", "OK");
				return assetBundlePath;
			}
			catch (Exception e)
			{
				AssetDatabase.Refresh();
				Log.Error(e.ToString());
			}
			return "";
		}

		/*public PublishedVisibility visibility;
		public CategoryType category = CategoryType.Unspecified;
		public Genre genre = Genre.Unspecified;
		public Rating rating = Rating.Unspecified;*/
		public static void SaveModelDataToFile(string path, string id, string title, string desc, string preview, string bundle, string asset,
			PublishedVisibility visibility = PublishedVisibility.Unspecified, 
			CategoryType category = CategoryType.Unspecified,
			Genre genre = Genre.Unspecified, 
			Rating rating = Rating.Unspecified)
		{
			ModelData modelData = new ModelData();
			modelData.workshopId = id;
			modelData.title = title;
			modelData.description = desc;
			modelData.preview = preview;
			modelData.bundle = bundle;
			modelData.asset = asset;
			modelData.visibility = visibility;
			modelData.category = category;
			modelData.genre = genre;
			modelData.rating = rating;
			string jsonStr = JsonConvert.SerializeObject(modelData, Formatting.Indented);
			File.WriteAllText(path, jsonStr, Encoding.UTF8);
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