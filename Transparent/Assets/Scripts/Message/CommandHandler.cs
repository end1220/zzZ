

using System.Collections.Generic;
using Lite;
using ProtoBuf;


[ProtoContract]
public class Command
{
	[ProtoMember(1)]
	public ushort cmdType;
	[ProtoMember(2)]
	public int number1;
	[ProtoMember(3)]
	public int number2;
	[ProtoMember(4)]
	public int number3;
	[ProtoMember(5)]
	public string string1;
	[ProtoMember(6)]
	public string string2;
	[ProtoMember(7)]
	public string string3;
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
	delegate void HandleMethod(byte[] data);

	private static Dictionary<CommandId, HandleMethod> handles = new Dictionary<CommandId, HandleMethod>();

	public static void Register()
	{
		handles.Add(CommandId.ShowWindow, OnShowWindow);
		handles.Add(CommandId.HideWindow, OnHideWindow);
		// ...
	}

	public static void Handle(Packet packet)
	{
		CommandId id = (CommandId)packet.msgId;
		HandleMethod func;
		if (handles.TryGetValue(id, out func))
			func(packet.data);
		Log.Error("Handle id : " + id);
	}

	static void OnShowWindow(byte[] data)
	{
		Command cmd = ProtobufUtil.DeSerialize<Command>(data);
		Log.Error("show window");
	}

	static void OnHideWindow(byte[] data)
	{
		Command cmd = ProtobufUtil.DeSerialize<Command>(data);
		Log.Error("hide window");
	}
}
