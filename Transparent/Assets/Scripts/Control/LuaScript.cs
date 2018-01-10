
using UnityEngine;
using XLua;


namespace Lite
{

	public class LuaScript : MonoBehaviour
	{
		public string luaPath;

		private LuaTable luaTable;


		private void Awake()
		{
			var ret = LuaManager.Instance.CallMethod("Game.New", luaPath);
			if (ret != null)
			{
				luaTable = ret[0] as LuaTable;
			}
			else
			{
				Log.Error("LuaScript.Awake: Cannot load lua " + luaPath);
			}
		}

		private void Start()
		{
			if (luaTable != null)
			{
				var func = luaTable.GetInPath<LuaFunction>("Start");
				if (func != null)
					func.Call();
				else
					Log.Error("LuaScript.Start: Cannot find function with name Start in " + luaPath);
			}
		}

		private void Update()
		{
			if (luaTable != null)
			{
				var func = luaTable.GetInPath<LuaFunction>("Update");
				if (func != null)
					func.Call();
				else
					Log.Error("LuaScript.Start: Cannot find function with name Update in " + luaPath);
			}
		}

	}

}
