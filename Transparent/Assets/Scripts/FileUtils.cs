/*
Copyright (c) 2015-2016 topameng(topameng@qq.com)

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/

using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Text;


public class FileUtils
{
	public static FileUtils Instance
	{
		get
		{
			if (instance == null)
			{
				instance = new FileUtils();
			}

			return instance;
		}

		protected set
		{
			instance = value;
		}
	}

	//beZip = false 在search path 中查找读取lua文件。否则从外部设置过来bundel文件中读取lua文件
	public bool beBundle = false;
	protected List<string> searchPaths = new List<string>();
	protected Dictionary<string, AssetBundle> BundleMap = new Dictionary<string, AssetBundle>();

	protected static FileUtils instance = null;

	public FileUtils()
	{
		instance = this;
	}

	public virtual void Dispose()
	{
		if (instance != null)
		{
			instance = null;
			searchPaths.Clear();

			foreach (KeyValuePair<string, AssetBundle> iter in BundleMap)
			{
				iter.Value.Unload(true);
			}

			BundleMap.Clear();
		}
	}

	//格式: 路径/?.lua
	public bool AddSearchPath(string path, bool front = false)
	{
		int index = searchPaths.IndexOf(path);

		if (index >= 0)
		{
			return false;
		}

		if (front)
		{
			searchPaths.Insert(0, path);
		}
		else
		{
			searchPaths.Add(path);
		}

		return true;
	}

	public bool RemoveSearchPath(string path)
	{
		int index = searchPaths.IndexOf(path);

		if (index >= 0)
		{
			searchPaths.RemoveAt(index);
			return true;
		}

		return false;
	}

	public string GetPackagePath()
	{
		StringBuilder sb = StringBuilderCache.Acquire();
		sb.Append(";");

		for (int i = 0; i < searchPaths.Count; i++)
		{
			sb.Append(searchPaths[i]);
			sb.Append(';');
		}

		return StringBuilderCache.GetStringAndRelease(sb);
	}

	public void AddSearchBundle(string name, AssetBundle bundle)
	{
		BundleMap[name] = bundle;
	}

	public string FindFile(string fileName)
	{
		if (fileName == string.Empty)
		{
			return string.Empty;
		}

		if (Path.IsPathRooted(fileName))
		{
			if (!fileName.EndsWith(".lua"))
			{
				fileName += ".lua";
			}

			return fileName;
		}

		if (fileName.EndsWith(".lua"))
		{
			fileName = fileName.Substring(0, fileName.Length - 4);
		}

		string fullPath = null;

		for (int i = 0; i < searchPaths.Count; i++)
		{
			fullPath = searchPaths[i].Replace("?", fileName);

			if (File.Exists(fullPath))
			{
				return fullPath;
			}
		}

		return null;
	}

	public virtual byte[] ReadFile(string fileName)
	{
		if (!beBundle)
		{
			string path = FindFile(fileName);
			byte[] str = null;

			if (!string.IsNullOrEmpty(path) && File.Exists(path))
			{
#if !UNITY_WEBPLAYER
				str = File.ReadAllBytes(path);
#else
                    throw new LuaException("can't run in web platform, please switch to other platform");
#endif
			}

			return str;
		}
		else
		{
			return ReadBundleFile(fileName);
		}
	}

	public virtual string FindFileError(string fileName)
	{
		if (Path.IsPathRooted(fileName))
		{
			return fileName;
		}

		StringBuilder sb = StringBuilderCache.Acquire();

		if (fileName.EndsWith(".lua"))
		{
			fileName = fileName.Substring(0, fileName.Length - 4);
		}

		for (int i = 0; i < searchPaths.Count; i++)
		{
			sb.AppendFormat("\n\tno file '{0}'", searchPaths[i]);
		}

		sb = sb.Replace("?", fileName);

		if (beBundle)
		{
			int pos = fileName.LastIndexOf('/');
			string bundle = "";

			if (pos > 0)
			{
				bundle = fileName.Substring(0, pos);
				//bundle = bundle.Replace('/', '_');
				bundle = string.Format("lua/{0}", bundle);
			}
			else
			{
				bundle = "lua/lua";
			}

			sb.AppendFormat("\n\tno file '{0}' in {1}", fileName, bundle);
		}

		return StringBuilderCache.GetStringAndRelease(sb);
	}

	byte[] ReadBundleFile(string fileName)
	{
		byte[] buffer = null;
		string bundleName = null;
		StringBuilder sbBundleName = StringBuilderCache.Acquire();
		sbBundleName.Append("lua");
		int pos = fileName.LastIndexOf('/');

		if (pos > 0)
		{
			sbBundleName.Append("/");
			sbBundleName.Append(fileName.Substring(0, pos).ToLower());
			//sb.Replace('/', '_');
			//fileName = fileName.Substring(pos + 1);
		}
		else
		{
			sbBundleName.Append("/lua");
		}

		bundleName = StringBuilderCache.GetStringAndRelease(sbBundleName);
		AssetBundle abFile = null;
		BundleMap.TryGetValue(bundleName, out abFile);

		if (abFile != null)
		{
			StringBuilder sbAssetName = StringBuilderCache.Acquire();
			sbAssetName.Append("Assets/LuaTemp/Lua/");
			sbAssetName.Append(fileName);
			if (!fileName.EndsWith(".lua"))
				sbAssetName.Append(".lua.bytes");
			else
				sbAssetName.Append(".bytes");
			fileName = StringBuilderCache.GetStringAndRelease(sbAssetName);
			TextAsset luaCode = abFile.LoadAsset<TextAsset>(fileName);

			if (luaCode != null)
			{
				buffer = luaCode.bytes;
				Resources.UnloadAsset(luaCode);
			}
		}
		else
		{
			Log.Error(string.Format("Cannot find {0} in bundle map. Forget calling LuaLoader.AddBundle(xxx)??", bundleName));
		}

		return buffer;
	}

}
