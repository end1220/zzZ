
using System.Collections.Generic;
using System.IO;


namespace Float
{
	public class ModelData
	{

	}

	public class ModelManager
	{
		private Dictionary<long, ModelData> modelDatas = new Dictionary<long, ModelData>();

		public void RebuildModelList()
		{
			string modelPath = "";
			string modelListPath = "";
			FileStream stream = File.Open(modelListPath, FileMode.Create);
			StreamReader reader = new StreamReader(stream);

			string[] subDirs = Directory.GetDirectories(modelPath);
			for (int i = 0; i < subDirs.Length; ++i)
			{
				string subDir = subDirs[i].Replace('\\', '/');
				string dirName = subDir.Substring(subDir.LastIndexOf("/") + 1);
				string sbmPath = subDir + "/" + dirName + ".sbm";

			}
		}

	}

}