
using System.Collections.Generic;
using System.IO;
using System.Diagnostics;
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

	static CompileConfig runtimeCfg = new CompileConfig()
	{
		OutputPath = "Float-Runtime.dll",
		SourcePath = new string[]
		{
			"Assets\\Scripts\\",
			"Assets\\Editor\\"
		},
		Dlls = new string[]
		{
			"C:\\Program Files (x86)\\Reference Assemblies\\Microsoft\\Framework\\.NETFramework\\v3.5\\Profile\\Unity Subset v3.5\\mscorlib.dll",
			"C:\\Program Files (x86)\\Reference Assemblies\\Microsoft\\Framework\\.NETFramework\\v3.5\\Profile\\Unity Subset v3.5\\System.dll",
			"C:\\Program Files (x86)\\Reference Assemblies\\Microsoft\\Framework\\.NETFramework\\v3.5\\Profile\\Unity Subset v3.5\\System.Core.dll",
			"C:\\Program Files (x86)\\Reference Assemblies\\Microsoft\\Framework\\.NETFramework\\v3.5\\Profile\\Unity Subset v3.5\\System.xml.dll",
			"C:\\Program Files (x86)\\Reference Assemblies\\Microsoft\\Framework\\.NETFramework\\v3.5\\Profile\\Unity Subset v3.5\\System.xml.linq.dll",
			"Library\\UnityAssemblies\\UnityEditor.dll",
			"Library\\UnityAssemblies\\UnityEngine.dll",
			"Library\\UnityAssemblies\\UnityEngine.UI.dll",
			"Library\\UnityAssemblies\\UnityEditor.UI.dll",
			//"Assets\\Plugins\\x86_64\\xlua.dll",
			"Assets\\Plugins\\protobuf-net.dll"
		}
	};

	[MenuItem(AppDefine.AppName + "/Runtime Dll", false, 3)]
	static void CompileRuntimeDllCmd()
	{
		RunCmd("help");
	}

	//[MenuItem(AppDefine.AppName + "/Runtime Dll", false, 3)]
	static void CompileRuntimeDll()
	{
		CompileConfig cfg = runtimeCfg;
		List<string> sourceFiles = new List<string>();
		for (int i = 0; i < cfg.SourcePath.Length; ++i)
		{
			string path = System.Environment.CurrentDirectory + "\\" + cfg.SourcePath[i];
			if (Directory.Exists(path))
				GetSourceFiles(sourceFiles, path);
			else
				UnityEngine.Debug.LogError(path + " not exist...");
		}
		string[] sources = sourceFiles.ToArray();
		Compile(cfg.Dlls, sources, System.Environment.CurrentDirectory + "\\" + cfg.OutputPath);
	}

	//[MenuItem(AppDefine.AppName + "/Editor Dll", false, 3)]
	static void CompileEditorDll()
	{
		CompileConfig cfg = runtimeCfg;
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
	}

	static string RunCmd(string command)
	{
		//例Process  
		Process p = new Process();
		p.StartInfo.FileName = "C:/Windows/Microsoft.NET/Framework/v3.5/csc.exe";         //确定程序名  
		p.StartInfo.Arguments = command;   //确定程式命令行  
		p.StartInfo.UseShellExecute = false;      //Shell的使用  
		p.StartInfo.RedirectStandardInput = true;  //重定向输入  
		p.StartInfo.RedirectStandardOutput = true; //重定向输出  
		p.StartInfo.RedirectStandardError = true;  //重定向输出错误  
		p.StartInfo.CreateNoWindow = false;        //设置置不显示窗口  
		p.Start();
		return p.StandardOutput.ReadToEnd();      //输出出流取得命令行结果果  
	}

}