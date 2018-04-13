using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.IO;

namespace Float
{
	public class FloatEditorWindow : EditorWindow
	{

		[MenuItem(AppConst.AppName + "/Float Editor")]
		public static void ShowExcelWindow()
		{
			Rect wr = new Rect(100, 100, 800, 700);
			var window = (FloatEditorWindow)EditorWindow.GetWindowWithRect(typeof(FloatEditorWindow), wr, true, Language.Get(TextID.wndTitle));
			window.Show();
			window.Reset();
		}

		private class FloatEditorWindowContext
		{
			public int currentPageIndex;
		}

		string layoutSavePath = Environment.CurrentDirectory.Replace("\\", "/") + "/ProjectSettings/FloatEditor";
		private FloatEditorWindowContext context = new FloatEditorWindowContext();
		private FloatEditorPage currentPage = null;
		private Dictionary<Type, FloatEditorPage> pagesDic = new Dictionary<Type, FloatEditorPage>();

		private void Awake()
		{
			Ensure();
		}

		private void OnDestroy()
		{
			foreach (var pg in pagesDic.Values)
				pg.Destroy();
			
			if (SteamManager.Instance.Initialized)
				SteamManager.Instance.Destroy();

			DeleteLayoutFile();
		}

		private void Update()
		{
			Ensure();
			currentPage.Update();
		}

		void OnGUI()
		{
			currentPage.DrawGUI();
		}

		public void OpenPage(Type type, object param)
		{
			FloatEditorPage page;
			if (pagesDic.TryGetValue(type, out page))
			{
				if (page == currentPage)
					return;
				if (currentPage != null)
					currentPage.Hide();
				currentPage = page;
				currentPage.Show(param);

				SaveContext();
			}
		}

		private void Ensure()
		{
			if (!SteamManager.Instance.Initialized)
				SteamManager.Instance.Init();

			FloatGUIStyle.Ensure();

			if (pagesDic.Count == 0)
			{
				pagesDic.Add(typeof(WelcomePage), new WelcomePage(this));
				pagesDic.Add(typeof(CreateNewItemPage), new CreateNewItemPage(this));
				pagesDic.Add(typeof(ModifyOldItemPage), new ModifyOldItemPage(this));

				Type tp = typeof(WelcomePage);
				if (File.Exists(layoutSavePath))
					context = JsonConvert.DeserializeObject<FloatEditorWindowContext>(File.ReadAllText(layoutSavePath));
				if (context.currentPageIndex == 0)
					tp = typeof(WelcomePage);
				else if (context.currentPageIndex == 1)
					tp = typeof(CreateNewItemPage);
				else if (context.currentPageIndex == 2)
					tp = typeof(ModifyOldItemPage);
				OpenPage(tp, null);
			}
		}

		private void SaveContext()
		{
			string str = JsonConvert.SerializeObject(context);
			File.WriteAllText(this.layoutSavePath, str);
		}

		private void DeleteLayoutFile()
		{
			if (File.Exists(layoutSavePath))
				File.Delete(layoutSavePath);
		}

		private void Reset()
		{
			DeleteLayoutFile();
			if (pagesDic.Count > 0)
			{
				OpenPage(typeof(WelcomePage), null);
			}
		}
	}

}