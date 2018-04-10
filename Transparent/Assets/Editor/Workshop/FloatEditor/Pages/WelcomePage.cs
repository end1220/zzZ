using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Float
{
	public class ProjectItemData
	{
		public string directory;
		public ModelData modeldata;
	}

	public class WelcomePage : FloatEditorPage
	{
		private List<ProjectItemData> projectList = new List<ProjectItemData>();

		public WelcomePage(FloatEditorWindow creator) :
			base(creator)
		{

		}

		public override void OnShow()
		{
			/*string installPath = "";*/
			string projectsPath = "d:/Float/projects";

			projectList.Clear();
			if (Directory.Exists(projectsPath))
			{
				string[] dirs = Directory.GetDirectories(projectsPath);
				foreach (string d in dirs)
				{
					string dir = d.Replace("\\", "/");
					string modelDataPath = Path.Combine(dir, AppConst.subModelDataName);
					if (!File.Exists(modelDataPath))
					{
						Log.Error(AppConst.subModelDataName + " is missing in " + dir);
						continue;
					}
					string previewPath = Path.Combine(dir, AppConst.previewName);
					if (!File.Exists(previewPath + ".jpg") && !File.Exists(previewPath + ".png"))
					{
						Log.Error(AppConst.previewName + " image is missing in " + dir);
						continue;
					}
					string bundlePath = Path.Combine(Path.Combine(dir, AppConst.contentFolderName), AppConst.assetbundleName);
					if (!File.Exists(bundlePath))
					{
						Log.Error(AppConst.assetbundleName + " is missing in " + Path.Combine(dir, AppConst.contentFolderName));
						continue;
					}
					string sbmPath = Path.Combine(Path.Combine(dir, AppConst.contentFolderName), AppConst.subMetaName);
					if (!File.Exists(sbmPath))
					{
						Log.Error(AppConst.subMetaName + " is missing in " + Path.Combine(dir, AppConst.contentFolderName));
						continue;
					}

					var data = JsonConvert.DeserializeObject<ModelData>(File.ReadAllText(modelDataPath));
					if (data == null)
					{
						Log.Error("Deserialize failed: " + modelDataPath);
						continue;
					}
					ProjectItemData item = new ProjectItemData() { directory = dir, modeldata = data };
					projectList.Add(item);
				}
			}
		}

		public override void OnGUI()
		{
			int space = 5;

			GUILayout.BeginHorizontal();
			GUILayout.Label(Language.Get(TextID.mainHelpBox), FloatGUIStyle.helpBox, GUILayout.Width(400), GUILayout.Height(60));
			GUILayout.Space(40);
			GUILayout.Label(Language.Get(TextID.language), GUILayout.Width(60));
			Language.langType = (LangType)EditorGUILayout.EnumPopup(Language.langType, GUILayout.Width(150));
			GUILayout.EndHorizontal();
			GUILayout.Label("——————————————————————————————————————————————————————");
			GUILayout.Space(20);

			if (!SteamManager.Instance.Initialized)
			{
				GUILayout.BeginVertical();
				GUILayout.Label(Language.Get(TextID.errorTitle), FloatGUIStyle.largeLabel);
				GUILayout.Label(Language.Get(TextID.steamInitError), FloatGUIStyle.largeLabel);
				GUILayout.EndVertical();
				return;
			}

			GUILayout.BeginVertical();

			GUILayout.Label("Recent projects:", FloatGUIStyle.largeLabel);
			GUILayout.Space(space);

			if (projectList.Count == 0)
			{
				GUILayout.Label("    No projects", FloatGUIStyle.label);
				GUILayout.Space(space);
			}
			else
			{
				GUILayout.BeginScrollView(new Vector2(10, 10), GUILayout.Width(500), GUILayout.Height(400));
				foreach (var project in projectList)
				{
					GUILayout.BeginHorizontal();
					GUILayout.Box(Resources.Load(AppConst.previewName) as Texture, GUILayout.Width(64), GUILayout.Height(64));
					GUILayout.BeginVertical();
					GUILayout.Label(project.modeldata.title, FloatGUIStyle.label);
					GUILayout.Label(project.modeldata.description, FloatGUIStyle.label);
					GUILayout.EndVertical();
					if (GUILayout.Button("open"))
					{
						creatorWindow.OpenPage(typeof(ModifyOldItemPage));
					}
					GUILayout.EndHorizontal();
				}
				GUILayout.EndScrollView();
			}

			GUILayout.Label("........................................................");
			GUILayout.Space(space);

			if (GUILayout.Button("Create new project", FloatGUIStyle.button, GUILayout.Width(200), GUILayout.Height(50)))
			{
				creatorWindow.OpenPage(typeof(CreateNewItemPage));
			}

			GUILayout.EndVertical();
		}

	}

}