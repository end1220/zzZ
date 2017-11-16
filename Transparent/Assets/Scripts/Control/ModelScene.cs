
using UnityEngine;



public class ModelScene : MonoBehaviour
{
	public static ModelScene Instance { get; private set; }

	private Transform modelRoot;
	


	void Awake()
	{
		Instance = this;
		modelRoot = transform;
	}

	public void LoadModel(int id)
	{
		ModelData data = DataManager.Instance.GetModelData(id);
		if (data != null)
		{
			GameObject go = ResourceManager.Instance.LoadAsset<GameObject>(data.bundleName, data.assetName);
			Transform model = go.transform;
			model.parent = modelRoot;
			model.localPosition = Vector3.zero;
			model.localScale = Vector3.one;
			model.localRotation = Quaternion.identity;

			SpinCamera.Instance.target = modelRoot;
		}
		else
		{
			Log.Error("ModelScene.LoadModel: cannot find Id: " + id);
		}
	}

	public void RemoveModel()
	{
		for (int i = 0; i < modelRoot.childCount; ++i)
		{
			GameObject.DestroyImmediate(modelRoot.GetChild(i).gameObject);
		}
	}

	
}
