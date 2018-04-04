using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Lite
{
	public class FixScriptRef
	{
		const string RuntimeDllPath = "Assets/Scripts/UnityRuntimeDll.dll";
		private const int DefaultFileId = 11500000;
		private const string MetaFile = "Assets/Editor/FixScriptRef/ScriptMeta.json";
		private static ScriptMetaInfo metaInfo = null;


		[MenuItem(AppConst.AppName + "/Fix/Gen meta", false, 1)]
		public static void GenScriptMeta()
		{
			string[] paths = AssetDatabase.GetAllAssetPaths();
			ScriptMetaInfo scriptMeta = ScriptableObject.CreateInstance<ScriptMetaInfo>();
			List<ScriptMetaEntry> metaEntris = new List<ScriptMetaEntry>();

			for (var index = 0; index < paths.Length; index++)
			{
				EditorUtility.DisplayProgressBar("Generating...", "OJZ", index / (float)paths.Length);
				var v = paths[index];
				if (!v.Contains("Assets/Scripts/") && !v.Contains("Assets/Editor/") && !v.Contains("Assets/ThirdParty/"))
					continue;
				MonoScript o = AssetDatabase.LoadAssetAtPath(v, typeof(MonoScript)) as MonoScript;
				if (o != null && o.GetClass() != null)
				{
					ScriptMetaEntry entry;
					entry.guid = AssetDatabase.AssetPathToGUID(v);
					entry.fileId = FileIdUtil.FromType(o.GetClass());
					metaEntris.Add(entry);
				}
			}
			EditorUtility.ClearProgressBar();
			scriptMeta.scriptMetas = metaEntris.ToArray();
			string json = JsonUtility.ToJson(scriptMeta);
			FileStream fs = File.Open(MetaFile, FileMode.Create);
			StreamWriter sw = new StreamWriter(fs);
			sw.Write(json);
			sw.Flush();
			fs.Close();
			//EditorUtility.DisplayDialog("Floating", "Gen Script Done!", "OK");
			Debug.Log("Gen Script Done.");
		}

		[MenuItem(AppConst.AppName + "/Fix/Fix All", false, 2)]
		public static void FixAllResourceInDev()
		{
			try
			{
				if (HasScriptDll())
				{
					Debug.LogError("你...点错了吧？ (⊙﹏⊙)b");
					return;
				}

				FixAllPrefabs(true);

				Debug.Log("修复成功\\(^o^)/~");
				EditorUtility.ClearProgressBar();
			}
			catch (Exception e)
			{
				Debug.LogError("/(ㄒoㄒ)/~~修复出错，请尝试重新修复。" + e.ToString());
				EditorUtility.ClearProgressBar();
				AssetDatabase.Refresh();
			}
		}


		[MenuItem(AppConst.AppName + "/Fix/Fix All with Dll", false, 3)]
		public static void FixAllResourceInEditor()
		{
			try
			{
				if (!HasScriptDll())
				{
					Debug.LogError("你...点错了吧？ (⊙﹏⊙)b");
					return;
				}

				FixAllPrefabs(false);

				Debug.Log("修复成功\\(^o^)/~");
				EditorUtility.ClearProgressBar();
			}
			catch (Exception e)
			{
				Debug.LogError("/(ㄒoㄒ)/~~修复出错，请尝试重新修复。" + e.ToString());
				EditorUtility.ClearProgressBar();
				AssetDatabase.Refresh();
			}
		}

		private static bool HasScriptDll()
		{
			bool ret = File.Exists(RuntimeDllPath);
			return ret;
		}



		public static void FixAllPrefabs(bool isDev)
		{
			try
			{
				List<string> paths = new List<string>();

				List<string> dirs = new List<string>
                {
                    Application.dataPath
                };

				foreach (var dir in dirs)
				{
					paths.AddRange(GetFiles(dir));
				}

				int count = 0;
				foreach (string path in paths)
				{
					count++;
					if (count % 10 == 0) EditorUtility.DisplayProgressBar("Fixing...", "orz...", count * 1.0f / paths.Count);
					ParseYaml(path, isDev);
				}
				metaInfo = null;
				Debug.Log("Fix prefab in dev done !!!");
				EditorUtility.ClearProgressBar();
				AssetDatabase.Refresh();
			}
			catch (Exception e)
			{
				EditorUtility.ClearProgressBar();
				AssetDatabase.Refresh();
				Debug.LogError(e.ToString());
			}
		}


		private static List<string> GetFiles(string dir)
		{
			List<string> paths = new List<string>();
			var files = Directory.GetFiles(dir, "*.prefab");
			paths.AddRange(files);
			files = Directory.GetFiles(dir, "*.unity");
			paths.AddRange(files);
			files = Directory.GetFiles(dir, "*.asset");
			paths.AddRange(files);
			var dirs = Directory.GetDirectories(dir);
			foreach (var d in dirs)
			{
				paths.AddRange(GetFiles(d));
			}
			return paths;
		}


		public static void ParseYaml(string path, bool isDev)
		{
			if (metaInfo == null)
			{
				OpenMetaFile();
			}
			FileStream readFs = File.Open(path, FileMode.Open);
			string format = "  m_Script: {0}fileID: {1}, guid: {2}, type: 3{3}";
			StreamReader sr = new StreamReader(readFs);

			StringWriter stringWriter = new StringWriter();
			string line;
			const int monoScriptOuter = 1;
			const int monoScriptInner = 2;
			//const int MonoScriptOuter = 3;
			int parseState = monoScriptOuter;
			bool isChange = false;

			while (!sr.EndOfStream)
			{
				line = sr.ReadLine();
				if (parseState == monoScriptOuter)
				{
					if (line.IndexOf("MonoBehaviour", StringComparison.Ordinal) != -1)
					{
						parseState = monoScriptInner;
					}
				}
				else if (parseState == monoScriptInner)
				{
					string innerLine = line;
					innerLine = innerLine.Trim();
					if (innerLine.IndexOf("m_Script", StringComparison.Ordinal) != -1)
					{
						int fileId = 0;
						string guid = "";
						ParseMonoBehaviour(innerLine, out fileId, out guid);
						if (fileId == 0)
						{
							Debug.Log(path + " file error!!!!");
							sr.Close();
							readFs.Close();
							return;
						}
						bool isRight = JudgeRight(isDev, fileId, guid);
						isChange |= !isRight;
						if (!isRight)
						{
							if (isDev)
							{
								line = string.Format(format, "{", DefaultFileId, GetGuidByFileId(fileId), "}");
							}
							else
							{
								string dllGuid = GetDllGuid();
								line = string.Format(format, "{", GetFileIdByGuid(guid), dllGuid, "}");
							}
						}
						parseState = monoScriptOuter;
					}
				}
				stringWriter.WriteLine(line);
			}
			sr.Close();
			readFs.Close();
			if (isChange)
			{
				FileStream writeFs = File.Open(path, FileMode.Create);
				StreamWriter sw = new StreamWriter(writeFs);
				sw.Write(stringWriter.ToString());
				sw.Flush();
				writeFs.Close();
				//Debug.Log(path + " processed!");
			}
			//            Debug.Log(stringWriter.ToString());
		}

		private static void OpenMetaFile()
		{
			FileStream fs = File.Open(MetaFile, FileMode.Open);
			StreamReader sr = new StreamReader(fs);
			string v = sr.ReadToEnd();
			var inst = ScriptableObject.CreateInstance<ScriptMetaInfo>();
			JsonUtility.FromJsonOverwrite(v, inst);
			if (inst.scriptMetas != null)
			{
				metaInfo = inst;
			}
		}

		private static bool JudgeRight(bool isDev, int file, string guid)
		{
			if (file == DefaultFileId)
			{
				if (isDev)
					return true;
				else
				{
					int fileid = GetFileIdByGuid(guid);
					return fileid == 0;
				}
			}

			if (string.IsNullOrEmpty(GetGuidByFileId(file)))
			{
				return true;
			}
			return !isDev;
		}

		private static void ParseMonoBehaviour(string script, out int fileId, out string guid)
		{
			int startIndex = script.IndexOf("{", StringComparison.Ordinal);
			int endIndex = script.IndexOf("}", StringComparison.Ordinal);
			fileId = 0;
			guid = "";
			if (startIndex == -1 || endIndex == -1 || endIndex < startIndex)
			{
				Debug.Log("failed to find index");
				return;
			}
			string content = script.Substring(startIndex + 1, endIndex - startIndex - 1);
			string[] items = content.Split(',', ':');
			for (int i = 0; i < items.Length; i++)
			{
				string item = items[i].Trim();
				if (item == "fileID")
				{
					string nextItem = items[++i].Trim();
					try
					{
						fileId = int.Parse(nextItem);
					}
					catch (Exception e)
					{
						Console.WriteLine(e);
						//                        throw;
					}
				}
				if (item == "guid")
				{
					guid = items[++i].Trim();
				}
			}
		}

		private static string GetGuidByFileId(int fileId)
		{
			foreach (var v in metaInfo.scriptMetas)
			{
				if (v.fileId == fileId)
				{
					return v.guid;
				}
			}
			return "";
		}

		private static int GetFileIdByGuid(string guid)
		{
			foreach (var v in metaInfo.scriptMetas)
			{
				if (v.guid == guid)
				{
					return v.fileId;
				}
			}
			return 0;
		}

		private static string GetDllGuid()
		{
			return AssetDatabase.AssetPathToGUID(RuntimeDllPath);
		}


		public static void OutputMissingScriptsOnBehaviours(bool isDev = true)
		{
			try
			{
				List<string> paths = new List<string>();
				paths.AddRange(GetFiles(Application.dataPath));

				if (metaInfo == null)
					OpenMetaFile();

				for (int i = 0; i < paths.Count; ++i)
				{
					if (i % 10 == 0) EditorUtility.DisplayProgressBar("Finding...", "orz...", i * 1.0f / paths.Count);
					string path = paths[i];

					FileStream readFs = File.Open(path, FileMode.Open);
					StreamReader sr = new StreamReader(readFs);
					string line;
					const int monoScriptOuter = 1;
					const int monoScriptInner = 2;
					int parseState = monoScriptOuter;
					bool isChange = false;

					while (!sr.EndOfStream)
					{
						line = sr.ReadLine();
						if (parseState == monoScriptOuter)
						{
							if (line.IndexOf("MonoBehaviour", StringComparison.Ordinal) != -1)
								parseState = monoScriptInner;
						}
						else if (parseState == monoScriptInner)
						{
							string innerLine = line;
							innerLine = innerLine.Trim();
							if (innerLine.IndexOf("m_Script", StringComparison.Ordinal) != -1)
							{
								int fileId;
								string guid;
								ParseMonoBehaviour(innerLine, out fileId, out guid);
								if (fileId == 0)
									continue;
								bool isRight = JudgeRight(isDev, fileId, guid);
								isChange |= !isRight;
								if (!isRight)
								{
									if (isDev)
									{
										string guid2 = GetGuidByFileId(fileId);
										if (string.IsNullOrEmpty(guid2))
											Debug.LogError("Missing: " + path);
									}
									else
									{
										int fileId2 = GetFileIdByGuid(guid);
										if (fileId2 == 0)
											Debug.LogError("Missing: " + path);
									}
								}
								parseState = monoScriptOuter;
							}
						}
					}
					sr.Close();
					readFs.Close();
				}

				EditorUtility.ClearProgressBar();
			}
			catch (Exception e)
			{
				EditorUtility.ClearProgressBar();
				Debug.LogError(e.ToString());
			}
		}


	}
}