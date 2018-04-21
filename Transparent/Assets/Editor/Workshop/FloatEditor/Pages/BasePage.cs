using UnityEditor;
using UnityEngine;

namespace Float
{
	public abstract class BasePage
	{
		protected FloatEditorWindow creatorWindow;

		float lastSaveTime = 0;

		public BasePage(FloatEditorWindow creator)
		{
			this.creatorWindow = creator;
		}

		public virtual void Destroy() { OnDestroy(); }

		public virtual void DrawGUI() { OnGUI(); }

		public virtual void Update()
		{
			OnUpdate();
			SaveContext();
		}

		public virtual void Show(object param) { OnShow(param); }

		public virtual void Hide() { OnHide(); }

		protected virtual void OnDestroy() { }

		protected virtual void OnGUI() { }

		protected virtual void OnUpdate() { }

		protected virtual void OnShow(object param) { }

		protected virtual void OnHide() { }

		protected virtual void SaveContext() { }

	}
}