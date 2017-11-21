
using System;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using System.Collections.Generic;


public class ModelData
{
	public long id;
	public string name;
	public string bundleName;
	public string assetName;
}

public class ModelDataArray
{
	public ModelData[] models;
}

public class DataManager : IManager
{
	public static DataManager Instance { private set; get; }

	Dictionary<long, ModelData> modelDic = new Dictionary<long, ModelData>();


	public override void Init()
	{
		Instance = this;
		ReloadModelData();
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
			string dirName = subdir.Substring(subdir.LastIndexOf("/") + 1);
			string infoPath = subdir + "/" + dirName + ".sbm";
			if (File.Exists(infoPath))
			{
				string text = File.ReadAllText(infoPath);
				ModelData data = JsonConvert.DeserializeObject<ModelData>(text);
				models.Add(data);
			}
			else
				Log.Error("RefreshModelList: file not exists: " + infoPath);
		}

		ModelDataArray modelArray = new ModelDataArray();
		modelArray.models = models.ToArray();
		string jsonStr = JsonConvert.SerializeObject(modelArray);
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

		ModelDataArray modelArray = JsonConvert.DeserializeObject<ModelDataArray>(jsonStr);
		modelDic.Clear();
		for (int i = 0; i < modelArray.models.Length; ++i)
			modelDic.Add(modelArray.models[i].id, modelArray.models[i]);
	}

	public ModelData GetModelData(long id)
	{
		ModelData data;
		modelDic.TryGetValue(id, out data);
		return data;
	}

	public static void RebuildModelList(string modelPath)
	{
		try
		{
			string[] subDirs = Directory.GetDirectories(modelPath);
			ModelDataArray modelArray = new ModelDataArray();
			modelArray.models = new ModelData[subDirs.Length];
			for (int i = 0; i < subDirs.Length; ++i)
			{
				string txt = File.ReadAllText(subDirs[i] + "/" + AppDefine.subModelDataName);
				modelArray.models[i] = JsonConvert.DeserializeObject<ModelData>(txt);
			}
			string arrayStr = JsonConvert.SerializeObject(modelArray, Formatting.Indented);
			File.WriteAllText(modelPath + "/" + AppDefine.modelListName, arrayStr, Encoding.UTF8);
		}
		catch (Exception e)
		{
			Log.Error(e.ToString());
		}
	}
}