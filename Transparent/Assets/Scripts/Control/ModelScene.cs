using System;
using System.IO;
using UnityEngine;


[Serializable]
public class ModelData
{
	public int id;
	public string name;
}

[Serializable]
public class ModelDataArray
{
	public ModelData[] models;
}

public class ModelScene : MonoBehaviour
{
	public static ModelScene Instance { get; private set; }

	private Transform root;

	ModelDataArray models;


	void Awake()
	{
		Instance = this;
		root = transform;
		ReloadAll();
	}

	public void LoadModel()
	{

	}

	public void RemoveAll()
	{
		for (int i = 0; i < root.childCount; ++i)
		{
			GameObject.DestroyImmediate(root.GetChild(i).gameObject);
		}
	}

	public void ReloadAll()
	{
		string path = AppDefine.PersistentDataPath + "models.json";
		string jsonStr = File.ReadAllText(path);
		if (string.IsNullOrEmpty(jsonStr))
		{
			Log.Error("Cannot load model data !");
			return;
		}

		models = JsonUtility.FromJson<ModelDataArray>(jsonStr);
	}

	
}
