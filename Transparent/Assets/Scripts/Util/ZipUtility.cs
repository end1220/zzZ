/******************************************************
* DESCRIPTION: Zip包的压缩与解压
*
*     Copyright (c) 2017, 谭伟俊 （TanWeijun）
*     All rights reserved
*
* CREATED: 2017.03.11, 08:37, CST
******************************************************/

using System.IO;
using UnityEngine;
using ICSharpCode.SharpZipLib.Zip;

public static class ZipUtility
{
	public abstract class ZipCallback
	{
		public virtual bool OnPreZip(ZipEntry _entry)
		{
			return true;
		}

		public virtual void OnPostZip(ZipEntry _entry) { }

		public virtual void OnFinished(bool success) { }
	}

	public abstract class UnzipCallback
	{
		public virtual bool OnPreUnzip(ZipEntry _entry)
		{
			return true;
		}

		public virtual void OnPostUnzip(ZipEntry _entry) { }

		public virtual void OnFinished(bool success) { }
	}

	/// <summary>
	/// zip to a zip file
	/// </summary>
	/// <param name="filesOrDirectories">relative to root path</param>
	/// <param name="rootPath">absolute path</param>
	/// <param name="outputFile">absolute path</param>
	/// <param name="password"></param>
	/// <param name="callback"></param>
	/// <returns></returns>
	public static bool Zip(string[] filesOrDirectories, string rootPath, string outputFile, string password, ZipCallback callback)
	{
		if ((filesOrDirectories == null) || string.IsNullOrEmpty(outputFile))
		{
			if (callback != null)
				callback.OnFinished(false);

			return false;
		}

		ZipOutputStream zipOutputStream = new ZipOutputStream(File.Create(outputFile));
		zipOutputStream.SetLevel(6);
		if (!string.IsNullOrEmpty(password))
			zipOutputStream.Password = password;

		for (int index = 0; index < filesOrDirectories.Length; ++index)
		{
			bool result = false;
			string relPath = filesOrDirectories[index];
			string absPath = Path.Combine(rootPath, relPath);
			if (Directory.Exists(absPath))
				result = ZipDirectory(relPath, rootPath, zipOutputStream, callback);
			else if (File.Exists(absPath))
				result = ZipFile(relPath, rootPath, zipOutputStream, callback);

			if (!result)
			{
				if (callback != null)
					callback.OnFinished(false);

				return false;
			}
		}

		zipOutputStream.Finish();
		zipOutputStream.Close();

		if (callback != null)
			callback.OnFinished(true);

		return true;
	}

	/// <summary>
	/// unzip a zip file
	/// </summary>
	/// <param name="filePath">absolute path</param>
	/// <param name="outputPath">absolute path</param>
	/// <param name="password"></param>
	/// <param name="callback"></param>
	/// <returns></returns>
	public static bool UnzipFile(string filePath, string outputPath, string password, UnzipCallback callback)
	{
		if (string.IsNullOrEmpty(filePath) || string.IsNullOrEmpty(outputPath))
		{
			if (callback != null)
				callback.OnFinished(false);

			return false;
		}

		try
		{
			return UnzipFile(File.OpenRead(filePath), outputPath, password, callback);
		}
		catch (System.Exception _e)
		{
			Debug.LogError("[ZipUtility.UnzipFile]: " + _e.ToString());

			if (callback != null)
				callback.OnFinished(false);

			return false;
		}
	}

	/// <summary>
	/// zip a file
	/// </summary>
	/// <param name="filePath">relative file path to root path</param>
	/// <param name="rootPath">absolute root path</param>
	/// <param name="outputStream"></param>
	/// <param name="callback"></param>
	/// <returns></returns>
	private static bool ZipFile(string filePath, string rootPath, ZipOutputStream outputStream, ZipCallback callback)
	{
		//Crc32 crc32 = new Crc32();
		ZipEntry entry = null;
		FileStream fileStream = null;
		try
		{
			string entryName = filePath;
			entry = new ZipEntry(entryName);
			entry.DateTime = System.DateTime.Now;

			if ((callback != null) && !callback.OnPreZip(entry))
				return true;

			fileStream = File.OpenRead(Path.Combine(rootPath, filePath));
			byte[] buffer = new byte[fileStream.Length];
			fileStream.Read(buffer, 0, buffer.Length);
			fileStream.Close();

			entry.Size = buffer.Length;

			//crc32.Reset();
			//crc32.Update(buffer);
			//entry.Crc = crc32.Value;

			outputStream.PutNextEntry(entry);
			outputStream.Write(buffer, 0, buffer.Length);
		}
		catch (System.Exception _e)
		{
			Debug.LogError("[ZipUtility.ZipFile]: " + _e.ToString());
			return false;
		}
		finally
		{
			if (null != fileStream)
			{
				fileStream.Close();
				fileStream.Dispose();
			}
		}

		if (callback != null)
			callback.OnPostZip(entry);

		return true;
	}

