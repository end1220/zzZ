
using UnityEngine;
using UnityEditor;


namespace Lite
{
	public class BuildMenuItems
	{
		const string kSimulationMode = AppConst.AppName + "/AssetBundle/Simulation Mode";


		[MenuItem(kSimulationMode, false, 1)]
		public static void ToggleSimulationMode()
		{
			ResourceManager.SimulateAssetBundleInEditor = !ResourceManager.SimulateAssetBundleInEditor;
		}


		[MenuItem(kSimulationMode, true, 2)]
		public static bool ToggleSimulationModeValidate()
		{
			Menu.SetChecked(kSimulationMode, ResourceManager.SimulateAssetBundleInEditor);
			return true;
		}


		[MenuItem(AppConst.AppName + "/AssetBundle/Refresh AssetBundle Names", false, 3)]
		static public void RefreshABNames()
		{
			try
			{
				BuildAssetBundles.RefreshAssetBundleNames();
			}
			catch (System.Exception e)
			{
				Debug.LogError(e.ToString());
			}
		}

		[MenuItem(AppConst.AppName + "/AssetBundle/Build AB", false, 4)]
		static public void BuildAB()
		{
			try
			{
				BuildAssetBundles.Build_All();
			}
			catch (System.Exception e)
			{
				Debug.LogError(e.ToString());
			}
		}

		/*[MenuItem(AppConst.AppName + "/AssetBundle/Build Sbms", false, 4)]
		static public void BuildSbms()
		{
			try
			{
				BuildAssetBundles.BuildSubmanifests();
			}
			catch (System.Exception e)
			{
				Debug.LogError(e.ToString());
			}
		}*/

	}

}