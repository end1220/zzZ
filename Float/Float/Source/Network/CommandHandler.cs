

using System.Collections.Generic;
using Float;
using ProtoBuf;


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
	delegate void HandleMethod(CommandId msgId, byte[] data);
	private static Dictionary<CommandId, HandleMethod> handles = new Dictionary<CommandId, HandleMethod>();


	public static void Register()
	{
		handles.Add(CommandId.Connect, OnConnect);
		// ...
	}

	public static void Handle(Packet packet)
	{
		CommandId id = (CommandId)packet.msgId;
		HandleMethod func;
		if (handles.TryGetValue(id, out func))
			func(id, packet.data);
	}

	static void OnConnect(CommandId msgId, byte[] bytes)
	{
		Log.Error("connect one");
	}
}
