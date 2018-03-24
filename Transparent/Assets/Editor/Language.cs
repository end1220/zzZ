using System.Collections.Generic;


public enum LangType
{
	English,
	Chinese,
	Japanese,
	Korean
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

public static class TextID
{
	public const int Title = 1;
	public const int Desc = 2;
	public const int Preview = 3;
	public const int Tag = 4;
	public const int Visib = 5;

	public const int Output = 50;
	public const int model = 51;
	public const int prefab = 52;

	public const int wndTitle = 100;
	public const int mainHelpBox = 101;
	public const int language = 102;
	public const int export = 103;
	public const int submit = 104;
	public const int errorTitle = 105;
	public const int steamInitError = 106;
	public const int ok = 107;
	public const int cancel = 108;
	public const int legal = 109;
	public const int accept = 110;
}

public static class Language
{
	public static LangType langType = LangType.Chinese;

	private static LangData[] datas = new LangData[]
	{
		new LangData(1, "Title", "标题"),
		new LangData(2, "Description", "描述"),
		new LangData(3, "Preview", "预览图"),
		new LangData(4, "Tag", "标签"),
		new LangData(5, "Visibility", "可见度"),

		new LangData(50, "Output", "输出路径"),
		new LangData(51, "Model", "模型路径"),
		new LangData(52, "Prefab", "预制路径"),

		new LangData(100, "Float Creator", "Float创作与发布"),
		new LangData(101, "This is where you: \n1. build mods\n 2. submit to steam workshop", "在这里你\n：1.制作mod\n2. 上传到steam创意工坊"),
		new LangData(102, "Language", "语言"),
		new LangData(103, "Export Model", "导出生成"),
		new LangData(104, "Submit", "提交分享"),
		new LangData(105, "Error", "出错啦"),
		new LangData(106, "Please make sure your Steam client is runnning before using Float Creator", "请首先开启并登录Steam再使用Float Creator"),
		new LangData(107, "Ok", "确定"),
		new LangData(108, "Cancel", "取消"),
		new LangData(109, "Workshop Legal Agreement", "《创意工坊服务条款》"),
		new LangData(110, "Accept", "接受"),
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

	public static string Get(int id)
	{
		LazyInit();
		LangData data = null;
		if (dataDic.TryGetValue(id, out data))
		{
			switch (langType)
			{
				case LangType.Chinese:
					return data.zh;
				case LangType.English:
					return data.en;
				case LangType.Japanese:
					return string.IsNullOrEmpty(data.jp) ? data.en : data.jp;
				case LangType.Korean:
					return string.IsNullOrEmpty(data.kr) ? data.en : data.kr;
			}
		}
		return "**";
	}
}