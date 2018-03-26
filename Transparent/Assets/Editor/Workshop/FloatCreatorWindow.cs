using UnityEngine;
using UnityEditor;
using System;


namespace Lite
{
	public partial class FloatCreatorWindow : EditorWindow
	{
		[MenuItem(AppDefine.AppName + "/Float Creator")]
		public static void ShowExcelWindow()
		{
			Rect wr = new Rect(100, 100, 800, 700);
			var window = (FloatCreatorWindow)EditorWindow.GetWindowWithRect(typeof(FloatCreatorWindow), wr, true, Language.Get(TextID.wndTitle));
			window.Show();
		}

        string modelPath = "";
        string outputPath = "Output/" + AppDefine.AppName;
        string prefabPath = "";
        ModelPrefab prefab;

		string itemTitle = "Test title";
		string itemDesc = "This is the test description.";
		string previewFilePath = "";
		bool agreeWorkshopPolicy = false;

		int opreationIndex = 0;


        void OnEnable()
        {
			FloatGUIStyle.Ensure();
			InitSteamAPI();
			AppUtils.SetRandomSeed(DateTime.Now.Millisecond * DateTime.Now.Second);

			if (!SteamManager.Instance.Initialized)
			{
				EditorUtility.DisplayDialog(Language.Get(TextID.errorTitle), Language.Get(TextID.steamInitError),
					Language.Get(TextID.ok), Language.Get(TextID.cancel));
			}
		}

        private void OnDestroy()
		{
			DestroySteamAPI();
		}

		private void Update()
		{
			SteamManager.Instance.Update();
		}

		float spaceSize = 15f;
		float leftSpace = 10;
		float titleLen = 100;
		float textLen = 600;
		float buttonLen1 = 100;
		float buttonLen2 = 50;
		float buttonHeight = 40;

		void OnGUI()
		{
			if (!SteamManager.Instance.Initialized)
				return;

			GUILayout.BeginHorizontal();
			GUILayout.Label(Language.Get(TextID.mainHelpBox), FloatGUIStyle.helpBox, GUILayout.Width(400), GUILayout.Height(60));
			GUILayout.Space(40);
			GUILayout.Label(Language.Get(TextID.language), GUILayout.Width(60));
			Language.langType = (LangType)EditorGUILayout.EnumPopup(Language.langType, GUILayout.Width(150));
			GUILayout.EndHorizontal();
			GUILayout.Label("——————————————————————————————————————————————————————");
			GUILayout.Space(20);

			string[] operations = new string[] { Language.Get(TextID.export), Language.Get(TextID.submit) };
			opreationIndex = GUILayout.SelectionGrid(opreationIndex, operations, operations.Length, GUILayout.Width(600), GUILayout.Height(25));
			GUILayout.Label("——————————————————————————————————————————————————————");
			GUILayout.Space(10);
			if (opreationIndex == 0)
				OnBuildGUI();
			else if (opreationIndex == 1)
				OnSubmitGUI();
		}

	}

	

}