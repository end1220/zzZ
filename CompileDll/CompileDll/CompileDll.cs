
using System;
using System.Collections.Generic;
using System.IO;
using System.CodeDom.Compiler;
using Microsoft.CSharp;
using System.Text;


namespace CompileDll
{
	class DllCompiler
	{
		const string relativePath = "..\\..\\..\\..\\Transparent\\";
		string workingDirectory = Environment.CurrentDirectory + "\\" + relativePath;

		[System.Serializable]
		class CompileConfig
		{
			public string OutputPath;
			public string[] SourcePath;
			public string[] Dlls;
		}

		CompileConfig runtimeCfg = new CompileConfig()
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
				relativePath + "Library\\UnityAssemblies\\UnityEditor.dll",
				relativePath + "Library\\UnityAssemblies\\UnityEngine.dll",
				relativePath + "Library\\UnityAssemblies\\UnityEngine.UI.dll",
				relativePath + "Library\\UnityAssemblies\\UnityEditor.UI.dll",
				//"Assets\\Plugins\\x86_64\\xlua.dll",
				relativePath + "Assets\\Plugins\\protobuf-net.dll"
			}
		};

		public void CompileRuntimeDll()
		{
			CompileConfig cfg = runtimeCfg;
			List<string> sourceFiles = new List<string>();
			for (int i = 0; i < cfg.SourcePath.Length; ++i)
			{
				string path = workingDirectory + "\\" + cfg.SourcePath[i];
				if (Directory.Exists(path))
					GetSourceFiles(sourceFiles, path);
				else
					Console.WriteLine(path + " not exist...");
			}
			string[] sources = sourceFiles.ToArray();
			Compile(cfg.Dlls, sources, workingDirectory + "\\" + cfg.OutputPath);
		}

		public void CompileEditorDll()
		{
			CompileConfig cfg = runtimeCfg;
			Compile(cfg.Dlls, cfg.SourcePath, cfg.OutputPath);
		}

		void Compile(string[] references, string[] sources, string outputfile)
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
					sb.Append("(" + error.ErrorNumber + ") " + error.ErrorText + ". Line:" + error.Line.ToString() + ", Column:" + error.Column.ToString());
					Console.WriteLine(sb.ToString());
				}
			}
			else
			{
				Console.WriteLine("Build successfully");
			}
		}

		void GetSourceFiles(List<string> sourceFiles, string directory)
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

	}
}
