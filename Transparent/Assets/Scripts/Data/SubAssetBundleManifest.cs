
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SubAssetBundleManifest
{
	public List<string> Assets = new List<string>();
	public List<string> Dependencies = new List<string>();

	public void AddUnityManifest(string manifest)
	{
		
	}
}
