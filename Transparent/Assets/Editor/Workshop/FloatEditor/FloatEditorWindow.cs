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
			window.Reset();
			window.Show();
		}

		private class FloatEditorWindowContext
		{
			public string currentPage;
		}

		string layoutSavePath = Environment.CurrentDirectory.Replace("\\", "/") + "/ProjectSettings/FloatEditor";
		private FloatEditorWindowContext context = new FloatEditorWindowContext();
		private BasePage currentPage = null;
		private Dictionary<string, BasePage> pagesDic = new Dictionary<string, BasePage>();

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
			if (SteamManager.Instance.Initialized)
				SteamManager.Instance.Update();
			currentPage.Update();
		}

		void OnGUI()
		{
			FloatGUIStyle.Ensure();
			currentPage.DrawGUI();
		}

		public void OpenPage(string type, object param)
		{
			BasePage page;
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

		public void OpenDefaultPage()
		{
			OpenPage(typeof(WelcomePage).Name, null);
		}

		private void Ensure()
		{
			if (!SteamManager.Instance.Initialized)
				SteamManager.Instance.Init();

			if (pagesDic.Count == 0)
			{
				pagesDic.Add(typeof(WelcomePage).Name, new WelcomePage(this));
				pagesDic.Add(typeof(CreateNewItemPage).Name, new CreateNewItemPage(this));
				pagesDic.Add(typeof(ModifyOldItemPage).Name, new ModifyOldItemPage(this));

				if (File.Exists(layoutSavePath))
				{
					context = JsonConvert.DeserializeObject<FloatEditorWindowContext>(File.ReadAllText(layoutSavePath));
					if (!string.IsNullOrEmpty(context.currentPage))
					{
						if (!string.IsNullOrEmpty(context.currentPage) && pagesDic.ContainsKey(context.currentPage))
							OpenPage(context.currentPage, null);
					}
				}
				else
					OpenDefaultPage();
			}
		}

		private void SaveContext()
		{
			context.currentPage = currentPage.GetType().Name;
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
				OpenDefaultPage();
			}
		}
	}

}