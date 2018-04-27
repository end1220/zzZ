using System.Collections.Generic;

namespace Float
{

	public enum LangType
	{
		[EnumLabel("English")]
		English,
		[EnumLabel("中文")]
		Chinese,
		[EnumLabel("日本語")]
		Japanese,
		[EnumLabel("한국어")]
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
		public const int visibility = 5;
		public const int category = 6;
		public const int content = 7;
		public const int genre = 8;
		public const int rating = 9;
		public const int required = 19;

		public const int Output = 50;
		public const int model = 51;
		public const int prefab = 52;
		public const int select = 53;
		public const int build = 54;
		public const int refresh = 55;
		public const int selectModelFolder = 56;
		public const int selectPreview = 57;

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
		public const int createItem = 111;
		public const int submitToWorkshop = 112;
		public const int workshopPolicy = 113;
		public const int ackWorkshopPolicy = 114;

		public const int recentProject = 115;
		public const int noProject = 116;
		public const int createProject = 117;
		public const int newProject = 118;

		public const int step1 = 119;
		public const int step2 = 120;
		public const int submitting = 121;
		public const int createFailed = 122;
		public const int error = 123;
		public const int complete = 124;
		public const int submitDone = 125;
		public const int submitFailed = 126;
		public const int titleEmpty = 127;
		public const int descEmpty = 128;
		public const int contentEmpty = 129;
		public const int contentMissing = 130;
		public const int contentInvalid = 131;
		public const int previewMissing = 132;
		public const int PreparingConfig = 133;
		public const int PreparingContent = 134;
		public const int UploadingContent = 135;
		public const int ploadingPreviewFile = 136;
		public const int CommittingChanges = 137;
		public const int titleToLong = 138;
		public const int descToLong = 139;

		public const int Unspecified = 140;
		public const int Scene = 141;
		public const int Video = 142;
		public const int Web = 143;
		public const int App = 144;
		public const int Abstract = 145;
		public const int Animal = 146;
		public const int Cartoon = 147;
		public const int Games = 148;
		public const int Girls = 149;
		public const int Guys = 150;
		public const int Fantacy = 151;
		public const int Nature = 152;
		public const int Music = 153;
		public const int Everybody = 154;
		public const int Suspicious = 155;
		public const int Mature = 156;
		public const int Public = 157;
		public const int FriendsOnly = 158;
		public const int Private = 159;
	}

	public static class Language
	{
		public static LangType langType = LangType.Chinese;

