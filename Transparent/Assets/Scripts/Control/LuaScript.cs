
using System;
using UnityEngine;
using XLua;


namespace Lite
{

	public class LuaScript : MonoBehaviour
	{
		public string ModuleName;

		private LuaFunction luaStart;
		private LuaFunction luaUpdate;
		private LuaFunction luaOnDestroy;

		private LuaTable luaTable;


		void Awake()
		{
			LuaEnv luaEnv = LuaManager.Instance.luaEnv;

			luaTable = luaEnv.NewTable();

			LuaTable meta = luaEnv.NewTable();
			meta.Set("__index", luaEnv.Global);
			luaTable.SetMetaTable(meta);
			meta.Dispose();

			luaTable.Set("self", this);

			LuaManager.Instance.DoFile(ModuleName, luaTable);

			LuaFunction luaAwake = luaTable.Get<LuaFunction>("Awake");
			luaTable.Get("Start", out luaStart);
			luaTable.Get("Update", out luaUpdate);
			luaTable.Get("OnDestroy", out luaOnDestroy);

			luaAwake = luaTable.GetInPath<LuaFunction>("Awake");
			luaStart = luaTable.GetInPath<LuaFunction>("Start");
			luaUpdate = luaTable.GetInPath<LuaFunction>("Update");

			if (luaAwake != null)
			{
				luaAwake.Call();
			}
		}

		void Start()
		{
			if (luaStart != null)
			{
				luaStart.Call();
			}
		}

		void Update()
		{
			if (luaUpdate != null)
			{
				luaUpdate.Call();
			}
		}

		void OnDestroy()
		{
			if (luaOnDestroy != null)
			{
				luaOnDestroy.Call();
			}
			luaOnDestroy = null;
			luaUpdate = null;
			luaStart = null;
			luaTable.Dispose();
			//injections = null;
		}

	}

}
