

using System.Collections.Generic;
using Lite;
using ProtoBuf;


[ProtoContract]
public class Command
{
	[ProtoMember(1)]
	public int number1;
	[ProtoMember(2)]
	public int number2;
	[ProtoMember(3)]
	public int number3;
	[ProtoMember(4)]
	public string string1;
	[ProtoMember(5)]
	public string string2;
	[ProtoMember(6)]
	public string string3;

	public Command() { }
	public Command(int n1 = 0, int n2 = 0, int n3 = 0, string s1 = null, string s2 = null, string s3 = null)
	{
		number1 = n1; number2 = n2; number3 = n3;
		string1 = s1; string2 = s2; string3 = s3;
	}
}


public enum CommandId
{
	Connect = 101,
	Exception = 102,
	Disconnect = 103,
	ShowWindow = 201,
	HideWindow = 202,
	ChangeAlpha = 203,
	ChangeSize = 204,
	PlayThisOne = 205
}


public class CommandHandler
{
	delegate void HandleMethod(Command cmd);

	private static Dictionary<CommandId, HandleMethod> handles = new Dictionary<CommandId, HandleMethod>();

	public static void Register()
	{
		handles.Add(CommandId.ShowWindow, OnShowWindow);
		handles.Add(CommandId.HideWindow, OnHideWindow);
		handles.Add(CommandId.PlayThisOne, OnPlayThis);
	}

	public static void Handle(Packet packet)
	{
		CommandId id = (CommandId)packet.msgId;
		HandleMethod func;
		if (handles.TryGetValue(id, out func))
			func(ProtobufUtil.DeSerialize<Command>(packet.data));
		Log.Error("Handle id : " + id);
	}

	static void OnShowWindow(Command cmd)
	{
		Log.Error("show window");
	}

	static void OnHideWindow(Command cmd)
	{
		Log.Error("hide window");
	}

	static void OnPlayThis(Command cmd)
	{
		Log.Error("play this");
		int id = cmd.number1;
		ModelScene.Instance.LoadModel(5139483679854042868);
	}
}
