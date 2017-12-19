
using System;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;



public class BuildAssetBundles
{

	static string tmpOutputPath = "AssetBundles";
	static string copyTargetPath = "Assets/StreamingAssets";

	static string LuaSrcPath = Application.dataPath + "/Lua";
	static string luaTempDir = Application.dataPath + "/LuaTemp";


	enum AssignType
	{
		Whole,
		ChildFolder,
		Mannully
	}

	struct BuildConfig
	{
		public AssignType assignType;
		public string categoryName;
		public string path;
		public BuildConfig(AssignType assignType, string categoryName, string path)
		{
			this.assignType = assignType;
			this.categoryName = categoryName;
			this.path = path;
		}
	}

	static BuildConfig[] configs = new BuildConfig[]
	{
		new BuildConfig(AssignType.ChildFolder, "model", "Assets/Models"),
		new BuildConfig(AssignType.ChildFolder, "lua", "Assets/LuaTemp"),
	};


	/// <summary>
	/// 打全部
	/// </summary>
	public static void Build_All()
	{
		try
		{
			string outputPath = CreateNewOutputPath(true);

			MakeLuaTempDir();
			RefreshAssetBundleNames();
			BuildPipeline.BuildAssetBundles(outputPath, BuildAssetBundleOptions.None, EditorUserBuildSettings.activeBuildTarget);
			BuildFileIndex(outputPath);
			DeleteLuaTempDir();
			Debug.Log("Bulid done.");

			CopyAssetBundles(outputPath, copyTargetPath);
			Debug.Log("Copy done.");

			AssetDatabase.Refresh();
		}
		catch (Exception e)
		{
			Debug.LogError(e.ToString());
			EditorUtility.ClearProgressBar();
		}
	}

	/*public static void Build_Character()
	{
		string category = "character";
		string subDir = "/Characters";
		string outputPath = CreateNewOutputPath(false);
		if (Directory.Exists(outputPath + "/" + category))
			Directory.Delete(outputPath + "/" + category, true);
		RefreshAssetBundleNames();
		List<AssetBundleBuild> mapbuild = CollectBuildListRecur(Application.dataPath + subDir, category);
		BuildPipeline.BuildAssetBundles(outputPath, mapbuild.ToArray(), BuildAssetBundleOptions.None, EditorUserBuildSettings.activeBuildTarget);
		CopyAssetBundles(outputPath + "/" + category, copyTargetPath + "/" + category);
		Debug.Log("Build character done.");
	}*/

	public static void BuildSubmanifests()
	{
		string outputPath = Path.Combine(tmpOutputPath, UtilsForEdit.GetPlatformName());
		outputPath = Path.Combine(outputPath, AppDefine.AppName);
		BuildSubmanifestRecur(outputPath);
		EditorUtility.DisplayDialog("Floating", "Build submanifests Done!", "OK");
	}

