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
			base.SaveContext();
			UpdateSaveProjectFiles();
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

		private void UpdateSaveProjectFiles()
		{
			CopyPreviewFile(context.PreviewPath);

			string modelDataPath = context.ContentPath + "/" + AppConst.subModelDataName;
			string text = File.ReadAllText(modelDataPath);
			ModelData data = JsonUtility.FromJson<ModelData>(text);

			int endIdx = context.ContentPath.LastIndexOf("/");
			string rootPath = context.ContentPath.Substring(0, endIdx);
			string rootModelPath = rootPath + "/" + AppConst.subModelDataName;
			ModelAssetBuilder.SaveModelDataToFile(
					rootModelPath,
					data.workshopId,
					context.ItemTitle,
					context.ItemDesc,
					FormatPreviewFileName(context.PreviewPath),
					data.bundle,
					data.asset,
					context.Visibility,
					context.Category,
					context.Genre,
					context.Rating
					);

			ModelAssetBuilder.SaveModelDataToFile(
					modelDataPath,
					data.workshopId,
					context.ItemTitle,
					context.ItemDesc,
					FormatPreviewFileName(context.PreviewPath),
					data.bundle,
					data.asset,
					context.Visibility,
					context.Category,
					context.Genre,
					context.Rating
					);
		}
	}


}