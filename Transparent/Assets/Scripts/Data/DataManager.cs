using UnityEngine;
using System;
using System.IO;
using System.Collections.Generic;


namespace Float
{
	public enum CategoryType
	{
		[EnumLanguage(TextID.Unspecified)]
		Unspecified,
		[EnumLanguage(TextID.Scene)]
		Scene,
		[EnumLanguage(TextID.Video)]
		Video,
		[EnumLanguage(TextID.Web)]
		Web,
		[EnumLanguage(TextID.App)]
		App
	}

	public enum Genre
	{
		[EnumLanguage(TextID.Unspecified)]
		Unspecified,
		[EnumLanguage(TextID.Abstract)]
		Abstract,
		[EnumLanguage(TextID.Animal)]
		Animal,
		[EnumLanguage(TextID.Cartoon)]
		Cartoon,
		[EnumLanguage(TextID.Games)]
		Games,
		[EnumLanguage(TextID.Girls)]
		Girls,
		[EnumLanguage(TextID.Guys)]
		Guys,
		[EnumLanguage(TextID.Fantacy)]
		Fantacy,
		[EnumLanguage(TextID.Nature)]
		Nature,
		[EnumLanguage(TextID.Music)]
		Music
	}

	public enum Rating
	{
		[EnumLanguage(TextID.Unspecified)]
		Unspecified,
		[EnumLanguage(TextID.Everybody)]
		Everybody,
		[EnumLanguage(TextID.Suspicious)]
		Suspicious,
		[EnumLanguage(TextID.Mature)]
		Mature
	}

	public enum PublishedVisibility
	{
		[EnumLanguage(TextID.Unspecified)]
		Unspecified,
		[EnumLanguage(TextID.Public)]
		Public,
		[EnumLanguage(TextID.FriendsOnly)]
		FriendsOnly,
		[EnumLanguage(TextID.Private)]
		Private,
	}

	[Serializable]
	public class ModelData
	{
		public int version;
		public string workshopId;
		public string title;
		public string description;
		public string preview;
		public string[] tags;
		public PublishedVisibility visibility;
		public CategoryType category = CategoryType.Unspecified;
		public Genre genre = Genre.Unspecified;
		public Rating rating = Rating.Unspecified;
		public string bundle;
		public string asset;
	}

	[Serializable]
	public class ModelDataArray
	{
		public ModelData[] models;
	}

	public class DataManager : IManager
	{
		public static DataManager Instance { private set; get; }

		Dictionary<string, ModelData> modelDic = new Dictionary<string, ModelData>();


		public override void Init()
		{
			Instance = this;
			ReloadModelData();
		}

		public override void Tick()
		{

		}

		public override void Destroy()
		{

		}

		public void ReloadModelData()
		{
			string jsonStr = File.ReadAllText(AppConst.modelListPath);
			if (string.IsNullOrEmpty(jsonStr))
			{
				Log.Error("Cannot load model data !");
				return;
			}

			ModelDataArray modelArray = JsonUtility.FromJson<ModelDataArray>(jsonStr);
			modelDic.Clear();
			for (int i = 0; i < modelArray.models.Length; ++i)
				modelDic.Add(modelArray.models[i].workshopId, modelArray.models[i]);
		}

		public ModelData GetModelData(string id)
		{
			ModelData data;
			modelDic.TryGetValue(id, out data);
			return data;
		}
	}

}