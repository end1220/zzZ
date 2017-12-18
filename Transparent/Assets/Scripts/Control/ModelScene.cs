
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
			GameObject prefab = ResourceManager.Instance.LoadAsset<GameObject>(data.bundleName, data.assetName);
			GameObject go = GameObject.Instantiate(prefab);
			Transform model = go.transform;
			model.parent = modelRoot;
			model.localPosition = Vector3.zero;
			model.localScale = Vector3.one * data.scale;
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


	private void OnGUI()
	{
		int count = 0;
		if (GUI.Button(new Rect(10, 40 * count++, 60, 30), "play"))
		{
			var cmd = new Command(1087141166);
			NetworkManager.Instance.SendBytes((ushort)CommandId.PlayThisOne, ProtobufUtil.Serialize(cmd));
		}
		if (GUI.Button(new Rect(10, 40 * count++, 60, 30), "hide"))
		{
			var cmd = new Command(1);
			NetworkManager.Instance.SendBytes((ushort)CommandId.HideWindow, ProtobufUtil.Serialize(cmd));
		}
	}
}
