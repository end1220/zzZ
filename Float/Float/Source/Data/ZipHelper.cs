using System;
using System.IO;
using ICSharpCode.SharpZipLib.Zip;
using ICSharpCode.SharpZipLib.Checksums;



public class ZipHelper
{
	#region 压缩
 
	private static bool ZipDirectory(string folderToZip, ZipOutputStream zipStream, string parentFolderName)
	{
		bool result = true;
		string[] folders, files;
		ZipEntry ent = null;
		FileStream fs = null;
		Crc32 crc = new Crc32();

		try
		{
			ent = new ZipEntry(Path.Combine(parentFolderName, Path.GetFileName(folderToZip) + "/"));
			zipStream.PutNextEntry(ent);
			zipStream.Flush();

			files = Directory.GetFiles(folderToZip);
			foreach (string file in files)
			{
				fs = File.OpenRead(file);

				byte[] buffer = new byte[fs.Length];
				fs.Read(buffer, 0, buffer.Length);
				ent = new ZipEntry(Path.Combine(parentFolderName, Path.GetFileName(folderToZip) + "/" + Path.GetFileName(file)));
				ent.DateTime = DateTime.Now;
				ent.Size = fs.Length;

				fs.Close();

				crc.Reset();
				crc.Update(buffer);

				ent.Crc = crc.Value;
				zipStream.PutNextEntry(ent);
				zipStream.Write(buffer, 0, buffer.Length);
			}

		}
		catch
		{
			result = false;
		}
		finally
		{
			if (fs != null)
			{
				fs.Close();
				fs.Dispose();
			}
			if (ent != null)
			{
				ent = null;
			}
			GC.Collect();
			GC.Collect(1);
		}

		folders = Directory.GetDirectories(folderToZip);
		foreach (string folder in folders)
			if (!ZipDirectory(folder, zipStream, folderToZip))
				return false;

		return result;
	}

	public static bool ZipDirectory(string folderToZip, string zipedFile, string password)
	{
		bool result = false;
		if (!Directory.Exists(folderToZip))
			return result;

		ZipOutputStream zipStream = new ZipOutputStream(File.Create(zipedFile));
		zipStream.SetLevel(6);
		if (!string.IsNullOrEmpty(password)) zipStream.Password = password;

		result = ZipDirectory(folderToZip, zipStream, "");

		zipStream.Finish();
		zipStream.Close();

		return result;
	}

	public static bool ZipDirectory(string folderToZip, string zipedFile)
	{
		bool result = ZipDirectory(folderToZip, zipedFile, null);
		return result;
	}

	public static bool ZipFile(string fileToZip, string zipedFile, string password)
	{
		bool result = true;
		ZipOutputStream zipStream = null;
		FileStream fs = null;
		ZipEntry ent = null;

		if (!File.Exists(fileToZip))
			return false;

		try
		{
			fs = File.OpenRead(fileToZip);
			byte[] buffer = new byte[fs.Length];
			fs.Read(buffer, 0, buffer.Length);
			fs.Close();

			fs = File.Create(zipedFile);
			zipStream = new ZipOutputStream(fs);
			if (!string.IsNullOrEmpty(password)) zipStream.Password = password;
			ent = new ZipEntry(Path.GetFileName(fileToZip));
			zipStream.PutNextEntry(ent);
			zipStream.SetLevel(6);

			zipStream.Write(buffer, 0, buffer.Length);

		}
		catch
		{
			result = false;
		}
		finally
		{
			if (zipStream != null)
			{
				zipStream.Finish();
				zipStream.Close();
			}
			if (ent != null)
			{
				ent = null;
			}
			if (fs != null)
			{
				fs.Close();
				fs.Dispose();
			}
		}
		GC.Collect();
		GC.Collect(1);

		return result;
	}

	public static bool ZipFile(string fileToZip, string zipedFile)
	{
		bool result = ZipFile(fileToZip, zipedFile, null);
		return result;
	}

	public static bool Zip(string fileToZip, string zipedFile, string password)
	{
		bool result = false;
		if (Directory.Exists(fileToZip))
			result = ZipDirectory(fileToZip, zipedFile, password);
		else if (File.Exists(fileToZip))
			result = ZipFile(fileToZip, zipedFile, password);

		return result;
	}

	public static bool Zip(string fileOrDirToZip, string zipedFile)
	{
		bool result = Zip(fileOrDirToZip, zipedFile, null);
		return result;

	}

	#endregion

	#region 解压


	public static bool UnZip(string fileToUnZip, string zipedFolder, string password)
	{
		bool result = true;
		FileStream fs = null;
		ZipInputStream zipStream = null;
		ZipEntry ent = null;
		string fileName;

		if (!File.Exists(fileToUnZip))
			return false;

		if (!Directory.Exists(zipedFolder))
			Directory.CreateDirectory(zipedFolder);

		try
		{
			zipStream = new ZipInputStream(File.OpenRead(fileToUnZip));
			if (!string.IsNullOrEmpty(password)) zipStream.Password = password;
			while ((ent = zipStream.GetNextEntry()) != null)
			{
				if (!string.IsNullOrEmpty(ent.Name))
				{
					fileName = Path.Combine(zipedFolder, ent.Name);
					fileName = fileName.Replace('/', '\\');//change by Mr.HopeGi   

					int index = ent.Name.LastIndexOf('/');
					if (index != -1 || fileName.EndsWith("\\"))
					{
						string tmpDir = (index != -1 ? fileName.Substring(0, fileName.LastIndexOf('\\')) : fileName) + "\\";
						if (!Directory.Exists(tmpDir))
						{
							Directory.CreateDirectory(tmpDir);
						}
						if (tmpDir == fileName)
						{
							continue;
						}
					}

					fs = File.Create(fileName);
					int size = 2048;
					byte[] data = new byte[size];
					while (true)
					{
						size = zipStream.Read(data, 0, data.Length);
						if (size > 0)
							fs.Write(data, 0, data.Length);
						else
							break;
					}
				}
			}
		}
		catch
		{
			result = false;
		}
		finally
		{
			if (fs != null)
			{
				fs.Close();
				fs.Dispose();
			}
			if (zipStream != null)
			{
				zipStream.Close();
				zipStream.Dispose();
			}
			if (ent != null)
			{
				ent = null;
			}
			GC.Collect();
			GC.Collect(1);
		}
		return result;
	}

	public static bool UnZip(string fileToUnZip, string zipedFolder)
	{
		bool result = UnZip(fileToUnZip, zipedFolder, null);
		return result;
	}

	#endregion
}
