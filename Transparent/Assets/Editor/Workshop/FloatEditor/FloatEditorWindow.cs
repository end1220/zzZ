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

		void Awake()
		{
			FloatGUIStyle.Ensure();
			pages.Add(typeof(WelcomePage), new WelcomePage(this));
			pages.Add(typeof(CreateNewItemPage), new CreateNewItemPage(this));
			pages.Add(typeof(ModifyOldItemPage), new ModifyOldItemPage(this));
	
			OpenPage(typeof(WelcomePage), null);

			if (!SteamManager.Instance.Initialized)
				SteamManager.Instance.Init();
		}

		private void OnDestroy()
		{
			foreach (var pg in pages.Values)
				pg.OnDestroy();
			
			if (SteamManager.Instance.Initialized)
				SteamManager.Instance.Destroy();
		}

		private void Update()
		{
			currentPage.OnUpdate();
		}

		void OnGUI()
		{
			currentPage.OnGUI();
		}

		public void OpenPage(Type type, object param)
		{
			FloatEditorPage page;
			if (pages.TryGetValue(type, out page))
			{
				if (page == currentPage)
					return;
				if (currentPage != null)
					currentPage.OnHide();
				currentPage = page;
				currentPage.OnShow(param);
			}
		}
	}


	public abstract class FloatEditorPage
	{
		protected FloatEditorWindow creatorWindow;

		public FloatEditorPage(FloatEditorWindow creator)
		{
			this.creatorWindow = creator;
		}

		public virtual void OnDestroy() { }

		public virtual void OnGUI() { }

		public virtual void OnUpdate() { }

		public virtual void OnShow(object param) { }

		public virtual void OnHide() { }

	}

}