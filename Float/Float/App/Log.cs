
using System;
using System.IO;



public class Log
{
	private static Log _inst;
	private static Log Instance
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

	public static void Info(string text)
	{
		Instance.Info_(text);
	}

	public static void Warning(string text)
	{
		Instance.Warning_(text);
	}

	public static void Error(string text)
	{
		Instance.Error_(text);
	}

	const bool EnableLogConsole = true;

	const bool EnableLogFile = true;

	StreamWriter logFile = null;


	void OnDestroy()
	{
		Close();
	}

	void Info_(string text)
	{
		Console.WriteLine(text);
		WriteLog("[Info] " + text);
	}

	void Warning_(string text)
	{
		Console.WriteLine(text);
		WriteLog("[Warning] " + text);
	}

	void Error_(string text)
	{
		Console.WriteLine(text);
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
				string path = "D:/Float/wpflog/";
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
			Console.WriteLine(e.ToString());
		}
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
			Console.WriteLine(e.ToString());
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
			Console.WriteLine(e.ToString());
		}
	}

}

