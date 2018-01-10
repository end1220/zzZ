using UnityEngine;
using System.IO;
using System.Text;


/// <summary>
/// 集成自LuaFileUtils，重写里面的ReadFile，
/// </summary>
public class LuaLoader : FileUtils
{

	// Use this for initialization
	public LuaLoader()
	{
		instance = this;
#if UNITY_EDITOR
		beBundle = !Lite.ResourceManager.SimulateAssetBundleInEditor;
#else
		beBundle = true;
#endif
	}

	/// <summary>
	/// 添加打入Lua代码的AssetBundle
	/// </summary>
	/// <param name="bundleName"></param>
	public void AddBundle(string bundleName)
	{
		string url = AppDefine.PersistentDataPath + bundleName.ToLower();
        string err = null;
		if (File.Exists(url))
		{
            AssetBundle bundle = null;
			Lite.LoadedAssetBundle loadedBundle = Lite.ResourceManager.Instance.GetLoadedAssetBundle(bundleName, out err);
            if (loadedBundle == null)
            {
                bundle = AssetBundle.LoadFromFile(url);
                if (bundle == null)
                    Log.Info(string.Format("Add Lua Bundle path: {0}. failed load it from file.", url));
            }
            else
                bundle = loadedBundle.assetBundle;
            if (bundle != null)
                base.AddSearchBundle(bundleName.ToLower(), bundle);
        }
        else
		{
			Log.Info(string.Format("Add Lua Bundle path: {0}. file not exist.", url));
		}
	}

	/// <summary>
	/// 当LuaVM加载Lua文件的时候，这里就会被调用，
	/// 用户可以自定义加载行为，只要返回byte[]即可。
	/// </summary>
	/// <param name="fileName"></param>
	/// <returns></returns>
	public override byte[] ReadFile(string fileName)
	{
		byte[] buffer = base.ReadFile(fileName);

		return buffer;
	}



}