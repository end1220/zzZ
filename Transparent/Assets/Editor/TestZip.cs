

using UnityEngine;
using UnityEditor;
using ICSharpCode.SharpZipLib.Zip;


class TestZip
{
	[MenuItem("Floating/zip")]
	static void Zip()
	{
		ZipUtility.Zip(new string[]{ "Zip" }, "C:/Users/admin/Desktop/", "C:/Users/admin/Desktop/test.zip", null, new MyZipCallback());
	}


	[MenuItem("Floating/unzip")]
	static void UnZip()
	{
		ZipUtility.UnzipFile("C:/Users/admin/Desktop/test.zip", "C:/Users/admin/Desktop/test", null, new MyUnzipCallback());
	}

	
}

public class MyZipCallback : ZipUtility.ZipCallback
{
	public override bool OnPreZip(ZipEntry _entry)
	{
		return true;
	}

	public override void OnPostZip(ZipEntry _entry) { }

	public override void OnFinished(bool _result) { Debug.LogError("zip finished"); }
}

public class MyUnzipCallback : ZipUtility.UnzipCallback
{
	public override bool OnPreUnzip(ZipEntry _entry)
	{
		return true;
	}

	public override void OnPostUnzip(ZipEntry _entry) { }

	public override void OnFinished(bool _result) { Debug.LogError("unzip finished"); }
}