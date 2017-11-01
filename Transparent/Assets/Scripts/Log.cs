
using System;
using System.IO;
using UnityEngine;



public class Log
{
	private static Log _inst;
	public static Log Instance
	{
		get
		{
			if (_inst == null)
			{
				_inst = new Log();
				_inst.Init();
			}
			return _inst;
		}
	}
	const bool EnableLogConsole = true;

	const bool EnableLogFile = true;

	StreamWriter logFile = null;


	void OnDestroy()
	{
		Close();
	}

	public void Info(string text)
	{
		UnityEngine.Debug.Log(text);
		if (EnableLogConsole)
			UnityEngine.Debug.Log(text);
		WriteLog("[Info] " + text);
	}

	public void Warning(string text)
	{
		UnityEngine.Debug.LogWarning(text);
		if (EnableLogConsole)
			UnityEngine.Debug.LogWarning(text);
		WriteLog("[Warning] " + text);
	}

	public void Error(string text)
	{
		UnityEngine.Debug.LogError(text);
		if (EnableLogConsole)
			UnityEngine.Debug.LogError(text);
		WriteLog("[Error] " + text);
	}



	public void Init()
	{
		try
		{
			if (logFile != null)
				return;

			if (EnableLogFile)
			{
				string path = "D:/Float/log/";
				if (!Directory.Exists(path))
					Directory.CreateDirectory(path);

				string filename = string.Format("{0}{1}{2}-{3}-{4}-{5}", DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day,
					DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);
				string logName = Path.Combine(path, filename + ".log");
				FileInfo logInfo = new FileInfo(logName);
				logFile = logInfo.CreateText();
				logFile.AutoFlush = true;
			}

			Info("Log.Init success.");
		}
		catch (Exception e)
		{
			Debug.LogError(e.ToString());
		}
	}

	public void RenderTick()
	{

	}


	public void FrameTick()
	{

	}


	public void Destroy()
	{

	}

	public void WriteLog(string message)
	{
		try
		{
			if (logFile != null)
				logFile.WriteLine(message);
		}
		catch (Exception e)
		{
			Debug.LogError(e.ToString());
		}
	}

	public void Close()
	{
		try
		{
			if (logFile != null)
				logFile.Close();
		}
		catch (Exception e)
		{
			Debug.LogError(e.ToString());
		}
	}

}

