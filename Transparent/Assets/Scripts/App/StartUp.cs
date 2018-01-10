
using System;
using System.Collections;
using UnityEngine;


namespace Lite
{

	public class StartUp : MonoBehaviour
	{
		public static StartUp Instance { get; private set; }


		void Awake()
		{
			Instance = this;
			StartCoroutine(LoadResAndStart());
		}

		IEnumerator LoadResAndStart()
		{
			yield return new WaitForEndOfFrame();

			ResourceManager.Instance.Initialize();

			yield return ResourceManager.Instance.LoadAssetBundleAsync("lua/common");
			yield return ResourceManager.Instance.LoadAssetBundleAsync("lua/lua");
			yield return ResourceManager.Instance.LoadAssetBundleAsync("lua/xlua");

			OnLoaded();
		}


		private void OnLoaded()
		{
			Log.Info("OnLoaded");
			try
			{
				LuaManager luaMgr = App.Instance.GetManager<LuaManager>();
				luaMgr.InitStart();
				luaMgr.StartMain();

				var luaTable = luaMgr.GetTable("Game");
				var fun = luaTable.GetInPath<XLua.LuaFunction>("OnInitOK");
				fun.Call();
				Log.Info("OnLoaded 2");


				var ret = luaMgr.CallMethod("Game.New", "TestLuaScript");
				// ref to luatable
				luaTable = ret[0] as XLua.LuaTable;

				/*var luaTable = luaMgr.GetTable("Game");
				var fun = luaTable.GetInPath<LuaFunction>("OnInitOK");
				fun.Call();*/

				var func = luaTable.GetInPath<XLua.LuaFunction>("Start");
				func.Call();

				func = luaTable.GetInPath<XLua.LuaFunction>("Update");
				func.Call();
			}
			catch (Exception e)
			{
				Log.Error(e.ToString());
			}
		}

	}

}