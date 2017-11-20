using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using Newtonsoft.Json;


public class BuildModelWindow : EditorWindow
{

	[MenuItem(AppDefine.AppName + "/BuildModel")]
	public static void ShowExcelWindow()
	{
		Rect wr = new Rect(100, 100, 640, 480);
		var window = (BuildModelWindow)EditorWindow.GetWindowWithRect(typeof(BuildModelWindow), wr, true, "Build Model");
		window.Show();
	}

	string modelPath;
	string outputPath;
	void OnEnable()
	{
		modelPath = Application.dataPath + "/Models/Test/";
		outputPath = Application.streamingAssetsPath + "/" + AppDefine.AppName;
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

		GUILayout.Label("Make your model", EditorStyles.helpBox);
		GUILayout.Space(spaceSize);

		GUILayout.BeginHorizontal();
		GUILayout.Space(leftSpace);
		GUILayout.Label("Model Path", EditorStyles.label, GUILayout.Width(titleLen));
		modelPath = GUILayout.TextField(modelPath, GUILayout.Width(textLen));
		if (GUILayout.Button("Select", GUILayout.Width(buttonLen2)))
			modelPath = EditorUtility.OpenFolderPanel("Select Model Folder", String.Empty, "");
		GUILayout.EndHorizontal();
		GUILayout.Space(spaceSize);

		GUILayout.BeginHorizontal();
		GUILayout.Space(spaceSize);
		GUILayout.Label("Output Path", EditorStyles.label, GUILayout.Width(titleLen));
		outputPath = GUILayout.TextField(outputPath, GUILayout.Width(textLen));
		if (GUILayout.Button("Select", GUILayout.Width(buttonLen2)))
			outputPath = EditorUtility.OpenFolderPanel("Select Output Folder", String.Empty, "");
		GUILayout.EndHorizontal();
		GUILayout.Space(leftSpace);

		GUILayout.BeginHorizontal();
		GUILayout.Space(leftSpace);
		if (GUILayout.Button("build", GUILayout.Width(buttonLen1), GUILayout.Height(buttonHeight)))
			BuildSingleAB(modelPath, outputPath);
		if (GUILayout.Button("refresh", GUILayout.Width(buttonLen1), GUILayout.Height(buttonHeight)))
			RebuildModelList(outputPath, outputPath + "/" + AppDefine.manifestName);
		GUILayout.EndHorizontal();
		GUILayout.Space(spaceSize);
	}
	
	static void UpdateProgress(int progress, int progressMax, string desc)
	{
		string title = "Processing...[" + progress + " / " + progressMax + "]";
		float value = (float)progress / (float)progressMax;
		EditorUtility.DisplayProgressBar(title, desc, value);
	}

	public static void BuildSingleAB(string sourcePath, string outputPath)
	{
		try
		{
			string abName = AppUtils.GenUniqueGUIDLong().ToString();
			abName = abName + "/" + abName;
			CreateNewOutputPath(outputPath, true);
			AssetBundleBuild abb = CollectBuildInfo(sourcePath, abName);
			AssetBundleManifest manifest = BuildPipeline.BuildAssetBundles(outputPath, new AssetBundleBuild[] { abb }, 
				BuildAssetBundleOptions.None, EditorUserBuildSettings.activeBuildTarget);
			AssetDatabase.Refresh();

			string subManifestName = abName + ".sbm";
			SubAssetBundleManifest subManifest = new SubAssetBundleManifest(subManifestName, abName);
			subManifest.SetUnityManifest(outputPath + "/" + abName + ".manifest");
			string jsonStr = JsonConvert.SerializeObject(subManifest, Formatting.Indented);
			File.WriteAllText(outputPath + "/" + subManifestName, jsonStr, Encoding.UTF8);

			AssetDatabase.Refresh();
			EditorUtility.DisplayDialog("Floating", "Build success!", "OK");
		}
		catch (Exception e)
		{
			AssetDatabase.Refresh();
			Log.Error(e.ToString());
		}
	}

	public static void RebuildModelList(string modelPath, string modelManifestPath)
	{
		try
		{
			MyAssetBundleManifest manifest = new MyAssetBundleManifest();
			string[] subDirs = Directory.GetDirectories(modelPath);
			for (int i = 0; i < subDirs.Length; ++i)
			{
				string subDir = subDirs[i].Replace('\\', '/');
				string dirName = subDir.Substring(subDir.LastIndexOf("/") + 1);
				string sbmPath = subDir + "/" + dirName + ".sbm";
				string jsonStr = File.ReadAllText(sbmPath);
				SubAssetBundleManifest sub = JsonConvert.DeserializeObject<SubAssetBundleManifest>(jsonStr);
				manifest.AddSubManifest(sub);
			}
			string mainJsonStr = JsonConvert.SerializeObject(manifest, Formatting.Indented);
			File.WriteAllText(modelManifestPath, mainJsonStr, Encoding.UTF8);

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
			if (!files[i].EndsWith(".meta") && !files[i].EndsWith(".cs"))
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
			if (!files[i].EndsWith(".meta") && !files[i].EndsWith(".cs"))
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