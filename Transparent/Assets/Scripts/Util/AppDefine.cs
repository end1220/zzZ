
using UnityEngine;


public class AppDefine
{
	public const string AppName = "Floating";
	public const int FPS = 30;

	public static string remoteIP = "127.0.0.1";
	public static int remotePort = 1503;
	public const int listenPort = 1504;

	public static string PersistentDataPath
	{
		get
		{
			if (Application.isMobilePlatform)
			{
				return Application.persistentDataPath + "/" + AppName + "/";
			}
			else if (Application.platform == RuntimePlatform.OSXEditor)
			{
				int i = Application.dataPath.LastIndexOf('/');
				return Application.dataPath.Substring(0, i + 1) + AppName + "/";
			}
			else if (Application.platform == RuntimePlatform.WindowsEditor)
			{
				return "D:/PersistentAssets/" + AppName + "/";
			}
			else if (Application.platform == RuntimePlatform.WindowsPlayer)
			{
				return Application.streamingAssetsPath + "/";
			}

			return string.Empty;
		}
	}

}