	/// <summary>
	/// zip a dir
	/// </summary>
	/// <param name="directory">relative to root path</param>
	/// <param name="rootPath">absolute root path</param>
	/// <param name="outputStream"></param>
	/// <param name="callback"></param>
	/// <returns></returns>
	private static bool ZipDirectory(string directory, string rootPath, ZipOutputStream outputStream, ZipCallback callback = null)
	{
		ZipEntry entry = null;
		try
		{
			/*string entryName = directory;
			entry = new ZipEntry(entryName);
			entry.DateTime = System.DateTime.Now;
			entry.Size = 0;

			if ((callback != null) && !callback.OnPreZip(entry))
				return true;

			outputStream.PutNextEntry(entry);
			outputStream.Flush();*/

			string[] files = Directory.GetFiles(Path.Combine(rootPath, directory));
			for (int index = 0; index < files.Length; ++index)
			{
				string relativeFile = files[index].Substring(rootPath.Length);
				ZipFile(relativeFile, rootPath, outputStream, callback);
			}
		}
		catch (System.Exception _e)
		{
			Debug.LogError("[ZipUtility.ZipDirectory]: " + _e.ToString());
			return false;
		}

		string[] directories = Directory.GetDirectories(Path.Combine(rootPath, directory));
		for (int index = 0; index < directories.Length; ++index)
		{
			string relativeDir = directories[index].Substring(rootPath.Length);
			if (!ZipDirectory(relativeDir, rootPath, outputStream, callback))
				return false;
		}

		if (callback != null)
			callback.OnPostZip(entry);

		return true;
	}

	public static bool UnzipFile(byte[] fileBytes, string outputPath, string password, UnzipCallback callback)
	{
		if ((null == fileBytes) || string.IsNullOrEmpty(outputPath))
		{
			if (callback != null)
				callback.OnFinished(false);

			return false;
		}

		bool result = UnzipFile(new MemoryStream(fileBytes), outputPath, password, callback);
		if (!result)
		{
			if (callback != null)
				callback.OnFinished(false);
		}

		return result;
	}

	public static bool UnzipFile(Stream inputStream, string outputPath, string password, UnzipCallback callback)
	{
		if ((null == inputStream) || string.IsNullOrEmpty(outputPath))
		{
			if (callback != null)
				callback.OnFinished(false);

			return false;
		}

		if (!Directory.Exists(outputPath))
			Directory.CreateDirectory(outputPath);

		ZipEntry entry = null;
		using (ZipInputStream zipInputStream = new ZipInputStream(inputStream))
		{
			if (!string.IsNullOrEmpty(password))
				zipInputStream.Password = password;

			while (null != (entry = zipInputStream.GetNextEntry()))
			{
				if (string.IsNullOrEmpty(entry.Name))
					continue;

				if ((callback != null) && !callback.OnPreUnzip(entry))
					continue;

				if (entry.Name.Contains("/") || entry.Name.Contains("\\"))
				{
					string tempName = entry.Name.Replace("\\", "/");
					string[] parts = tempName.Split('/');
					string path = outputPath;
					for (int i = 0; i < parts.Length - 1; ++i)
					{
						path = Path.Combine(path, parts[i]);
						if (!Directory.Exists(path))
							Directory.CreateDirectory(path);
					}
				}

				try
				{
					string filePathName = Path.Combine(outputPath, entry.Name);
					using (FileStream fileStream = File.Create(filePathName))
					{
						byte[] bytes = new byte[1024];
						while (true)
						{
							int count = zipInputStream.Read(bytes, 0, bytes.Length);
							if (count > 0)
								fileStream.Write(bytes, 0, count);
							else
							{
								if (callback != null)
									callback.OnPostUnzip(entry);

								break;
							}
						}
					}
				}
				catch (System.Exception _e)
				{
					Debug.LogError("[ZipUtility.UnzipFile]: " + _e.ToString());

					if (callback != null)
						callback.OnFinished(false);

					return false;
				}
			}
		}

		if (callback != null)
			callback.OnFinished(true);

		return true;
	}

}


