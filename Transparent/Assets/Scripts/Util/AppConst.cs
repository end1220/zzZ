
using UnityEngine;


public class AppConst
{
	public const string AppName = "Floating";
	public const int FPS = 30;

	public const string remoteIP = "127.0.0.1";
	public const int remotePort = 1503;
	public const int listenPort = 1504;

	public static bool LocalMode = true;

	public static string PersistentDataPath
	{
		get
		{
			return Application.streamingAssetsPath + "/" + AppName + "/";
		}
	}

	public const string modelListName = "projectlist.json";
	public const string subModelDataName = "project.json";
	public const string assetbundleName = "assetbundle";
	public const string manifestName = AppName + ".abm";
	public const string subMetaName = "info.sbm";
	public static string manifestPath = PersistentDataPath + "/" + manifestName;
	public static string modelListPath = PersistentDataPath + "/" + modelListName;
	public static string modelPath = PersistentDataPath;

	public const string workshopPolicyUrl = "http://steamcommunity.com/sharedfiles/workshoplegalagreement";
	public const string previewName = "preview";
}