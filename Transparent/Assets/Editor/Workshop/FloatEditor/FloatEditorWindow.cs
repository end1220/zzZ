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
			currentPage = pages[typeof(WelcomePage)];
		}

		private void OnDestroy()
		{
			
		}

		private void Update()
		{
			currentPage.OnUpdate();
		}

		void OnGUI()
		{
			currentPage.OnGUI();
		}
		
		public void ShowPage(Type type)
		{
			FloatEditorPage page;
			if (pages.TryGetValue(type, out page))
			{
				if (page == currentPage)
					return;
				currentPage.OnHide();
				currentPage = page;
				currentPage.OnShow();
			}
		}
	}


	public abstract class FloatEditorPage
	{
		protected FloatEditorWindow creator;

		public FloatEditorPage(FloatEditorWindow creator)
		{
			this.creator = creator;
		}

		public virtual void OnGUI()
		{

		}

		public virtual void OnUpdate()
		{

		}

		public virtual void OnShow()
		{

		}

		public virtual void OnHide()
		{

		}
	}

}