	private static void BuildSubmanifestRecur(string path)
	{
		path.Replace("\\", "/");
		string[] files = Directory.GetFiles(path);
		for (int i = 0; i < files.Length; ++i)
		{
			string fileName = files[i];
			if (fileName.EndsWith(".manifest"))
			{
				int beginIndex = fileName.LastIndexOf("\\");
				int endIndex = fileName.LastIndexOf(".manifest");
				string abName = fileName.Substring(beginIndex + 1, endIndex - beginIndex - 1);
				string subManifestName = abName + ".sbm";
				SubAssetBundleManifest subManifest = new SubAssetBundleManifest(subManifestName, abName);
				subManifest.SetUnityManifest(path + "/" + abName + ".manifest");
				string jsonStr = JsonConvert.SerializeObject(subManifest, Formatting.Indented);
				File.WriteAllText(path + "/" + subManifestName, jsonStr, Encoding.UTF8);
			}
		}
		string[] dirs = Directory.GetDirectories(path);
		for (int i = 0; i < dirs.Length; ++i)
		{
			string dir = dirs[i];
			BuildSubmanifestRecur(dir);
		}
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

	private static string CreateNewOutputPath(bool deleteOld)
	{
		string outputPath = Path.Combine(tmpOutputPath, UtilsForEdit.GetPlatformName());
		outputPath = Path.Combine(outputPath, AppDefine.AppName);
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

	public static void RefreshAssetBundleNames()
	{
		try
		{
			for (int i = 0; i < configs.Length; ++i)
			{
				var cfg = configs[i];
				UpdateProgress(i, configs.Length, "Asign Names");
				if (Directory.Exists(cfg.path))
					AssignAssetBundleNames(cfg.path, cfg);
			}

			EditorUtility.ClearProgressBar();
		}
		catch (Exception e)
		{
			Debug.LogError(e.ToString());
			EditorUtility.ClearProgressBar();
		}
	}

	private static void AssignAssetBundleNames(string source, BuildConfig cfg)
	{
		source = source.Replace("\\", "/");

		if (cfg.assignType == AssignType.Whole)
		{
			AssignNameRecur(source, cfg.categoryName + "/" + cfg.categoryName);
		}
		else if (cfg.assignType == AssignType.ChildFolder)
		{
			string[] folders = Directory.GetDirectories(source);
			for (int i = 0; i < folders.Length; ++i)
			{
				folders[i] = folders[i].Replace("\\", "/");
				string ABName = folders[i].Substring(folders[i].LastIndexOf("/") + 1);
				AssignNameRecur(folders[i], cfg.categoryName + "/" + ABName);
			}
			string[] files = Directory.GetFiles(source);
			for (int i = 0; i < files.Length; ++i)
			{
				if (!files[i].EndsWith(".meta") && !files[i].EndsWith(".cs"))
				{
					AssignName(files[i], cfg.categoryName + "/" + cfg.categoryName);
				}
			}
		}
		else
		{
			// do nothing.
		}

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


	private static string GetFileName(string assetPath)
	{
		assetPath = assetPath.Replace("\\", "/");
		int startIndex = assetPath.LastIndexOf("/") + 1;
		int len = assetPath.LastIndexOf(".") - startIndex + 1;
		string assetName = assetPath.Substring(startIndex, len);
		return assetName;
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


	static void CopyFiles(string fromDirectory, string toDirectory)
	{
		if (!Directory.Exists(fromDirectory))
		{
			Log.Error("Directory Not Exists. " + fromDirectory);
			return;
		}
		if (Directory.Exists(toDirectory))
			Directory.Delete(toDirectory, true);

		Directory.CreateDirectory(toDirectory);

		string[] directories = Directory.GetDirectories(fromDirectory);
		if (directories.Length > 0)
		{
			foreach (string d in directories)
			{
				CopyFiles(d, toDirectory + d.Substring(d.LastIndexOf("\\")));
			}
		}

		string[] files = Directory.GetFiles(fromDirectory);
		if (files.Length > 0)
		{
			foreach (string s in files)
			{
				File.Copy(s, toDirectory + s.Substring(s.LastIndexOf("\\")), true);
			}
		}
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


	static void UpdateProgress(int progress, int progressMax, string desc)
	{
		string title = "[" + progress + " / " + progressMax + "]";
		float value = (float)progress / (float)progressMax;
		EditorUtility.DisplayProgressBar(title, desc, value);
	}


	#region 文件列表

	static List<string> paths = new List<string>();
	static List<string> files = new List<string>();
	static void BuildFileIndex(string resPath)
	{
		resPath = resPath.Replace("\\", "/");
		if (!resPath.EndsWith("/"))
			resPath = resPath + "/";
		string newFilePath = resPath + "/files.txt";
		if (File.Exists(newFilePath))
		{
			File.Delete(newFilePath);
		}

		paths.Clear();
		files.Clear();
		Recursive(resPath);

		FileStream fs = new FileStream(newFilePath, FileMode.CreateNew);
		StreamWriter sw = new StreamWriter(fs);
		foreach (string file in files)
		{
			if (file.EndsWith(".meta") || file.Contains(".DS_Store"))
				continue;

			string md5 = AppUtils.md5file(file);
			string value = file.Replace(resPath, "");
			sw.WriteLine(value + "|" + md5);
		}
		sw.Close();
		fs.Close();
	}

	static void Recursive(string path)
	{
		string[] names = Directory.GetFiles(path);
		string[] dirs = Directory.GetDirectories(path);
		foreach (string filename in names)
		{
			string ext = Path.GetExtension(filename);
			if (ext != null && ext.Equals(".meta"))
			{
				continue;
			}
			files.Add(filename.Replace('\\', '/'));
		}
		foreach (string dir in dirs)
		{
			paths.Add(dir.Replace('\\', '/'));
			Recursive(dir);
		}
	}

	#endregion


	#region Lua特殊处理

	/// <summary>
	/// 把lua源文件复制一份并加上.bytes后缀，作为被打包的文件
	/// </summary>
	static void MakeLuaTempDir()
	{
		string sourceDir = LuaSrcPath;
		string destDir = luaTempDir;

		if (Directory.Exists(destDir))
			Directory.Delete(destDir, true);

		Directory.CreateDirectory(destDir);

		string[] files = Directory.GetFiles(sourceDir, "*.lua", SearchOption.AllDirectories);
		int len = sourceDir.Length;
		if (sourceDir[len - 1] == '/' || sourceDir[len - 1] == '\\')
			--len;

		foreach (string file in files)
		{
			string str = file.Remove(0, len);
			string dest = destDir + "/" + str;
			dest += ".bytes";
			string dir = Path.GetDirectoryName(dest);
			Directory.CreateDirectory(dir);
			File.Copy(file, dest, true);
		}

		AssetDatabase.Refresh();
	}


	static void DeleteLuaTempDir()
	{
		string destDir = luaTempDir;

		if (Directory.Exists(destDir))
			Directory.Delete(destDir, true);

		AssetDatabase.Refresh();
	}


	#endregion

}


