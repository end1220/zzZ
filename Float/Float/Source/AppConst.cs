﻿
namespace Float
{
	public class AppConst
	{
		public const string AppName = "Floating";
		public static string remoteIP = "127.0.0.1";
		public static int remotePort = 1504;
		public const int listenPort = 1503;

		public static string PersistentDataPath
		{
			get
			{
				//return System.Environment.CurrentDirectory + "/ww_data/StreamingAssets/" + AppName + "/";
				return SteamManager.WorkshopInstallPath + "/";
			}
		}

		public const string modelListName = "projectlist.json";
		public const string subModelDataName = "project.json";
		//public const string manifestName = AppName + ".abm";
		//public const string subMetaName = "info.sbm";
		public static string modelListPath = PersistentDataPath + modelListName;
		//public static string modelPath = PersistentDataPath;
		public const string previewName = "preview";

		// message id
		public const int MSG_ITEM_INSTALLED = 100;
		public const int MSG_ITEM_DOWNLOADED = 101;
		public const int MSG_MODEL_LIST_READY = 102;
		public const int MSG_MODEL_LIST_UPDATE = 103;
		public const int MSG_WORKSHOP_PATH_FOUND = 104;
	}

}