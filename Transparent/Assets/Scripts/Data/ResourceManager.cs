using UnityEngine;
using System.Collections.Generic;
using System;
using System.IO;
using Newtonsoft.Json;

using UObject = UnityEngine.Object;


namespace Lite
{

	public class LoadedAssetBundle
{
	public AssetBundle assetBundle;
	public int referencedCount;

	public LoadedAssetBundle(AssetBundle assetBundle)
	{
		this.assetBundle = assetBundle;
		this.referencedCount = 1;
	}
}


	public class ResourceManager : IManager
	{
		string m_BaseDownloadingURL = "Assets/StreamingAssets/";

		public MyAssetBundleManifest manifest;

		Dictionary<string, LoadedAssetBundle> m_LoadedAssetBundles = new Dictionary<string, LoadedAssetBundle>();
		Dictionary<string, WWW> m_DownloadingWWWs = new Dictionary<string, WWW>();
		Dictionary<string, string> m_DownloadingErrors = new Dictionary<string, string>();
		List<AssetBundleLoadOperation> m_InProgressOperations = new List<AssetBundleLoadOperation>();
		Dictionary<string, string[]> m_Dependencies = new Dictionary<string, string[]>();

#if UNITY_EDITOR
		static int m_SimulateAssetBundleInEditor = -1;
		const string kSimulateAssetBundles = "SimulateAssetBundles";
		public static bool SimulateAssetBundleInEditor
		{
			get
			{
				if (m_SimulateAssetBundleInEditor == -1)
					m_SimulateAssetBundleInEditor = UnityEditor.EditorPrefs.GetBool(kSimulateAssetBundles, true) ? 1 : 0;

				return m_SimulateAssetBundleInEditor != 0;
			}
			set
			{
				int newValue = value ? 1 : 0;
				if (newValue != m_SimulateAssetBundleInEditor)
				{
					m_SimulateAssetBundleInEditor = newValue;
					UnityEditor.EditorPrefs.SetBool(kSimulateAssetBundles, value);
				}
			}
		}
#endif

		public static ResourceManager Instance { private set; get; }

		public override void Init()
		{
			Instance = this;
		}

		public override void Tick()
		{
			UpdateLoading();
		}

		public void Initialize()
		{
			m_BaseDownloadingURL = "file://" + AppConst.PersistentDataPath;// = "http://www.MyWebsite/MyAssetBundles";

#if UNITY_EDITOR
			if (SimulateAssetBundleInEditor)
				return;
#endif
			string txt = File.ReadAllText(AppConst.manifestPath);
			manifest = JsonConvert.DeserializeObject<MyAssetBundleManifest>(txt);
		}

		public LoadedAssetBundle GetLoadedAssetBundle(string assetBundleName, out string error)
		{
			if (m_DownloadingErrors.TryGetValue(assetBundleName, out error))
				return null;

			LoadedAssetBundle bundle = null;
			m_LoadedAssetBundles.TryGetValue(assetBundleName, out bundle);
			if (bundle == null)
				return null;

			// No dependencies are recorded, only the bundle itself is required.
			string[] dependencies = null;
			if (!m_Dependencies.TryGetValue(assetBundleName, out dependencies))
				return bundle;

			// Make sure all dependencies are loaded
			foreach (var dependency in dependencies)
			{
				if (m_DownloadingErrors.TryGetValue(assetBundleName, out error))
					return bundle;

				// Wait all the dependent assetBundles being loaded.
				LoadedAssetBundle dependentBundle;
				m_LoadedAssetBundles.TryGetValue(dependency, out dependentBundle);
				if (dependentBundle == null)
					return null;
			}

			return bundle;
		}

		private List<string> keysToRemove = new List<string>();
		void UpdateLoading()
		{
			foreach (var keyValue in m_DownloadingWWWs)
			{
				string key = keyValue.Key;
				WWW download = keyValue.Value;

				if (download.error != null)
				{
					m_DownloadingErrors.Add(key, string.Format("Failed downloading bundle {0} from {1}: {2}", key, download.url, download.error));
					keysToRemove.Add(key);
					continue;
				}

				if (download.isDone)
				{
					if (download.assetBundle == null)
					{
						m_DownloadingErrors.Add(key, string.Format("{0} is not a valid asset bundle.", key));
						keysToRemove.Add(key);
						continue;
					}

					m_LoadedAssetBundles.Add(key, new LoadedAssetBundle(download.assetBundle));
					keysToRemove.Add(key);
				}
			}

			// Remove the finished WWWs.
			for (int i = 0; i < keysToRemove.Count; ++i)
			{
				var key = keysToRemove[i];
				WWW download = m_DownloadingWWWs[key];
				m_DownloadingWWWs.Remove(key);
				download.Dispose();
			}
			keysToRemove.Clear();

			// Update all in progress operations
			for (int i = 0; i < m_InProgressOperations.Count;)
			{
				if (!m_InProgressOperations[i].Update())
				{
					m_InProgressOperations.RemoveAt(i);
				}
				else
					i++;
			}
		}


