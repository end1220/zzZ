
using System;
using UnityEngine;
using XLua;


namespace Float
{

	public class LuaScript : MonoBehaviour
	{
		public string ModuleName;

		private Action luaStart;
		private Action luaUpdate;
		private Action luaOnDestroy;

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

			Action luaAwake = luaTable.Get<Action>("Awake");
			luaTable.Get("Start", out luaStart);
			luaTable.Get("Update", out luaUpdate);
			luaTable.Get("OnDestroy", out luaOnDestroy);

			if (luaAwake != null)
			{
				luaAwake();
			}
		}

		void Start()
		{
			if (luaStart != null)
			{
				luaStart();
			}
		}

		void Update()
		{
			if (luaUpdate != null)
			{
				luaUpdate();
			}
		}

		void OnDestroy()
		{
			if (luaOnDestroy != null)
			{
				luaOnDestroy();
			}
			luaOnDestroy = null;
			luaUpdate = null;
			luaStart = null;
			luaTable.Dispose();
			//injections = null;
		}

	}

}
