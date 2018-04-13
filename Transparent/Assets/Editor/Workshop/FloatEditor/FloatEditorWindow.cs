using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;

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
		}

		///////////////////

		private FloatEditorPage currentPage = null;
		private Dictionary<Type, FloatEditorPage> pages = new Dictionary<Type, FloatEditorPage>();

		private void Ensure()
		{
			if (!SteamManager.Instance.Initialized)
				SteamManager.Instance.Init();

			FloatGUIStyle.Ensure();

			if (pages.Count == 0)
			{
				pages.Add(typeof(WelcomePage), new WelcomePage(this));
				pages.Add(typeof(CreateNewItemPage), new CreateNewItemPage(this));
				pages.Add(typeof(ModifyOldItemPage), new ModifyOldItemPage(this));

				OpenPage(typeof(WelcomePage), null);
			}
		}

		private void OnDestroy()
		{
			foreach (var pg in pages.Values)
				pg.Destroy();
			
			if (SteamManager.Instance.Initialized)
				SteamManager.Instance.Destroy();
		}

		private void Update()
		{
			Ensure();
			currentPage.Update();
		}

		void OnGUI()
		{
			currentPage.DrawGUI();
			SaveContext();
		}

		public void OpenPage(Type type, object param)
		{
			FloatEditorPage page;
			if (pages.TryGetValue(type, out page))
			{
				if (page == currentPage)
					return;
				if (currentPage != null)
					currentPage.Hide();
				currentPage = page;
				currentPage.Show(param);
			}
		}

		private void SaveContext()
		{

		}

	}

}