		public T LoadAsset<T>(string assetBundleName, string assetName) where T : UnityEngine.Object
		{
			string path = AppConst.PersistentDataPath + assetBundleName;

#if UNITY_EDITOR
			if (SimulateAssetBundleInEditor)
			{
				var target = UnityEditor.AssetDatabase.LoadAssetAtPath<T>(assetName);
				return target;
			}
			else
#endif
			{
				AssetBundle assetbundle = null;
				string err;
				LoadedAssetBundle bundle = GetLoadedAssetBundle(assetBundleName, out err);
				if (bundle == null)
				{
					assetbundle = AssetBundle.LoadFromFile(path);
					if (assetbundle != null)
						m_LoadedAssetBundles.Add(assetBundleName, new LoadedAssetBundle(assetbundle));

					if (manifest == null)
					{
						Log.Error("Please initialize AssetBundleManifest by calling ResourceManager.Initialize()");
					}
					else
					{
						string[] dependencies = manifest.GetAllDependencies(assetBundleName);
						for (int i = 0; i < dependencies.Length; i++)
						{
							LoadAssetBundleSync(dependencies[i]);
						}

						m_Dependencies.Add(assetBundleName, dependencies);
					}
				}
				else
				{
					assetbundle = bundle.assetBundle;
				}
				if (assetbundle == null)
				{
					Log.Error("ResourceManager.LoadAsset: file not exist: " + path);
					return null;
				}
				var target = assetbundle.LoadAsset<T>(assetName);
				return target;
			}
		}


		public AssetBundleLoadAssetOperation LoadAssetAsync<T>(string assetBundleName, string assetName)
		{
			return LoadAssetAsync(assetBundleName, assetName, typeof(T));
		}


		public AssetBundleLoadAssetOperation LoadAssetAsync(string assetBundleName, string assetName, Type type)
		{
			//Log.Info("Loading " + assetName + " from " + assetBundleName + " bundle");

			AssetBundleLoadAssetOperation operation = null;
#if UNITY_EDITOR
			if (SimulateAssetBundleInEditor)
			{
				int index1 = assetName.LastIndexOf("/");
				int len = assetName.LastIndexOf(".") - index1 - 1;
				string assetNameShort = assetName.Substring(index1 + 1, len);
				string[] assetPaths = UnityEditor.AssetDatabase.GetAssetPathsFromAssetBundleAndAssetName(assetBundleName, assetNameShort);
				if (assetPaths.Length == 0)
				{
					Log.Error("There is no asset with name \"" + assetName + "\" in " + assetBundleName);
					return null;
				}

				UObject target = UnityEditor.AssetDatabase.LoadMainAssetAtPath(assetName);
				operation = new AssetBundleLoadAssetOperationSimulation(target);
			}
			else
#endif
			{
				LoadAssetBundle(assetBundleName, false);
				operation = new AssetBundleLoadAssetOperationFull(assetBundleName, assetName, type);

				m_InProgressOperations.Add(operation);
			}

			return operation;
		}


		public AssetBundleLoadOperation LoadLevelAsync(string assetBundleName, string levelName, bool isAdditive)
		{
			//Log.Info("Loading " + levelName + " from " + assetBundleName + " bundle");

			AssetBundleLoadOperation operation = null;
#if UNITY_EDITOR
			if (SimulateAssetBundleInEditor)
			{
				operation = new AssetBundleLoadLevelSimulationOperation(assetBundleName, levelName, isAdditive);
			}
			else
#endif
			{
				LoadAssetBundle(assetBundleName, false);
				operation = new AssetBundleLoadLevelOperation(assetBundleName, levelName, isAdditive);

				m_InProgressOperations.Add(operation);
			}

			return operation;
		}


		private void LoadAssetBundle(string assetBundleName, bool isLoadingAssetBundleManifest)
		{
			//Log.Info("Loading Asset Bundle " + (isLoadingAssetBundleManifest ? "Manifest: " : ": ") + assetBundleName);

#if UNITY_EDITOR
			if (SimulateAssetBundleInEditor)
				return;
#endif

			if (!isLoadingAssetBundleManifest)
			{
				if (manifest == null)
				{
					Log.Error("Please initialize AssetBundleManifest by calling ResourceManager.Initialize()");
					return;
				}
			}

			// Check if the assetBundle has already been processed.
			bool isAlreadyProcessed = LoadAssetBundleInternal(assetBundleName, isLoadingAssetBundleManifest);

			// Load dependencies.
			if (!isAlreadyProcessed && !isLoadingAssetBundleManifest)
				LoadDependencies(assetBundleName);
		}


