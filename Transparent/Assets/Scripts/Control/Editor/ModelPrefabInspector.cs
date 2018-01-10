
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;


namespace Lite
{
	[CustomEditor(typeof(ModelPrefab))]
	public class ModelPrefabInspector : Editor
	{
		ModelPrefab modelPrefab;

		void OnEnable()
		{
			modelPrefab = target as ModelPrefab;
		}


		public override void OnInspectorGUI()
		{
			base.OnInspectorGUI();

			/*EditorGUILayout.Separator();

			if (GUILayout.Button("Save", GUILayout.Height(24.0f)))
			{
				Transform tran = modelPrefab.gameObject.GetComponent<Transform>();
				modelPrefab.scale = tran.lossyScale;

				EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
				EditorUtility.SetDirty(modelPrefab);
			}*/
		}

	}
}