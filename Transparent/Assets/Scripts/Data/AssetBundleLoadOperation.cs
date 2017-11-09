using UnityEngine;
using System;
using System.Collections;


public abstract class AssetBundleLoadOperation : IEnumerator
{
	protected AsyncOperation m_Operation = null;

	public object Current
	{
		get
		{
			return null;
		}
	}
	public bool MoveNext()
	{
		return !IsDone();
	}

	public void Reset()
	{
	}

	public float GetProgress()
	{
		if (m_Operation != null)
		{
			return m_Operation.progress;
		}
		return 0f;
	}

	abstract public bool Update();

	abstract public bool IsDone();
}


#if UNITY_EDITOR
public class AssetBundleLoadLevelSimulationOperation : AssetBundleLoadOperation
{
	public AssetBundleLoadLevelSimulationOperation(string assetBundleName, string levelName, bool isAdditive)
	{
		int index1 = levelName.LastIndexOf("/");
		int len = levelName.LastIndexOf(".") - index1 - 1;
		levelName = levelName.Substring(index1 + 1, len);
		string[] levelPaths = UnityEditor.AssetDatabase.GetAssetPathsFromAssetBundleAndAssetName(assetBundleName, levelName);
		if (levelPaths.Length == 0)
		{
			///@TODO: The error needs to differentiate that an asset bundle name doesn't exist
			//        from that there right scene does not exist in the asset bundle...

			Debug.LogError("There is no scene with name \"" + levelName + "\" in " + assetBundleName);
			return;
		}

		if (isAdditive)
			m_Operation = UnityEditor.EditorApplication.LoadLevelAdditiveAsyncInPlayMode(levelPaths[0]);
		else
			m_Operation = UnityEditor.EditorApplication.LoadLevelAsyncInPlayMode(levelPaths[0]);
	}

	public override bool Update()
	{
		return false;
	}

	public override bool IsDone()
	{
		return m_Operation == null || m_Operation.isDone;
	}
}

#endif


public class AssetBundleLoadLevelOperation : AssetBundleLoadOperation
{
	protected string m_AssetBundleName;
	protected string m_LevelName;
	protected bool m_IsAdditive;
	protected string m_DownloadingError;


	public AssetBundleLoadLevelOperation(string assetbundleName, string levelName, bool isAdditive)
	{
		m_AssetBundleName = assetbundleName;
		m_LevelName = levelName;
		m_IsAdditive = isAdditive;
	}

	public override bool Update()
	{
		if (m_Operation != null)
			return false;

		LoadedAssetBundle bundle = ResourceManager.Instance.GetLoadedAssetBundle(m_AssetBundleName, out m_DownloadingError);
		if (bundle != null)
		{
			if (m_IsAdditive)
				m_Operation = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(m_LevelName, UnityEngine.SceneManagement.LoadSceneMode.Additive);
			else
				m_Operation = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(m_LevelName, UnityEngine.SceneManagement.LoadSceneMode.Single);
			return false;
		}
		else
			return true;
	}

	public override bool IsDone()
	{
		// Return if meeting downloading error.
		// m_DownloadingError might come from the dependency downloading.
		if (m_Operation == null && m_DownloadingError != null)
		{
			Debug.LogError(m_DownloadingError);
			return true;
		}

		return m_Operation != null && m_Operation.isDone;
	}
}


public abstract class AssetBundleLoadAssetOperation : AssetBundleLoadOperation
{
	public abstract T GetAsset<T>() where T : UnityEngine.Object;

	public abstract UnityEngine.Object GetAsset();
}


public class AssetBundleLoadAssetOperationSimulation : AssetBundleLoadAssetOperation
{
	UnityEngine.Object m_SimulatedObject;

	public AssetBundleLoadAssetOperationSimulation(UnityEngine.Object simulatedObject)
	{
		m_SimulatedObject = simulatedObject;
	}

	public override T GetAsset<T>()
	{
		return m_SimulatedObject as T;
	}

	public override UnityEngine.Object GetAsset()
	{
		return m_SimulatedObject;
	}

	public override bool Update()
	{
		return false;
	}

	public override bool IsDone()
	{
		return true;
	}
}

public class AssetBundleLoadAssetOperationFull : AssetBundleLoadAssetOperation
{
	protected string m_AssetBundleName;
	protected string m_AssetName;
	protected string m_DownloadingError;
	protected System.Type m_Type;
	protected AssetBundleRequest m_Request = null;

	public AssetBundleLoadAssetOperationFull(string bundleName, string assetName, System.Type type)
	{
		m_AssetBundleName = bundleName;
		m_AssetName = assetName;
		m_Type = type;
	}

	public override T GetAsset<T>()
	{
		if (m_Request != null && m_Request.isDone)
			return m_Request.asset as T;
		else
			return null;
	}

	public override UnityEngine.Object GetAsset()
	{
		if (m_Request != null && m_Request.isDone)
			return m_Request.asset;
		else
			return null;
	}

	// Returns true if more Update calls are required.
	public override bool Update()
	{
		if (m_Request != null)
			return false;

		LoadedAssetBundle bundle = ResourceManager.Instance.GetLoadedAssetBundle(m_AssetBundleName, out m_DownloadingError);
		if (bundle != null)
		{
			///@TODO: When asset bundle download fails this throws an exception...
			m_Request = bundle.assetBundle.LoadAssetAsync(m_AssetName, m_Type);
			return false;
		}
		else
		{
			return true;
		}
	}

	public override bool IsDone()
	{
		// Return if meeting downloading error.
		// m_DownloadingError might come from the dependency downloading.
		if (m_Request == null && m_DownloadingError != null)
		{
			Debug.LogError(m_DownloadingError);
			return true;
		}

		return m_Request != null && m_Request.isDone;
	}
}

public class AssetBundleLoadManifestOperation : AssetBundleLoadAssetOperationFull
{
	public AssetBundleLoadManifestOperation(string bundleName, string assetName, System.Type type)
		: base(bundleName, assetName, type)
	{
	}

	public override bool Update()
	{
		base.Update();

		if (m_Request != null && m_Request.isDone)
		{
			ResourceManager.Instance.AssetBundleManifestObject = GetAsset<AssetBundleManifest>();
			return false;
		}
		else
			return true;
	}
}


public class AssetBundleLoadOperationPure : AssetBundleLoadOperation
{
	protected string m_AssetBundleName;
	protected string m_DownloadingError;

	protected LoadedAssetBundle m_bundle;
	public LoadedAssetBundle Bundle { get { return m_bundle; } }


	public AssetBundleLoadOperationPure(string bundleName)
	{
		m_AssetBundleName = bundleName;
	}

	// Returns true if more Update calls are required.
	public override bool Update()
	{
		if (m_bundle != null)
			return false;

		m_bundle = ResourceManager.Instance.GetLoadedAssetBundle(m_AssetBundleName, out m_DownloadingError);
		return m_bundle == null;
	}

	public override bool IsDone()
	{
		// Return if meeting downloading error.
		// m_DownloadingError might come from the dependency downloading.
		if (m_bundle == null && m_DownloadingError != null)
		{
			Debug.LogError(m_DownloadingError);
			return true;
		}

		return m_bundle != null && string.IsNullOrEmpty(m_DownloadingError);
	}
}

