
using System;
using System.Collections.Generic;
using UnityEngine;



public class App : MonoBehaviour
{
	public static App Instance { private set; get; }

	private Dictionary<Type, IManager> mManagerDic = new Dictionary<Type, IManager>();


	void Awake()
	{
		try
		{
			Instance = this;
			Application.runInBackground = true;
			Screen.sleepTimeout = SleepTimeout.NeverSleep;
			Application.targetFrameRate = AppDefine.FPS;
			Screen.SetResolution(1024, 768, false);

			AddManager<ResourceManager>();
			AddManager<LuaManager>();
			AddManager<NetworkManager>();
			AddManager<DataManager>();

			foreach (var item in mManagerDic)
				item.Value.Init();
		}
		catch (Exception e)
		{
			Log.Error(e.ToString());
		}
	}


	void OnDestroy()
	{
		try
		{
			foreach (var item in mManagerDic)
				item.Value.Destroy();
		}
		catch (Exception e)
		{
			Log.Error(e.ToString());
		}
	}


	void Update()
	{
		try
		{
			foreach (var item in mManagerDic)
				item.Value.Tick();
		}
		catch (Exception e)
		{
			Log.Error(e.ToString());
		}
	}


	private T AddManager<T>() where T : IManager, new()
	{
		T mgr = null;
		Type name = typeof(T);
		if (!mManagerDic.ContainsKey(name))
		{
			mgr = new T();
			mManagerDic.Add(name, mgr);
		}
		return mgr;
	}


	public T GetManager<T>() where T : IManager
	{
		Type name = typeof(T);
		IManager mgr = null;
		mManagerDic.TryGetValue(name, out mgr);
		return mgr as T;
	}

}
