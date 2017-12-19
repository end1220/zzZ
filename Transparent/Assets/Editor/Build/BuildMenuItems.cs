
using UnityEngine;
using UnityEditor;



public class BuildMenuItems
{
	const string kSimulationMode = AppDefine.AppName + "/AssetBundle/Simulation Mode";


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


	[MenuItem(AppDefine.AppName + "/AssetBundle/Refresh AssetBundle Names", false, 3)]
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

	[MenuItem(AppDefine.AppName + "/AssetBundle/Build AB", false, 4)]
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

	/*[MenuItem(AppDefine.AppName + "/AssetBundle/Build Sbms", false, 4)]
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

