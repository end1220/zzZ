﻿using UnityEngine;
using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using XLua;


[Serializable]
public class ModelData
{
	public int id;
	public string name;
	public string bundleName;
	public string assetName;
}

[Serializable]
public class ModelDataArray
{
	public ModelData[] models;
}

public class DataManager : MonoBehaviour, IManager
{
	public static DataManager Instance { private set; get; }

	Dictionary<int, ModelData> modelDic = new Dictionary<int, ModelData>();


	public void Init()
	{
		Instance = this;
		ReloadModelData();
	}

	public void Tick()
	{
		
	}

	public void Destroy()
	{
		
	}

	public static void RefreshModelList()
	{
		string modelPath = AppDefine.modelPath;
		if (!Directory.Exists(modelPath))
		{
			Log.Error("RefreshModelList: Directory Not Exists: " + modelPath);
			return;
		}

		string[] directories = Directory.GetDirectories(modelPath);
		string[] files = Directory.GetFiles(modelPath);
		if (directories.Length + files.Length == 0)
			return;

		List<ModelData> models = new List<ModelData>();
		foreach (string subdir in directories)
		{
			string infoPath = subdir + "/info.json";
			if (File.Exists(infoPath))
			{
				string text = File.ReadAllText(infoPath);
				ModelData data = JsonUtility.FromJson<ModelData>(text);
				models.Add(data);
			}
			else
				Log.Error("RefreshModelList: file not exists: " + infoPath);
		}

		ModelDataArray modelArray = new ModelDataArray();
		modelArray.models = models.ToArray();
		string jsonStr = JsonUtility.ToJson(modelArray);
		FileStream fs = File.Open(AppDefine.modelListPath, FileMode.Create);
		StreamWriter sw = new StreamWriter(fs);
		sw.Write(jsonStr);
		sw.Flush();
		fs.Close();
	}

	public void ReloadModelData()
	{
		string jsonStr = File.ReadAllText(AppDefine.modelListPath);
		if (string.IsNullOrEmpty(jsonStr))
		{
			Log.Error("Cannot load model data !");
			return;
		}

		ModelDataArray modelArray = JsonUtility.FromJson<ModelDataArray>(jsonStr);
		modelDic.Clear();
		for (int i = 0; i < modelArray.models.Length; ++i)
			modelDic.Add(modelArray.models[i].id, modelArray.models[i]);
	}

	public ModelData GetModelData(int id)
	{
		ModelData data;
		modelDic.TryGetValue(id, out data);
		return data;
	}
}