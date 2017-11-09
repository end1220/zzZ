
using System;
using System.Collections.Generic;
using UnityEngine;



public class App : MonoBehaviour
{
	private Dictionary<Type, IManager> mManagerDic = new Dictionary<Type, IManager>();


	void Awake()
	{
		Screen.sleepTimeout = SleepTimeout.NeverSleep;
		Application.targetFrameRate = AppDefine.FPS;

		this.AddManager<ResourceManager>();
		this.AddManager<LuaManager>();
		this.AddManager<NetworkManager>();

		foreach (var mgr in mManagerDic.Values)
			mgr.Init();
	}


	void OnDestroy()
	{
		foreach (var mgr in mManagerDic.Values)
		{
			mgr.Destroy();
		}
	}


	void Update()
	{
		var itor = mManagerDic.GetEnumerator();
		while (itor.MoveNext())
			itor.Current.Value.Tick();
	}


	private T AddManager<T>() where T : MonoBehaviour, IManager, new()
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


	public T GetManager<T>() where T : MonoBehaviour, IManager
	{
		Type name = typeof(T);
		IManager mgr = null;
		mManagerDic.TryGetValue(name, out mgr);
		return mgr as T;
	}

}
