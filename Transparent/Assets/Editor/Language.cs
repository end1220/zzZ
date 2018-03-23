using System.Collections.Generic;


public static class TextID
{
	public const int Title = 1;
	public const int Desc = 2;
	public const int Preview = 3;
	public const int Tag = 4;
	public const int Visib = 5;

	public const int Output = 50;
}



public enum LangType
{
	EN,
	ZH,
	JP,
	KR
}

public class LangData
{
	public int id;
	public string en;
	public string zh;
	public string jp;
	public string kr;

	public LangData(int id, string en, string zh, string jp = "", string kr = "")
	{
		this.id = id;
		this.en = en;
		this.zh = zh;
		this.jp = jp;
		this.kr = kr;
	}
}

public static class Language
{
	public static LangType langType = LangType.ZH;

	private static LangData[] datas = new LangData[]
	{
		new LangData(1, "Title", "����"),
		new LangData(2, "Description", "����"),
		new LangData(3, "Preview Image", "Ԥ��ͼ"),
		new LangData(4, "Tag", "��ǩ"),
		new LangData(5, "Visibility", "�ɼ���"),
		new LangData(50, "Output Path", "���"),
	};

	private static Dictionary<int, LangData> dataDic = new Dictionary<int, LangData>();

	private static void LazyInit()
	{
		if (dataDic.Count == 0)
		{
			for (int i = 0; i < datas.Length; ++i)
				dataDic.Add(datas[i].id, datas[i]);
		}
	}

	public static string GetString(int id)
	{
		LazyInit();
		LangData data = null;
		if (dataDic.TryGetValue(id, out data))
		{
			switch (langType)
			{
				case LangType.ZH:
					return data.zh;
				case LangType.EN:
					return data.en;
				case LangType.JP:
					return data.jp;
				case LangType.KR:
					return data.kr;
			}
		}
		return "**";
	}
}