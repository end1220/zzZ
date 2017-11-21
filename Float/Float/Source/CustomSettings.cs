
using System.IO;
using Newtonsoft.Json;


public class CustomSettings
{
	public static CustomSettings Current { get; private set; }
	
	private static string settingFilePath = System.Environment.CurrentDirectory + "/settings.json";

	public string ModelPath = System.Environment.CurrentDirectory + "/Models";
	public bool StartEnabled = true;
	public float scale = 1;

	public static CustomSettings Load()
	{
		string path = settingFilePath;
		CustomSettings settings = null;
		if (Directory.Exists(path))
			settings = JsonConvert.DeserializeObject<CustomSettings>(path);
		else
			settings = new CustomSettings();
		Current = settings;
		return settings;
	}

	public static void Save()
	{
		string path = settingFilePath;
		string settings = JsonConvert.SerializeObject(Current, Formatting.Indented);
		File.WriteAllText(path, settings);
	}
}
