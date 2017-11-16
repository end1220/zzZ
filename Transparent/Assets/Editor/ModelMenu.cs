
using UnityEngine;
using UnityEditor;



public class ModelMenu
{

	[MenuItem("Tools/Model/Refresh Model List", false, 3)]
	static public void RefreshModelList()
	{
		try
		{
			DataManager.RefreshModelList();
		}
		catch (System.Exception e)
		{
			Debug.LogError(e.ToString());
		}
	}

}

