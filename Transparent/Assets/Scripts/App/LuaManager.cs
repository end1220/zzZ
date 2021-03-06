﻿using UnityEngine;
using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using XLua;


namespace Float
{

	public class LuaManager : IManager
	{
		public static LuaManager Instance { private set; get; }

		public LuaEnv luaEnv { get; private set; }
		private LuaLoader loader;
		private float updateTime = 0;

		LuaFunction tickFunc;

		public override void Init()
		{
			Instance = this;
			loader = new LuaLoader();
			luaEnv = new LuaEnv();
			luaEnv.AddLoader(CustomLoad);
		}

		public override void Tick()
		{
			if (tickFunc != null)
			{
				tickFunc.Call();
			}

			if (Time.timeSinceLevelLoad - updateTime > 60)
			{
				luaEnv.Tick();
				updateTime = Time.timeSinceLevelLoad;
			}
		}

		public override void Destroy()
		{
			Close();
		}

		public void InitStart()
		{
			InitLuaPath();
			InitLuaBundle();
		}


		public void StartMain()
		{
			DoFile("Game");

			var luaTable = GetTable("Game");
			var fun = luaTable.GetInPath<LuaFunction>("OnInit");
			fun.Call();

			tickFunc = luaTable.GetInPath<LuaFunction>("OnUpdate");
		}

		byte[] CustomLoad(ref string fileName)
		{
			byte[] buffer = FileUtils.Instance.ReadFile(fileName);
			return buffer;
		}

		public object[] DoFile(string fileName, LuaTable table = null)
		{
			try
			{
				byte[] buffer = FileUtils.Instance.ReadFile(fileName);

				if (buffer == null)
				{
					string error = string.Format("cannot open {0}: No such file or directory", fileName);
					error += FileUtils.Instance.FindFileError(fileName);
					Log.Error(error);
					return null;
				}

				string strBuffer = Encoding.UTF8.GetString(buffer);
				if (table != null)
					return luaEnv.DoString(strBuffer, fileName, table);
				else
					return luaEnv.DoString(strBuffer, fileName);
			}
			catch (System.Exception e)
			{
				Log.Error(e.ToString());
			}
			return null;
		}


		void OpenLibs()
		{
		}


		void InitLuaPath()
		{
#if UNITY_EDITOR
			if (ResourceManager.SimulateAssetBundleInEditor)
			{
				AddSearchPath(Application.dataPath + "/Lua");
				return;
			}
#endif
			AddSearchPath(AppConst.PersistentDataPath + "/lua");
		}

		public void AddSearchPath(string fullPath)
		{
			if (!Path.IsPathRooted(fullPath))
			{
				throw new LuaException(fullPath + " is not a full path");
			}

			fullPath = ToPackagePath(fullPath);
			FileUtils.Instance.AddSearchPath(fullPath);
		}

		string ToPackagePath(string path)
		{
			StringBuilder sb = StringBuilderCache.Acquire();
			sb.Append(path);
			sb.Replace('\\', '/');

			if (sb.Length > 0 && sb[sb.Length - 1] != '/')
			{
				sb.Append('/');
			}

			sb.Append("?.lua");
			return StringBuilderCache.GetStringAndRelease(sb);
		}


		void InitLuaBundle()
		{
			if (loader.beBundle)
			{
				loader.AddBundle("lua/common");
				loader.AddBundle("lua/lua");
				loader.AddBundle("lua/xlua");
			}
		}

		public static object[] CallLuaMethod(string func, params object[] args)
		{
			return Instance.CallMethod(func, args);
		}

		public object[] CallMethod(string funcName, params object[] args)
		{
			LuaFunction func = luaEnv.Global.GetInPath<LuaFunction>(funcName);
			if (func != null)
			{
				return func.Call(args);
			}
			return null;
		}

		public LuaTable GetTable(string tableName)
		{
			LuaTable table = luaEnv.Global.Get<LuaTable>(tableName);
			return table;
		}


		public void LuaGC()
		{
			luaEnv.FullGc();
		}

		public void Close()
		{
			//luaEnv.Dispose();
			luaEnv = null;
			loader = null;
		}

	}

}