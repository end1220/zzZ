
using System.Collections.Generic;
using System.IO;
using System.CodeDom.Compiler;
using Microsoft.CSharp;
using System.Text;
using UnityEngine;
using UnityEditor;
using Newtonsoft.Json;


public class CompileDll
{
	[System.Serializable]
	class CompileConfig
	{
		public string OutputPath;
		public string[] SourcePath;
		public string[] Dlls;
	}

	static string runtimeConfigPath = System.Environment.CurrentDirectory + "/Assets/Editor/CompileConfig.json";
	static string editorConfigPath = System.Environment.CurrentDirectory + "/Assets/Editor/CompileConfig.json";

	[MenuItem(AppDefine.AppName + "/Runtime Dll", false, 3)]
	static void CompileRuntimeDll()
	{
		string cfgText = File.ReadAllText(runtimeConfigPath);
		CompileConfig cfg = JsonConvert.DeserializeObject<CompileConfig>(cfgText);
		List<string> sourceFiles = new List<string>();
		for (int i = 0; i < cfg.SourcePath.Length; ++i)
		{
			string path = System.Environment.CurrentDirectory + "/" + cfg.SourcePath[i];
			if (Directory.Exists(path))
				GetSourceFiles(sourceFiles, path);
			else
				Debug.LogError(path + " not exist...");
		}
		string[] sources = sourceFiles.ToArray();
		Compile(cfg.Dlls, sources, cfg.OutputPath);
	}

	[MenuItem(AppDefine.AppName + "/Editor Dll", false, 3)]
	static void CompileEditorDll()
	{
		string cfgText = File.ReadAllText(editorConfigPath);
		CompileConfig cfg = JsonConvert.DeserializeObject<CompileConfig>(cfgText);
		Compile(cfg.Dlls, cfg.SourcePath, cfg.OutputPath);
	}

	static void GetSourceFiles(List<string> sourceFiles, string directory)
	{
		string[] files = Directory.GetFiles(directory);
		for (int i = 0; i < files.Length; ++i)
		{
			if (!files[i].Contains(".meta") && files[i].Contains(".cs"))
				sourceFiles.Add(files[i]);
		}
		string[] dirs = Directory.GetDirectories(directory);
		for (int i = 0; i < dirs.Length; ++i)
		{
			GetSourceFiles(sourceFiles, dirs[i]);
		}
	}


	static void Compile(string[] references, string[] sources, string outputfile)
	{
		CompilerParameters param = new CompilerParameters(references, outputfile, true);
		param.TreatWarningsAsErrors = false;
		param.GenerateExecutable = false;
		param.IncludeDebugInformation = false;
		//param.EmbeddedResources.Add(@"C:\1.txt");
		//param.MainClass = "return2";

		CSharpCodeProvider provider = new CSharpCodeProvider();
		CompilerResults result = provider.CompileAssemblyFromFile(param, sources);

		StringBuilder sb = new StringBuilder();
		foreach (CompilerError error in result.Errors)
		{
			sb.Remove(0, sb.Length);
			sb.Append(error.IsWarning ? "Warning" : "Error");
			sb.Append("(" + error.ErrorNumber + ") - " + error.ErrorText + "\t\tLine:" + error.Line.ToString() + ", Column:" + error.Column.ToString());
			Debug.Log(sb.ToString());
		}
	}

}