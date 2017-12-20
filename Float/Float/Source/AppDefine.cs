
public class AppDefine
{
	public const string AppName = "Floating";
	public static string remoteIP = "127.0.0.1";
	public static int remotePort = 1504;
	public const int listenPort = 1503;

	public static string PersistentDataPath
	{
		get
		{
			return System.Environment.CurrentDirectory + "/ff_data/StreamingAssets/" + AppName + "/";
		}
	}

	public const string modelListName = "modellist.json";
	public const string subModelDataName = "modeldata.json";
	public const string manifestName = AppName + ".abm";
	public const string subMetaName = "info.sbm";
	public static string modelListPath = PersistentDataPath + modelListName;
	public static string modelPath = PersistentDataPath;
}