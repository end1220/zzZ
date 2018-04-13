using UnityEngine;
using UnityEditor;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using Steamworks;

namespace Float
{
	public class ModifyOldItemPage : BaseItemPage
	{
		ProjectItemData projectData;

		public ModifyOldItemPage(FloatEditorWindow creator) :
			base(creator)
		{

		}

		protected override void SaveContext()
		{

		}

		protected override void OnShow(object param)
		{
			base.OnShow(param);

			projectData = (ProjectItemData)param;
			mPublishedFileId = new PublishedFileId_t(ulong.Parse(projectData.modeldata.workshopId));
		}

		protected override void OnOperateGUI()
		{
			if (projectData == null || projectData.modeldata == null)
				return;

			if (GUILayout.Button(Language.Get(TextID.submitToWorkshop), FloatGUIStyle.button, GUILayout.Width(200), GUILayout.Height(FloatGUIStyle.buttonHeight)))
			{
				if (CheckInputInfo())
				{
					EditorUtility.DisplayCancelableProgressBar("Submiting", "Update item", 0.2f);
					UpdateItem();
				}
			}
		}

	}


}