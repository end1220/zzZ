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

		public override void OnShow(object param)
		{
			LoadHistoryProjects();
			CopyPreviewsToEditor();
		}

		public override void OnDestroy()
		{

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
					if (GUILayout.Button(Resources.Load(AppConst.previewName) as Texture, GUILayout.Width(100), GUILayout.Height(100)))
					{
						creatorWindow.OpenPage(typeof(ModifyOldItemPage), project);
					}
					GUILayout.BeginVertical();
					GUILayout.Label(project.modeldata.title, FloatGUIStyle.label);
					GUILayout.Label(project.modeldata.description, FloatGUIStyle.label);
					GUILayout.EndVertical();
					GUILayout.EndHorizontal();
				}
				GUILayout.EndScrollView();
			}

			GUILayout.Label("........................................................");
			GUILayout.Space(space);

			if (GUILayout.Button("Create new project", FloatGUIStyle.button, GUILayout.Width(200), GUILayout.Height(50)))
			{
				creatorWindow.OpenPage(typeof(CreateNewItemPage), null);
			}

			GUILayout.EndVertical();
		}

		private void LoadHistoryProjects()
		{
			projectList.Clear();
			if (Directory.Exists(AppConst.projectsPath))
			{
				string[] dirs = Directory.GetDirectories(AppConst.projectsPath);
				foreach (string d in dirs)
				{
					string dir = d.Replace("\\", "/");
					string modelDataPath = Path.Combine(dir, AppConst.subModelDataName);
					if (!File.Exists(modelDataPath))
					{
						Log.Error(AppConst.subModelDataName + " is missing in " + dir);
						continue;
					}
					var data = JsonConvert.DeserializeObject<ModelData>(File.ReadAllText(modelDataPath));
					if (data == null)
					{
						Log.Error("Deserialize failed: " + modelDataPath);
						continue;
					}
					string previewPath = Path.Combine(dir, data.preview);
					if (!File.Exists(previewPath))
					{
						Log.Error(previewPath + " is missing");
						continue;
					}
					string bundlePath = Path.Combine(Path.Combine(dir, AppConst.contentFolderName), AppConst.assetbundleName);
					if (!File.Exists(bundlePath))
					{
						Log.Error(AppConst.assetbundleName + " is missing in " + Path.Combine(dir, AppConst.contentFolderName));
						continue;
					}
					string sbmPath = Path.Combine(Path.Combine(dir, AppConst.contentFolderName), AppConst.subManifestName);
					if (!File.Exists(sbmPath))
					{
						Log.Error(AppConst.subManifestName + " is missing in " + Path.Combine(dir, AppConst.contentFolderName));
						continue;
					}

					ProjectItemData item = new ProjectItemData() { directory = dir, modeldata = data };
					projectList.Add(item);
				}
			}
		}

		private string GetTempPreviewsPath()
		{
			return Application.dataPath + "/Editor/Resources/welcomepage_temp/";
		}

		private void CopyPreviewsToEditor()
		{
			string tempDir = GetTempPreviewsPath();
			if (Directory.Exists(tempDir))
				FileUtil.DeleteFileOrDirectory(tempDir);
			Directory.CreateDirectory(tempDir);

			foreach (var project in projectList)
			{
				string targetDir = tempDir + Path.GetDirectoryName(project.directory);
				Directory.CreateDirectory(targetDir);
				string sourceDir = Path.Combine(project.directory, project.modeldata.preview);
				string destPath = Path.Combine(targetDir, project.modeldata.preview);
				File.Copy(sourceDir, destPath);
			}
		}

		private void DeleteCopiedPreviews()
		{
			string tempDir = GetTempPreviewsPath();
			if (Directory.Exists(tempDir))
				FileUtil.DeleteFileOrDirectory(tempDir);
		}

	}

}