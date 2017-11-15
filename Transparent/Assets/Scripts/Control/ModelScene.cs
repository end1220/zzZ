using UnityEngine;

public class ModelScene : MonoBehaviour
{
	public static ModelScene Instance { get; private set; }

	private Transform root;

	void Awake()
	{
		Instance = this;
		root = transform;
	}

	public void LoadModel()
	{

	}

	public void RemoveAll()
	{
		for (int i = 0; i < root.childCount; ++i)
		{
			Object.DestroyImmediate(root.GetChild(i).gameObject);
		}
	}
}
