
namespace Float
{
	public abstract class FloatEditorPage
	{
		protected FloatEditorWindow creatorWindow;

		public FloatEditorPage(FloatEditorWindow creator)
		{
			this.creatorWindow = creator;
		}

		public virtual void Destroy() { OnDestroy(); }

		public virtual void DrawGUI() { OnGUI(); SaveContext(); }

		public virtual void Update() { OnUpdate(); }

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