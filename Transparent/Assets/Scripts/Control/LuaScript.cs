﻿
using UnityEngine;
using XLua;


public class LuaScript : MonoBehaviour
{
	public string luaPath;
	public LuaTable luaTable;
	private LuaManager luaMgr;

	private void Awake()
	{
		luaMgr = LuaManager.Instance;

		var ret = CallMethod("Require", luaPath);
		// ref to luatable
		luaTable = ret[0] as LuaTable;

		/*var luaTable = luaMgr.GetTable("Game");
		var fun = luaTable.GetInPath<LuaFunction>("OnInitOK");
		fun.Call();*/
	}

	private void Start()
	{
		var func = luaTable.GetInPath<LuaFunction>("Start");
		func.Call();
	}

	private void Update()
	{
		var func = luaTable.GetInPath<LuaFunction>("Update");
		func.Call();
	}

	public static object[] CallMethod(string func, params object[] args)
	{
		return LuaManager.Instance.CallMethod(func, args);
	}

}
