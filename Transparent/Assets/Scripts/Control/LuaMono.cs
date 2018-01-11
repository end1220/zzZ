
using UnityEngine;
using XLua;


namespace Lite
{

	public class LuaMono : MonoBehaviour
	{
		public string ModuleName;

		public LuaTable luaTable { get; private set; }

		private LuaFunction awakeFunc;
		private LuaFunction startFunc;
		private LuaFunction enableFunc;
		private LuaFunction disableFunc;
		private LuaFunction destroyFunc;
		private LuaFunction clickFunc;


		private void Awake()
		{
			object[] objs = LuaManager.Instance.DoFile(ModuleName);
			if (objs != null)
			{
				luaTable = objs[0] as LuaTable;
				if (luaTable != null)
				{
					luaTable.Set("self", this);
					GetFunction();
				}
				else
				{
					Log.Error("LuaScript.Awake: " + ModuleName + " is not a class.");
				}
				if (awakeFunc != null)
					awakeFunc.Call(luaTable, gameObject);
			}
			else
			{
				Log.Error("LuaScript.Awake: Cannot find module " + ModuleName);
			}
		}

		protected void GetFunction()
		{
			awakeFunc = luaTable.GetInPath<LuaFunction>("Awake");
			startFunc = luaTable.GetInPath<LuaFunction>("Start");
			enableFunc = luaTable.GetInPath<LuaFunction>("OnEnable");
			disableFunc = luaTable.GetInPath<LuaFunction>("OnDisable");
			destroyFunc = luaTable.GetInPath<LuaFunction>("OnDestroy");
			clickFunc = luaTable.GetInPath<LuaFunction>("OnClick");
		}

		private void Start()
		{
			if (startFunc != null)
				startFunc.Call(luaTable);
		}

		private void OnEnable()
		{
			if (enableFunc != null)
				enableFunc.Call(luaTable);
		}

		private void OnDisable()
		{
			if (disableFunc != null)
				disableFunc.Call(luaTable);
		}

		private void OnClick()
		{
			if (clickFunc != null)
				clickFunc.Call(luaTable);
		}

		private void OnDestroy()
		{
			if (destroyFunc != null)
				destroyFunc.Call(luaTable);
		}

	}

}
