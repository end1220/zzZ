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
		public string tempPreviewPath;
	}

	public class WelcomePage : BasePage
	{
		private List<ProjectItemData> projectList = new List<ProjectItemData>();

		public WelcomePage(FloatEditorWindow creator) :
			base(creator)
		{

		}

		protected override void SaveContext()
		{

		}

		protected override void OnShow(object param)
		{
			if (SteamManager.Instance.Initialized)
			{
				LoadHistoryProjects();
				CopyPreviewsToEditor();
			}
		}

		protected override void OnDestroy()
		{
			// do not delete
			//DeleteCopiedPreviews();
		}

		Vector2 scrollPosition = Vector2.zero;
		protected override void OnGUI()
		{
			int space = 5;
			int recentWidth = 400;
			int areaHeight = 500;
			GUILayout.Space(10);
			GUILayout.BeginHorizontal();
			GUILayout.Space(10);
			GUILayout.Label(Language.Get(TextID.mainHelpBox), FloatGUIStyle.helpBox, GUILayout.Width(400), GUILayout.Height(60));
			GUILayout.Space(40);
			GUILayout.Label(Language.Get(TextID.language), GUILayout.Width(60));
			Language.langType = (LangType)EditorGUILayout.EnumPopup(Language.langType, GUILayout.Width(150));
			GUILayout.EndHorizontal();
			GUILayout.Space(20);

			if (!SteamManager.Instance.Initialized)
			{
				GUILayout.BeginVertical();
				GUILayout.Label(Language.Get(TextID.errorTitle), FloatGUIStyle.largeLabel);
				GUILayout.Label(Language.Get(TextID.steamInitError), FloatGUIStyle.largeLabel);
				GUILayout.EndVertical();
				return;
			}

			GUILayout.BeginHorizontal();
			GUILayout.Space(10);// to left window border

			GUILayout.BeginVertical();
			GUILayout.Label(Language.Get(TextID.recentProject), FloatGUIStyle.largeLabel, GUILayout.Width(recentWidth));
			GUILayout.Space(space);
			GUILayout.Label("", FloatGUIStyle.helpBox, GUILayout.Width(recentWidth + 65), GUILayout.Height(areaHeight));
			GUILayout.EndVertical();

			GUILayout.BeginArea(new Rect(15, 135, recentWidth + 50, areaHeight));
			if (projectList.Count == 0)
			{
				GUILayout.Label(Language.Get(TextID.noProject), FloatGUIStyle.label, GUILayout.Width(recentWidth));
				GUILayout.Space(space);
			}
			else
			{
				scrollPosition = GUILayout.BeginScrollView(scrollPosition, GUILayout.Width(recentWidth + 50), GUILayout.Height(areaHeight - 20));
				foreach (var project in projectList)
				{
					GUILayout.BeginHorizontal();
					if (GUILayout.Button(Resources.Load(project.tempPreviewPath) as Texture, GUILayout.Width(100), GUILayout.Height(100)))
						creatorWindow.OpenPage(typeof(ModifyOldItemPage).Name, project);
					
					GUILayout.BeginVertical();
					GUILayout.Space(10);
					float textWidth = recentWidth - 80;
					GUILayout.Label(project.modeldata.title, FloatGUIStyle.boldLabel, GUILayout.Width(textWidth));
					GUILayout.Label(project.modeldata.description, FloatGUIStyle.label, GUILayout.Width(textWidth), GUILayout.Height(74));
					GUILayout.EndVertical();
					GUILayout.EndHorizontal();
				}
				GUILayout.EndScrollView();
			}
			GUILayout.EndArea();

			GUILayout.Space(30);
			GUILayout.BeginVertical();
			GUILayout.Label(Language.Get(TextID.createProject), FloatGUIStyle.largeLabel, GUILayout.Width(300));
			GUILayout.Space(space);
			GUILayout.Label("", FloatGUIStyle.helpBox, GUILayout.Width(240), GUILayout.Height(areaHeight));
			GUILayout.EndVertical();

			GUILayout.BeginArea(new Rect(520, 140, 200, areaHeight));
			if (GUILayout.Button(Language.Get(TextID.newProject), FloatGUIStyle.button, GUILayout.Width(120), GUILayout.Height(30)))
				creatorWindow.OpenPage(typeof(CreateNewItemPage).Name, null);
			GUILayout.EndArea();

			
			GUILayout.EndHorizontal();
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
					string folderName = Path.GetFileName(dir);
					int uid = 0;
					if (!int.TryParse(folderName, out uid))
						continue;

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
			else
			{
				Directory.CreateDirectory(AppConst.projectsPath);
			}
		}

		private string GetTempPreviewsPath()
		{
			return Application.dataPath + "/Editor/Resources/temp_welcomepage/";
		}

		private void CopyPreviewsToEditor()
		{
			string tempDir = GetTempPreviewsPath();
			//if (Directory.Exists(tempDir))
			//	FileUtil.DeleteFileOrDirectory(tempDir);
			if (!Directory.Exists(tempDir))
				Directory.CreateDirectory(tempDir);

			foreach (var project in projectList)
			{
				string folderName = Path.GetFileName(project.directory);
				string targetDir = Path.Combine(tempDir, folderName);
				Directory.CreateDirectory(targetDir);
				string sourceDir = Path.Combine(project.directory, project.modeldata.preview);
				string destPath = Path.Combine(targetDir, project.modeldata.preview);
				if (!File.Exists(destPath))
					File.Copy(sourceDir, destPath, true);
				project.tempPreviewPath = "temp_welcomepage/" + folderName + "/" + AppConst.previewName;
			}

			AssetDatabase.Refresh();
		}

		private void DeleteCopiedPreviews()
		{
			string tempDir = GetTempPreviewsPath();
			if (Directory.Exists(tempDir))
				FileUtil.DeleteFileOrDirectory(tempDir);
		}

	}

}