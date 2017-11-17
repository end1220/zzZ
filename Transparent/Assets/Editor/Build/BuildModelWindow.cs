using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Collections.Generic;


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
		modelPath = Application.dataPath;
		outputPath = Application.streamingAssetsPath;
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
		if (GUILayout.Button("Open", GUILayout.Width(buttonLen2)))
		{
			modelPath = EditorUtility.OpenFolderPanel("Select Model Folder", String.Empty, "");
		}
		GUILayout.EndHorizontal();
		GUILayout.Space(spaceSize);

		GUILayout.BeginHorizontal();
		GUILayout.Space(spaceSize);
		GUILayout.Label("Output Path", EditorStyles.label, GUILayout.Width(titleLen));
		outputPath = GUILayout.TextField(outputPath, GUILayout.Width(textLen));
		if (GUILayout.Button("Open", GUILayout.Width(buttonLen2)))
		{
			outputPath = EditorUtility.OpenFolderPanel("Select Output Folder", String.Empty, "");
		}
		GUILayout.EndHorizontal();
		GUILayout.Space(leftSpace);

		GUILayout.BeginHorizontal();
		GUILayout.Space(leftSpace);
		if (GUILayout.Button("build", GUILayout.Width(buttonLen1), GUILayout.Height(buttonHeight)))
		{
			Build(modelPath, outputPath);
		}
		GUILayout.EndHorizontal();
		GUILayout.Space(spaceSize);
	}
	
	static void UpdateProgress(int progress, int progressMax, string desc)
	{
		string title = "Processing...[" + progress + " / " + progressMax + "]";
		float value = (float)progress / (float)progressMax;
		EditorUtility.DisplayProgressBar(title, desc, value);
	}

	public static void Build(string sourcePath, string outputPath)
	{
		string category = "character";
		CreateNewOutputPath(outputPath, true);
		AssignNameRecur(sourcePath, "AB_Name_aaa666");
		List<AssetBundleBuild> mapbuild = CollectBuildListRecur(sourcePath, category);
		BuildPipeline.BuildAssetBundles(outputPath, mapbuild.ToArray(), BuildAssetBundleOptions.None, EditorUserBuildSettings.activeBuildTarget);
		//CopyAssetBundles(outputPath + "/" + category, copyTargetPath + "/" + category);
		EditorUtility.DisplayDialog("title", "Build success!", "good");
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

	private static void AssignNameRecur(string source, string ABName)
	{
		string[] files = Directory.GetFiles(source);
		for (int i = 0; i < files.Length; ++i)
		{
			if (!files[i].EndsWith(".meta") && !files[i].EndsWith(".cs"))
			{
				AssignName(files[i], ABName);
			}
		}

		string[] folders = Directory.GetDirectories(source);
		for (int i = 0; i < folders.Length; ++i)
		{
			AssignNameRecur(folders[i], ABName);
		}
	}


	private static void AssignName(string assetPath, string ABName)
	{
		AssetImporter assetImporter = AssetImporter.GetAtPath(assetPath);
		if (assetImporter != null)
			assetImporter.assetBundleName = ABName;
	}

	private static List<AssetBundleBuild> CollectBuildListRecur(string sourceDir, string category)
	{
		List<AssetBundleBuild> builds = new List<AssetBundleBuild>();
		List<string> assetNames = new List<string>();

		// root dir
		string[] files = Directory.GetFiles(sourceDir);
		for (int i = 0; i < files.Length; ++i)
			if (!files[i].EndsWith(".meta") && !files[i].EndsWith(".cs"))
				assetNames.Add(files[i].Substring(files[i].IndexOf("Assets")).Replace("\\", "/"));
		AssetBundleBuild abb0 = new AssetBundleBuild();
		abb0.assetBundleName = category + "/" + category;
		abb0.assetNames = assetNames.ToArray();
		builds.Add(abb0);

		// sub dir
		string[] folders = Directory.GetDirectories(sourceDir);
		for (int i = 0; i < folders.Length; ++i)
		{
			if (folders[i].Contains("."))
				continue;
			folders[i] = folders[i].Replace("\\", "/");
			string abName = category + "/" + folders[i].Substring(folders[i].LastIndexOf("/") + 1);

			assetNames.Clear();
			CollectAssetNamesRecur(folders[i], assetNames);
			AssetBundleBuild abb = new AssetBundleBuild();
			abb.assetBundleName = abName;
			abb.assetNames = assetNames.ToArray();
			builds.Add(abb);
		}

		return builds;
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