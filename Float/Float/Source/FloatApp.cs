using System;
using System.Collections.Generic;


namespace Float
{

	public class FloatApp
	{
		public static FloatApp Instance { get; private set; }

		private Dictionary<Type, IManager> mManagerDic = new Dictionary<Type, IManager>();

		public static MsgSystem MsgSystem { get; private set; }

		public static ModelDataManager DataManager { get; private set; }

		public FloatApp()
		{
			Instance = this;
		}

		public void Init()
		{
			try
			{
				CustomSettings.Load();
				MsgSystem = AddManager<MsgSystem>();
				AddManager<NetworkManager>();
				DataManager = AddManager<ModelDataManager>();
				AddManager<SteamManager>();

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

		private T GetManager<T>() where T : IManager
		{
			Type name = typeof(T);
			IManager mgr = null;
			mManagerDic.TryGetValue(name, out mgr);
			return mgr as T;
		}

	}
}
