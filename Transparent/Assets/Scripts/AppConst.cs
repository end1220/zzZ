
using UnityEngine;


public class AppConst
{
	public const string AppName = "Floating";

	/// <summary>
	/// 热更到本地的数据存放目录.
	/// 注意standalone模式www不能读取persistentDataPath.
	/// </summary>
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