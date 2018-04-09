
using UnityEngine;


namespace Float
{

	public class ModelScene : MonoBehaviour
	{
		public static ModelScene Instance { get; private set; }

		private Transform modelRoot;

		public ModelPrefab CurrentModelPrefab { private set; get; }

		public LuaScript CurrentLuaScript { private get; set; }


		void Awake()
		{
			Instance = this;
			modelRoot = transform;
		}

		public void LoadModel(string id)
		{
			ModelData data = DataManager.Instance.GetModelData(id);
			if (data != null)
			{
				GameObject prefab = ResourceManager.Instance.LoadAsset<GameObject>(data.bundle, data.asset);
				GameObject go = GameObject.Instantiate(prefab);
				go.SetActive(true);
				ModelPrefab modelPrefab = go.GetComponent<ModelPrefab>();
				CurrentModelPrefab = modelPrefab;
				Transform model = go.transform;
				model.parent = modelRoot;
				model.localPosition = Vector3.zero;
				//model.localScale = Vector3.one * data.scale;
				//model.localRotation = Quaternion.identity;

				SpinCamera spin = SpinCamera.Instance;
				spin.Reset();
				spin.distance = modelPrefab.defaultCameraDistance;
				spin.minDistance = modelPrefab.minCameraDistance;
				spin.maxDistance = modelPrefab.maxCameraDistance;
				spin.target = model.position + modelPrefab.lookAtOffset;

				var luaTable = LuaManager.Instance.GetTable("Game");
				var fun = luaTable.GetInPath<XLua.LuaFunction>("OnLoadModel");
				fun.Call(go, modelPrefab.LuaModulePath);
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

}
