
using System;
using System.Collections.Generic;
using System.IO;
using System.Diagnostics;
using System.CodeDom.Compiler;
using Microsoft.CSharp;
using System.Text;
using UnityEditor;
using Newtonsoft.Json;


public class CompileDll
{
	static string workingDirectory = Environment.CurrentDirectory + "/";

	[System.Serializable]
	class CompileConfig
	{
		public string OutputPath;
		public string[] SourcePath;
		public string[] Dlls;
	}

	static CompileConfig runtimeCfg = new CompileConfig()
	{
		OutputPath = "Float-Runtime.dll",
		SourcePath = new string[]
		{
			"Assets/Scripts/*.cs"/*,
			"Assets/Editor/ *.cs"*/
		},
		Dlls = new string[]
		{
			"UnitySubsetV3.5/mscorlib.dll",
			"UnitySubsetV3.5/System.dll",
			"UnitySubsetV3.5/System.Core.dll",
			"UnitySubsetV3.5/System.xml.dll",
			"UnitySubsetV3.5/System.xml.linq.dll",
			"Library/UnityAssemblies/UnityEditor.dll",
			"Library/UnityAssemblies/UnityEngine.dll",
			"Library/UnityAssemblies/UnityEngine.UI.dll",
			"Library/UnityAssemblies/UnityEditor.UI.dll",
			//"Assets/Plugins/x86_64/xlua.dll",
			"Assets/Plugins/protobuf-net.dll"
		}
	};


	[MenuItem(AppDefine.AppName + "/Build Runtime Dll", false, 3)]
	static void CompileRuntimeDllCmd()
	{
		string cscPath = "C:/Windows/Microsoft.NET/Framework/v4.0.30319/csc.exe";
		StringBuilder sb = new StringBuilder();
		sb.Append(" /noconfig");
		sb.Append(" /nowarn:1701,1702");
		sb.Append(" /nostdlib");
		sb.Append(" /errorreport:prompt");
		sb.Append(" /warn:4");
		sb.Append(" /define:trace");
		sb.Append(" /define:UNITY_5");
		sb.Append(" /define:UNITY_EDITOR");
		sb.Append(" /debug:pdbonly");
		sb.Append(" /filealign:512");
		sb.Append(" /optimize");
		sb.Append(" /target:library");
		sb.Append(" /out:" + workingDirectory + runtimeCfg.OutputPath);
		sb.Append(" /reference:");
		for (int i = 0; i < runtimeCfg.Dlls.Length; ++i)
			sb.Append(runtimeCfg.Dlls[i] + ((i == runtimeCfg.Dlls.Length - 1) ? "" : ";"));
		for (int i = 0; i < runtimeCfg.SourcePath.Length; ++i)
			sb.Append(" /recurse:" + runtimeCfg.SourcePath[i]);

		RunProcess(cscPath, sb.ToString());
	}

	//[MenuItem(AppDefine.AppName + "/Runtime Dll", false, 3)]
	/*static void CompileRuntimeDll()
	{
		CompileConfig cfg = runtimeCfg;
		List<string> sourceFiles = new List<string>();
		for (int i = 0; i < cfg.SourcePath.Length; ++i)
		{
			string path = workingDirectory + "/" + cfg.SourcePath[i];
			if (Directory.Exists(path))
				GetSourceFiles(sourceFiles, path);
			else
				UnityEngine.Debug.LogError(path + " not exist...");
		}
		string[] sources = sourceFiles.ToArray();
		Compile(cfg.Dlls, sources, workingDirectory + "/" + cfg.OutputPath);
	}

	//[MenuItem(AppDefine.AppName + "/Editor Dll", false, 3)]
	static void CompileEditorDll()
	{
		CompileConfig cfg = runtimeCfg;
		Compile(cfg.Dlls, cfg.SourcePath, cfg.OutputPath);
	}*/

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


	/*static void Compile(string[] references, string[] sources, string outputfile)
	{
		CompilerParameters param = new CompilerParameters(references, outputfile, true);
		param.TreatWarningsAsErrors = false;
		param.GenerateExecutable = false;
		param.IncludeDebugInformation = false;
		//param.EmbeddedResources.Add(@"C:\1.txt");
		//param.MainClass = "return2";

		CSharpCodeProvider provider = new CSharpCodeProvider();
		CompilerResults result = provider.CompileAssemblyFromFile(param, sources);

		if (result.Errors.Count > 0)
		{
			StringBuilder sb = new StringBuilder();
			foreach (CompilerError error in result.Errors)
			{
				sb.Remove(0, sb.Length);
				sb.Append(error.IsWarning ? "Warning" : "Error");
				sb.Append("(" + error.ErrorNumber + ") - " + error.ErrorText + "\t\tLine:" + error.Line.ToString() + ", Column:" + error.Column.ToString());
				UnityEngine.Debug.LogError(sb.ToString());
			}
		}
		else
		{
			UnityEngine.Debug.Log("Build successfully");
		}
	}*/

	static void RunProcess(string exePath, string args)
	{
		Process p = new Process();
		p.StartInfo.FileName = exePath;
		p.StartInfo.Arguments = args;
		p.StartInfo.UseShellExecute = false;
		p.StartInfo.RedirectStandardInput = true; 
		p.StartInfo.RedirectStandardOutput = true;
		p.StartInfo.RedirectStandardError = true;
		p.StartInfo.CreateNoWindow = true;
		p.StartInfo.StandardOutputEncoding = Encoding.GetEncoding("GB2312");
		p.StartInfo.StandardErrorEncoding = Encoding.GetEncoding("GB2312");
		p.Start();
		p.WaitForExit(5000);
		string output = p.StandardOutput.ReadToEnd();
		UnityEngine.Debug.Log(output);
	}

	/*public static string get_uft8(string unicodeString)
	{
		UTF8Encoding utf8 = new UTF8Encoding();
		Byte[] encodedBytes = utf8.GetBytes(unicodeString);
		String decodedString = utf8.GetString(encodedBytes);
		return decodedString;
	}*/

}