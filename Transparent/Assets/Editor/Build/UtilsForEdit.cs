#if UNITY_EDITOR
using UnityEditor;
#endif


static public class UtilsForEdit
{
	public static string GetPlatformName()
	{
#if UNITY_EDITOR
		return GetPlatformForAssetBundles(EditorUserBuildSettings.activeBuildTarget);
#else
            return "";
#endif
	}


#if UNITY_EDITOR
	public static string GetPlatformForAssetBundles(BuildTarget target)
	{
		switch (target)
		{
			case BuildTarget.Android:
				return "Android";
			case BuildTarget.iOS:
				return "iOS";
			case BuildTarget.WebGL:
				return "WebGL";
			case BuildTarget.StandaloneWindows:
			case BuildTarget.StandaloneWindows64:
				return "Windows";
			case BuildTarget.StandaloneOSXIntel:
			case BuildTarget.StandaloneOSXIntel64:
			case BuildTarget.StandaloneOSXUniversal:
				return "OSX";
			// Add more build targets for your own.
			// If you add more targets, don't forget to add the same platforms to GetPlatformForAssetBundles(RuntimePlatform) function.
			default:
				return null;
		}
	}
#endif

}
