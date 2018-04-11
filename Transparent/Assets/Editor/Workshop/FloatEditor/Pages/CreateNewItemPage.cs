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

		protected override void OnOperateGUI()
		{
			if (GUILayout.Button(Language.Get(TextID.submitToWorkshop), FloatGUIStyle.button, GUILayout.Width(200), GUILayout.Height(FloatGUIStyle.buttonHeight)))
			{
				if (string.IsNullOrEmpty(modelBuilder.AssetBundlePath))
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