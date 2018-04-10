

namespace Float
{
	public class ModifyOldItemPage : FloatEditorPage
	{
		private ModelAssetBuilder modelBuilder = new ModelAssetBuilder();

		public ModifyOldItemPage(FloatEditorWindow creator) :
			base(creator)
		{

		}

		public override void OnGUI()
		{
			modelBuilder.OnGUI();
		}

		public override void OnUpdate()
		{

		}

		public override void OnShow()
		{

		}

		public override void OnHide()
		{

		}
	}
}