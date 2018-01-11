
using System;
using System.Collections;
using UnityEngine;


namespace Lite
{

	public class StartUp : MonoBehaviour
	{
		public static StartUp Instance { get; private set; }


		void Awake()
		{
			Instance = this;
			StartCoroutine(LoadResAndStart());
		}

		IEnumerator LoadResAndStart()
		{
			yield return new WaitForEndOfFrame();

			ResourceManager.Instance.Initialize();

			yield return ResourceManager.Instance.LoadAssetBundleAsync("lua/common");
			yield return ResourceManager.Instance.LoadAssetBundleAsync("lua/lua");
			yield return ResourceManager.Instance.LoadAssetBundleAsync("lua/xlua");

			OnLoaded();
		}


		private void OnLoaded()
		{
			try
			{
				LuaManager luaMgr = App.Instance.GetManager<LuaManager>();
				luaMgr.InitStart();
				luaMgr.StartMain();
			}
			catch (Exception e)
			{
				Log.Error(e.ToString());
			}
		}

	}

}