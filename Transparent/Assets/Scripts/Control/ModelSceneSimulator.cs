
using UnityEngine;



public class ModelSceneSimulator : MonoBehaviour
{
	public ModelPrefab modelPrefab;

	private SpinCamera spinCamera;

	private void Start()
	{
		if (modelPrefab == null)
		{
			Debug.LogError("ModelSceneSimulator: modelPrefab not found.");
			return;
		}

		GameObject cam = GameObject.Find("Main Camera");
		if (cam == null)
		{
			Debug.LogError("ModelSceneSimulator: Cannot find \"Main Camera\" in current scene.");
			return;
		}

		GameObject prefab = modelPrefab.gameObject;
		GameObject go = GameObject.Instantiate(prefab);
		Transform model = go.transform;
		model.parent = transform;
		model.localPosition = Vector3.zero;
		model.localScale = modelPrefab.scale;
		model.localRotation = Quaternion.identity;

		SpinCamera spin = cam.GetComponent<SpinCamera>();
		if (spin == null)
			spin = cam.AddComponent<SpinCamera>();
		spinCamera = spin;
		spin.distance = modelPrefab.defaultCameraDistance;
		spin.minDistance = modelPrefab.minCameraDistance;
		spin.maxDistance = modelPrefab.maxCameraDistance;
		spin.target = model.position + modelPrefab.lookAtOffset;

		KeyboardMouse mouse = cam.GetComponent<KeyboardMouse>();
		if (mouse == null)
			mouse = cam.AddComponent<KeyboardMouse>();
	}

	
}
