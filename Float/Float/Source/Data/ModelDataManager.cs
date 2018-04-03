
using System;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Threading.Tasks;
using Steamworks;

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

	public class ModelDataManager : IManager
	{
		private List<ModelData> modelList = new List<ModelData>();

		public List<ModelData> ModelList
		{
			get { return modelList; }
		}

		private object fileRWLock = new object();

		public override void Init()
		{
			FloatApp.MsgSystem.Register(AppConst.MSG_ITEM_DOWNLOADED, OnItemDownloaded);
			FloatApp.MsgSystem.Register(AppConst.MSG_ITEM_INSTALLED, OnItemInstalled);
			FloatApp.MsgSystem.Register(AppConst.MSG_WORKSHOP_PATH_FOUND, OnWorkshopPathFound);
		}

		private void OnItemDownloaded(object[] args)
		{
			
		}

		private void OnItemInstalled(object[] args)
		{
			RebuildModelListAsync();
		}

		private void OnWorkshopPathFound(object[] args)
		{
			Log.Info("Workshop Path Found");
			ValidateAllModelDataAsync();
		}

		private async void RebuildModelListAsync()
		{
			await Task.Run(() => { RebuildModelList(); });

			await Task.Run(() => { ReloadModelList(modelList); });

			FloatApp.MsgSystem.Push(AppConst.MSG_MODEL_LIST_UPDATE);
		}

		private void ReloadModelList(List<ModelData> models)
		{
			Log.Info("Begin ReloadModelList");
			lock (fileRWLock)
			{
				string jsonStr = File.ReadAllText(AppConst.modelListPath);
				if (string.IsNullOrEmpty(jsonStr))
				{
					Log.Error("Cannot load model data !");
					return;
				}

				ModelDataArray modelArray = JsonConvert.DeserializeObject<ModelDataArray>(jsonStr);
				models.Clear();
				for (int i = 0; i < modelArray.models.Length; ++i)
				{
					models.Add(modelArray.models[i]);
				}
			}
			Log.Info("End ReloadModelList");
		}

		private void RebuildModelList()
		{
			try
			{
				Log.Info("Begin RebuildModelList");
				lock (fileRWLock)
				{
					string modelPath = SteamManager.WorkshopInstallPath;
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
				Log.Info("End RebuildModelList");
			}
			catch (Exception e)
			{
				Log.Error(e.ToString());
			}
		}

		public async void ValidateAllModelDataAsync()
		{
			bool ret = await Task.Run(() => { return ValidateAllModelData(); });
			if (ret)
			{
				FloatApp.MsgSystem.Push(AppConst.MSG_MODEL_LIST_READY);
			}
			else
			{
				Log.Error("Invalid model data");
				RebuildModelListAsync();
			}
		}

		private bool ValidateAllModelData()
		{
			if (!File.Exists(AppConst.modelListPath))
				return false;

			return true;
		}

		/*private Task<uint> RebuildModelList()
		{
			string modelPath = AppConst.modelPath;
			if (!Directory.Exists(modelPath))
			{
				Log.Error("RefreshModelList: Directory Not Exists: " + modelPath);
				return null;
			}

			string[] directories = Directory.GetDirectories(modelPath);
			string[] files = Directory.GetFiles(modelPath);
			if (directories.Length + files.Length == 0)
				return null;

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

			return null;
		}*/


	}
}