		private static LangData[] datas = new LangData[]
		{
		new LangData(TextID.Title, "Title", "标题"),
		new LangData(TextID.Desc, "Description", "简介"),
		new LangData(TextID.Preview, "Preview", "预览图"),
		new LangData(TextID.Tag, "Tag", "标签"),
		new LangData(TextID.visibility, "Visibility", "可见度"),
		new LangData(TextID.category, "Category", "种类"),
		new LangData(TextID.content, "Content", "内容"),
		new LangData(TextID.genre, "Genre", "分类"),
		new LangData(TextID.rating, "Rating", "评级"),
		new LangData(TextID.required, "*required", "*必填"),

		new LangData(TextID.Output, "Output", "输出路径"),
		new LangData(TextID.model, "Model", "模型路径"),
		new LangData(TextID.prefab, "Prefab", "预制路径"),
		new LangData(TextID.select, "Select", "选择"),
		new LangData(TextID.build, "Build", "打  包"),
		new LangData(TextID.refresh, "Refresh", "刷  新"),
		new LangData(TextID.selectModelFolder, "Select model folder", "选择资源根目录"),
		new LangData(TextID.selectPreview, "Select a preview image", "选择预览图片"),

		new LangData(TextID.wndTitle, "Float Creator", "Float创作与发布"),
		new LangData(TextID.mainHelpBox, "This is where you: \n1. build mods\n 2. submit to steam workshop", "在这里你：\n1.制作mod\n2. 上传到steam创意工坊"),
		new LangData(TextID.language, "Language", "语言"),
		new LangData(TextID.export, "Export Model", "导出生成"),
		new LangData(TextID.submit, "Submit", "提交分享"),
		new LangData(TextID.errorTitle, "Error", "出错啦"),
		new LangData(TextID.steamInitError, "Please ensure Steam is runnning before using Float Creator", "请首先开启并登录Steam再使用Float Creator"),
		new LangData(TextID.ok, "Ok", "确定"),
		new LangData(TextID.cancel, "Cancel", "取消"),
		new LangData(TextID.legal, "Workshop Legal Agreement", "《创意工坊服务条款》"),
		new LangData(TextID.accept, "Accept", "接受"),
		new LangData(TextID.createItem, "Create Item", "创建物品"),
		new LangData(TextID.submitToWorkshop, "Subimit to Workshop", "提交到创意工坊"),
		new LangData(TextID.workshopPolicy, "Workshop Policy", "创意工坊法律协议"),
		new LangData(TextID.ackWorkshopPolicy, "Accept the Workshop Legal Agreement?", "提交物品的同时也表示您同意了《创意工坊服务条款》"),

		new LangData(TextID.recentProject, "Recent projects", "最近的项目"),
		new LangData(TextID.noProject, "    No project", "    尚无..."),
		new LangData(TextID.createProject, "Create projects", "创建新项目"),
		new LangData(TextID.newProject, "New project", "新建项目"),

		new LangData(TextID.step1, "Step 1: Build model asset", "第1步：打包资源"),
		new LangData(TextID.step2, "Step 2: Submit to Workshop", "第2步：提交到创意工坊"),
		new LangData(TextID.submitting, "Submitting", "提交中。。。"),
		new LangData(TextID.createFailed, "Create workshop item failed. Error code : ", "创建新物品失败。错误码："),
		new LangData(TextID.error, "Error", "出错"),
		new LangData(TextID.complete, "Complete", "完成"),
		new LangData(TextID.submitDone, "Submit done! Open a browser window to the Steam Workshop page?", "上传完毕！在浏览器窗口打开Steam创意工坊该物品页面？"),
		new LangData(TextID.submitFailed, "Submit workshop item failed. Error code : ", "上传物品失败。错误码："),

		new LangData(TextID.titleEmpty, "The title cannot be empty.", "标题不能空"),
		new LangData(TextID.titleToLong, "The title exceeds the limit", "标题过长"),
		new LangData(TextID.descEmpty, "The description cannot be empty.", "简介不能空"),
		new LangData(TextID.descToLong, "The description exceeds the limit", "简介过长"),
		new LangData(TextID.contentEmpty, "Content directory cannot be empty.", "内容文件夹不能空"),
		new LangData(TextID.contentMissing, "Content directory does not exist.", "内容文件夹不存在"),
		new LangData(TextID.contentInvalid, "Content directory cannot be empty.", "内容文件夹不完整"),
		new LangData(TextID.previewMissing, "Preview file does not exist.", "预览图丢失"),

		new LangData(TextID.PreparingConfig, "Processing configuration data", "处理配置信息"),
		new LangData(TextID.PreparingContent, "Reading and processing content files", "处理内容"),
		new LangData(TextID.UploadingContent, "Uploading content changes to Steam", "上传内容"),
		new LangData(TextID.ploadingPreviewFile, "Uploading new preview file image", "上传预览图"),
		new LangData(TextID.CommittingChanges, "Committing all changes", "提交修改日志"),

		new LangData(TextID.Unspecified, "Unspecified", "未指定"),
		new LangData(TextID.Scene, "Scene", "场景"),
		new LangData(TextID.Video, "Video", "视频"),
		new LangData(TextID.Web, "Web", "网页"),
		new LangData(TextID.App, "App", "应用"),
		new LangData(TextID.Abstract, "Abstract", "抽象"),
		new LangData(TextID.Animal, "Animal", "动物"),
		new LangData(TextID.Cartoon, "Cartoon", "卡通"),
		new LangData(TextID.Games, "Games", "游戏"),
		new LangData(TextID.Girls, "Girls", "女人"),
		new LangData(TextID.Guys, "Guys", "男人"),
		new LangData(TextID.Fantacy, "Fantacy", "幻想"),
		new LangData(TextID.Nature, "Nature", "自然"),
		new LangData(TextID.Music, "Music", "音乐"),
		new LangData(TextID.Everybody, "Everybody", "适合所有人"),
		new LangData(TextID.Suspicious, "Suspicious", "敏感"),
		new LangData(TextID.Mature, "Mature", "成熟"),
		new LangData(TextID.Public, "Public", "完全公开"),
		new LangData(TextID.FriendsOnly, "FriendsOnly", "仅好友可见"),
		new LangData(TextID.Private, "Private", "仅自己可见"),
		
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

}