
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class MyAssetBundleManifest
{
	public List<string> allAssetBundles = new List<string>();
	public Dictionary<string, string[]> allDependencies = new Dictionary<string, string[]>();
	public Dictionary<string, Hash128> allHash = new Dictionary<string, Hash128>();

	public string[] GetAllAssetBundles()
	{
		return allAssetBundles.ToArray();
	}

	public string[] GetAllDependencies(string name)
	{
		string[] val = null;
		allDependencies.TryGetValue(name, out val);
		return val;
	}

	public Hash128 GetAssetBundleHash(string name)
	{
		Hash128 hash = new Hash128();
		allHash.TryGetValue(name, out hash);
		return hash;
	}

	public void AddUnityManifest(AssetBundleManifest manifest)
	{
		if (manifest == null)
			return;
		string[] inOriginList = manifest.GetAllAssetBundles();
		foreach (var origin in inOriginList)
		{
			allAssetBundles.Add(origin);
			allDependencies.Add(origin, manifest.GetAllDependencies(origin));
			allHash.Add(origin, manifest.GetAssetBundleHash(origin));
		}
	}

	public void AddSubManifest(SubAssetBundleManifest manifest)
	{
		if (manifest == null)
			return;
		allAssetBundles.Add(manifest.AssetBundleName);
		allDependencies.Add(manifest.AssetBundleName, manifest.Dependencies.ToArray());
		//allHash.Add(manifest.AssetBundleName, null);
	}

}