		public AssetBundle LoadAssetBundleSync(string assetBundleName)
		{
			string path = AppConst.PersistentDataPath + assetBundleName;
#if UNITY_EDITOR
			if (SimulateAssetBundleInEditor)
			{
				return null;
			}
			else
#endif
			{
				string err;
				AssetBundle assetbundle = null;
				LoadedAssetBundle loadedBundle = GetLoadedAssetBundle(assetBundleName, out err);
				if (loadedBundle != null)
				{
					return loadedBundle.assetBundle;
				}
				else
				{
					assetbundle = AssetBundle.LoadFromFile(path);
					if (assetbundle != null)
						m_LoadedAssetBundles.Add(assetBundleName, new LoadedAssetBundle(assetbundle));

					if (manifest == null)
					{
						Log.Error("Please initialize AssetBundleManifest by calling ResourceManager.Initialize()");
					}
					else
					{
						string[] dependencies = manifest.GetAllDependencies(assetBundleName);
						for (int i = 0; i < dependencies.Length; i++)
						{
							LoadAssetBundleSync(dependencies[i]);
						}

						m_Dependencies.Add(assetBundleName, dependencies);
					}

					return assetbundle;
				}
			}
		}


		public AssetBundleLoadOperationPure LoadAssetBundleAsync(string assetBundleName)
		{
#if UNITY_EDITOR
			if (SimulateAssetBundleInEditor)
			{
				return null;
			}
			else
#endif
			{
				if (manifest == null)
				{
					Log.Error("Please initialize AssetBundleManifest by calling ResourceManager.Initialize()");
					return null;
				}

				// Check if the assetBundle has already been processed.
				bool isAlreadyProcessed = LoadAssetBundleInternal(assetBundleName, false);
				if (!isAlreadyProcessed)
					LoadDependencies(assetBundleName);

				AssetBundleLoadOperationPure operation = null;
				operation = new AssetBundleLoadOperationPure(assetBundleName);
				m_InProgressOperations.Add(operation);
				return operation;
			}
		}


		protected bool LoadAssetBundleInternal(string assetBundleName, bool isLoadingAssetBundleManifest)
		{
			// Already loaded.
			LoadedAssetBundle bundle = null;
			m_LoadedAssetBundles.TryGetValue(assetBundleName, out bundle);
			if (bundle != null)
			{
				bundle.referencedCount++;
				return true;
			}

			// @TODO: Do we need to consider the referenced count of WWWs?
			// In the demo, we never have duplicate WWWs as we wait LoadAssetAsync()/LoadLevelAsync() to be finished before calling another LoadAssetAsync()/LoadLevelAsync().
			// But in the real case, users can call LoadAssetAsync()/LoadLevelAsync() several times then wait them to be finished which might have duplicate WWWs.
			if (m_DownloadingWWWs.ContainsKey(assetBundleName))
				return true;

			WWW download = null;
			string url = m_BaseDownloadingURL + assetBundleName;

			// For manifest assetbundle, always download it as we don't have hash for it.
			if (isLoadingAssetBundleManifest)
				download = new WWW(url);
			else
				download = WWW.LoadFromCacheOrDownload(url, manifest.GetAssetBundleHash(assetBundleName), 0);

			m_DownloadingWWWs.Add(assetBundleName, download);

			return false;
		}


		protected void LoadDependencies(string assetBundleName)
		{
			if (manifest == null)
			{
				Log.Error("Please initialize AssetBundleManifest by calling ResourceManager.Initialize()");
				return;
			}

			// Get dependecies from the AssetBundleManifest object..
			string[] dependencies = manifest.GetAllDependencies(assetBundleName);
			if (dependencies.Length == 0)
				return;

			// Record and load all dependencies.
			m_Dependencies.Add(assetBundleName, dependencies);
			for (int i = 0; i < dependencies.Length; i++)
				LoadAssetBundleInternal(dependencies[i], false);
		}


		public void UnloadAssetBundle(string assetBundleName)
		{
#if UNITY_EDITOR
			if (SimulateAssetBundleInEditor)
				return;
#endif
			UnloadAssetBundleInternal(assetBundleName);
			UnloadDependencies(assetBundleName);

			//Log.Info(m_LoadedAssetBundles.Count + " assetbundle(s) in memory after unloading " + assetBundleName);
		}

		protected void UnloadDependencies(string assetBundleName)
		{
			string[] dependencies = null;
			if (!m_Dependencies.TryGetValue(assetBundleName, out dependencies))
				return;

			foreach (var dependency in dependencies)
			{
				UnloadAssetBundleInternal(dependency);
			}

			m_Dependencies.Remove(assetBundleName);
		}

		protected void UnloadAssetBundleInternal(string assetBundleName)
		{
			string error;
			LoadedAssetBundle bundle = GetLoadedAssetBundle(assetBundleName, out error);
			if (bundle == null)
				return;

			if (--bundle.referencedCount == 0)
			{
				bundle.assetBundle.Unload(false);
				m_LoadedAssetBundles.Remove(assetBundleName);

				//Log.Info(assetBundleName + " has been unloaded successfully");
			}
		}


		public void Dump()
		{
			Debug.Log("------begin dump------");
			Debug.Log("Total asset bundle in memory: " + m_LoadedAssetBundles.Count);
			foreach (var item in m_LoadedAssetBundles)
			{
				Debug.Log(item.Key);
			}
			Debug.Log("------end dump------");
		}

	}

}

