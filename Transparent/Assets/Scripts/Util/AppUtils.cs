﻿using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;


static public class AppUtils
{
	public static string GenUniqueGUID()
	{
		return Guid.NewGuid().ToString();
	}

	public static long GenUniqueGUIDLong()
	{
		byte[] buffer = System.Guid.NewGuid().ToByteArray();
		return System.BitConverter.ToInt64(buffer, 0);
	}

	public static void SetRandomSeed(int seed)
	{
		UnityEngine.Random.InitState(seed);
	}

	public static int RandomInt(int min, int max)
	{
		return UnityEngine.Random.Range(min, max);
	}

	/// <summary>
	/// 计算字符串的MD5值
	/// </summary>
	public static string md5(string source)
	{
		MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
		byte[] data = System.Text.Encoding.UTF8.GetBytes(source);
		byte[] md5Data = md5.ComputeHash(data, 0, data.Length);
		md5.Clear();

		string destString = "";
		for (int i = 0; i < md5Data.Length; i++)
		{
			destString += System.Convert.ToString(md5Data[i], 16).PadLeft(2, '0');
		}
		destString = destString.PadLeft(32, '0');
		return destString;
	}

	/// <summary>
	/// 计算文件的MD5值
	/// </summary>
	public static string md5file(string file)
	{
		try
		{
			FileStream fs = new FileStream(file, FileMode.Open);
			System.Security.Cryptography.MD5 md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
			byte[] retVal = md5.ComputeHash(fs);
			fs.Close();

			StringBuilder sb = new StringBuilder();
			for (int i = 0; i < retVal.Length; i++)
			{
				sb.Append(retVal[i].ToString("x2"));
			}
			return sb.ToString();
		}
		catch (Exception ex)
		{
			throw new Exception("md5file() fail, error:" + ex.Message);
		}
	}


	public static void ClearMemory()
	{
		GC.Collect();
		Resources.UnloadUnusedAssets();
		Float.LuaManager.Instance.LuaGC();
	}


	private static string GetPlatformForAssetBundles(RuntimePlatform platform)
	{
		switch (platform)
		{
			case RuntimePlatform.Android:
				return "Android";
			case RuntimePlatform.IPhonePlayer:
				return "iOS";
			case RuntimePlatform.WebGLPlayer:
				return "WebGL";
			case RuntimePlatform.WindowsPlayer:
				return "Windows";
			case RuntimePlatform.OSXPlayer:
				return "OSX";
			// Add more build targets for your own.
			// If you add more targets, don't forget to add the same platforms to GetPlatformForAssetBundles(RuntimePlatform) function.
			default:
				return null;
		}
	}


	/// <summary>
	/// 应用程序内容路径
	/// </summary>
	public static string AppContentPath()
	{
		string path = string.Empty;
		switch (Application.platform)
		{
			case RuntimePlatform.Android:
				path = "jar:file://" + Application.dataPath + "!/assets/";
				break;
			case RuntimePlatform.IPhonePlayer:
				path = Application.dataPath + "/Raw/";
				break;
			default:
				path = Application.streamingAssetsPath + "/";
				break;
		}
		return path;
	}

}

