
using System;
using System.Collections.Generic;
using UnityEngine;



public class App : MonoBehaviour
{
	private Dictionary<Type, IManager> mManagerDic = new Dictionary<Type, IManager>();


	void Awake()
	{
		try
		{
			Application.runInBackground = true;
			Screen.sleepTimeout = SleepTimeout.NeverSleep;
			Application.targetFrameRate = AppDefine.FPS;

			//AddManager<Hook>();
			AddManager<ResourceManager>();
			AddManager<LuaManager>();
			AddManager<NetworkManager>();

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


	private T AddManager<T>() where T : MonoBehaviour, IManager, new()
	{
		T mgr = null;
		Type name = typeof(T);
		if (!mManagerDic.ContainsKey(name))
		{
			mgr = gameObject.AddComponent<T>();
			mManagerDic.Add(name, mgr);
		}
		return mgr;
	}


	public T GetManager<T>() where T : MonoBehaviour, IManager
	{
		Type name = typeof(T);
		IManager mgr = null;
		mManagerDic.TryGetValue(name, out mgr);
		return mgr as T;
	}

}
