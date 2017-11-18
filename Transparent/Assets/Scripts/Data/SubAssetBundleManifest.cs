
using System;
using System.IO;
using System.Collections.Generic;


[Serializable]
public class SubAssetBundleManifest
{
	public List<string> Assets = new List<string>();
	public List<string> Dependencies = new List<string>();

	public void SetUnityManifest(string filePath)
	{
		Assets.Clear();
		Dependencies.Clear();

		FileStream stream = File.Open(filePath, FileMode.Open);
		StreamReader reader = new StreamReader(stream);

		while (!reader.EndOfStream)
		{
			string line = reader.ReadLine();
			if (line.Contains("Assets:"))
				break;
		}
		while (!reader.EndOfStream)
		{
			string line = reader.ReadLine();
			string asset = line.Substring(2);
			Assets.Add(asset);
		}

		while (!reader.EndOfStream)
		{
			string line = reader.ReadLine();
			if (line.Contains("Dependencies:"))
				break;
		}
		while (!reader.EndOfStream)
		{
			string line = reader.ReadLine();
			string asset = line.Substring(2);
			Dependencies.Add(asset);
		}

	}
}
