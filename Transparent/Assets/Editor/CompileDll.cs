
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
		public string[] SourcePath;
		public string[] Dlls;
		public string[] LibPath;
		public string OutputPath;
	}

	static string runtimeConfigPath = System.Environment.CurrentDirectory + "Assets/Editor/CompileConfig.json";
	static string editorConfigPath = System.Environment.CurrentDirectory + "Assets/Editor/CompileConfig.json";

	[MenuItem(AppDefine.AppName + "/Runtime Dll", false, 3)]
	static void CompileRuntimeDll()
	{
		CompileConfig cfg = JsonConvert.DeserializeObject<CompileConfig>(runtimeConfigPath);
		Compile(cfg.Dlls, cfg.SourcePath, cfg.OutputPath);
	}

	[MenuItem(AppDefine.AppName + "/Editor Dll", false, 3)]
	static void CompileEditorDll()
	{
		CompileConfig cfg = JsonConvert.DeserializeObject<CompileConfig>(editorConfigPath);
		Compile(cfg.Dlls, cfg.SourcePath, cfg.OutputPath);
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