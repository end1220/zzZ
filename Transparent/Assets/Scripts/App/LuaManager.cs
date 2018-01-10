using UnityEngine;
using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using XLua;


namespace Lite
{

	public class LuaManager : IManager
	{
		public static LuaManager Instance { private set; get; }

		private LuaEnv luaEnv;
		private LuaLoader loader;
		private float updateTime = 0;

		Action tickFunc;

		[CSharpCallLua]
		public delegate object[] LuaFuncCall(params object[] args);

		private Dictionary<string, LuaFuncCall> funcList = new Dictionary<string, LuaFuncCall>();

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
				tickFunc();
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

			luaEnv.Global.Get("Window_OnTick", out tickFunc);
		}

		byte[] CustomLoad(ref string fileName)
		{
			byte[] buffer = FileUtils.Instance.ReadFile(fileName);
			return buffer;
		}

		public object[] DoFile(string fileName)
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

				/*if (AppDefine.openZbsDebugger)
				{
					fileName = FileUtils.Instance.FindFile(fileName);
				}*/

				string strBuffer = Encoding.UTF8.GetString(buffer);
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
			AddSearchPath(AppDefine.PersistentDataPath + "/lua");
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
			if (funcList.ContainsKey(funcName))
			{
				var call = funcList[funcName];
				return call(args);
			}
			else
			{
				LuaFunction func = luaEnv.Global.GetInPath<LuaFunction>(funcName);
				if (func != null)
				{
					return func.Call(args);
				}
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