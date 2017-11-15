
using System;
using System.Collections;
using UnityEngine;


public class StartUp : MonoBehaviour
{
	public static StartUp Instance { get; private set; }


	void Awake()
	{
		Instance = this;
		StartCoroutine(LoadResAndStartGame());
	}

	IEnumerator LoadResAndStartGame()
	{
		yield return new WaitForEndOfFrame();

		var request = ResourceManager.Instance.Initialize();
		if (request != null)
			yield return StartCoroutine(request);

		yield return ResourceManager.Instance.LoadAssetBundleAsync("lua/lua");

		OnLoaded();
	}


	private void OnLoaded()
	{
		try
		{
			LuaManager luaMgr = App.Instance.GetManager<LuaManager>();
			luaMgr.InitStart();
			luaMgr.StartMain();

			var luaTable = luaMgr.GetTable("Game");
			var fun = luaTable.GetInPath<XLua.LuaFunction>("OnInitOK");
			fun.Call();
		}
		catch (Exception e)
		{
			Log.Error(e.ToString());
		}
	}


}
