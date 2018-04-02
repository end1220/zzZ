
using System;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Float
{
	public partial class ModelData
	{
		public long id;
		public string name;
		public string bundleName;
		public string assetName;
		public string title;
		public string author;
		public string preview;
	}

	public class ModelDataArray
	{
		public ModelData[] models;
	}

	public class DataManager : IManager
	{
		public static DataManager Instance { private set; get; }

		List<ModelData> modelList = new List<ModelData>();
		Dictionary<long, ModelData> modelDic = new Dictionary<long, ModelData>();

		public List<ModelData> ModelList
		{
			get { return modelList; }
		}


		public override void Init()
		{
			Instance = this;
			ReloadModelData();
		}

		public static void RefreshModelList()
		{
			string modelPath = AppConst.modelPath;
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
			FileStream fs = File.Open(AppConst.modelListPath, FileMode.Create);
			StreamWriter sw = new StreamWriter(fs);
			sw.Write(jsonStr);
			sw.Flush();
			fs.Close();
		}

		public void ReloadModelData()
		{
			string jsonStr = File.ReadAllText(AppConst.modelListPath);
			if (string.IsNullOrEmpty(jsonStr))
			{
				Log.Error("Cannot load model data !");
				return;
			}

			ModelDataArray modelArray = JsonConvert.DeserializeObject<ModelDataArray>(jsonStr);
			modelList.Clear();
			modelDic.Clear();
			for (int i = 0; i < modelArray.models.Length; ++i)
			{
				modelList.Add(modelArray.models[i]);
				modelDic.Add(modelArray.models[i].id, modelArray.models[i]);
			}
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
					string txt = File.ReadAllText(subDirs[i] + "/" + AppConst.subModelDataName);
					modelArray.models[i] = JsonConvert.DeserializeObject<ModelData>(txt);
				}
				string arrayStr = JsonConvert.SerializeObject(modelArray, Formatting.Indented);
				File.WriteAllText(modelPath + "/" + AppConst.modelListName, arrayStr, Encoding.UTF8);
			}
			catch (Exception e)
			{
				Log.Error(e.ToString());
			}
		}
	}
}