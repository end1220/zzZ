using UnityEngine;
using UnityEditor;


namespace Float
{
	public class CreateNewItemPage : BaseItemPage
	{
		public CreateNewItemPage(FloatEditorWindow creator) :
			base(creator)
		{

		}

		protected override void SaveContext()
		{

		}

		protected override void OnOperateGUI()
		{
			if (GUILayout.Button(Language.Get(TextID.submitToWorkshop), FloatGUIStyle.button, GUILayout.Width(200), GUILayout.Height(FloatGUIStyle.buttonHeight)))
			{
				if (string.IsNullOrEmpty(modelBuilder.AssetbundlePath))
				{
					;
				}
				else if (CheckInputInfo())
				{
					EditorUtility.DisplayCancelableProgressBar("Submiting", "Create item", 0.2f);
					CreateItem();
				}
			}
		}

	}
}