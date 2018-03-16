using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using Newtonsoft.Json;


namespace Lite
{
	public class WorkshopWindow : EditorWindow
	{

		[MenuItem(AppDefine.AppName + "/Workshop")]
		public static void ShowExcelWindow()
		{
			Rect wr = new Rect(100, 100, 640, 480);
			var window = (WorkshopWindow)EditorWindow.GetWindowWithRect(typeof(WorkshopWindow), wr, true, "Upload");
			window.Show();
		}

		string zipFilePath = "";
		string titleDesc = "No title";
		string previewFilePath = "";


		void OnGUI()
		{
			if (SteamManager.Initialized)
			{
				float spaceSize = 10f;
				float leftSpace = 10;
				float titleLen = 70;
				float textLen = 450;
				float buttonLen1 = 100;
				float buttonLen2 = 50;
				float buttonHeight = 40;

				GUILayout.Label("Workshop", EditorStyles.helpBox);
				GUILayout.Space(spaceSize);

				GUILayout.BeginHorizontal();
				GUILayout.Space(leftSpace);
				GUILayout.Label("Zip File", EditorStyles.label, GUILayout.Width(titleLen));
				string savedZipPath = EditorPrefs.GetString("BMW_ZipPath");
				zipFilePath = string.IsNullOrEmpty(savedZipPath) ? zipFilePath : savedZipPath;
				zipFilePath = GUILayout.TextField(zipFilePath, GUILayout.Width(textLen));
				if (GUILayout.Button("Select", GUILayout.Width(buttonLen2)))
				{
					zipFilePath = EditorUtility.OpenFilePanel("Select Zip File", String.Empty, "*zip");
					if (!string.IsNullOrEmpty(zipFilePath))
						EditorPrefs.SetString("BMW_ZipPath", zipFilePath);
				}
				GUILayout.EndHorizontal();
				GUILayout.Space(spaceSize);

				GUILayout.BeginHorizontal();
				GUILayout.Space(spaceSize);
				GUILayout.Label("Title", EditorStyles.label, GUILayout.Width(titleLen));
				titleDesc = GUILayout.TextField(titleDesc, GUILayout.Width(textLen));
				GUILayout.EndHorizontal();
				GUILayout.Space(leftSpace);

				GUILayout.BeginHorizontal();
				GUILayout.Space(spaceSize);
				GUILayout.Label("Preview", EditorStyles.label, GUILayout.Width(titleLen));
				string savedPreviewPath = EditorPrefs.GetString("BMW_PreviewPath");
				previewFilePath = string.IsNullOrEmpty(savedPreviewPath) ? previewFilePath : savedPreviewPath;
				previewFilePath = GUILayout.TextField(previewFilePath, GUILayout.Width(textLen));
				if (GUILayout.Button("Select", GUILayout.Width(buttonLen2)))
				{
					zipFilePath = EditorUtility.OpenFilePanel("Select Preview File", String.Empty, "*.jpg");
					if (!string.IsNullOrEmpty(zipFilePath))
						EditorPrefs.SetString("BMW_PreviewPath", zipFilePath);
				}
				GUILayout.EndHorizontal();
				GUILayout.Space(leftSpace);

				GUILayout.BeginHorizontal();
				GUILayout.Space(leftSpace);
				if (GUILayout.Button("Upload", GUILayout.Width(buttonLen1), GUILayout.Height(buttonHeight)))
				{

				}

				GUILayout.EndHorizontal();
				GUILayout.Space(spaceSize);
			}
			else
			{
				GUILayout.Label("SteamAPI failed", EditorStyles.label);
			}
		}

		private void Awake()
		{
			if (!SteamManager.Initialized)
				SteamManager.Instance.Init();
		}

	}

}