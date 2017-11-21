using System;
using System.Collections.Generic;


namespace Float
{

	public class FloatDecade
	{
		private static FloatDecade _inst;
		public static FloatDecade Instance { get { if (_inst == null) _inst = new FloatDecade(); return _inst; } }
		private Dictionary<Type, IManager> mManagerDic = new Dictionary<Type, IManager>();

		public void Init()
		{
			try
			{
				CustomSettings.Load();
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

		public void Tick()
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

		public void Destory()
		{
			try
			{
				CustomSettings.Save();
				foreach (var item in mManagerDic)
					item.Value.Destroy();
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
}
