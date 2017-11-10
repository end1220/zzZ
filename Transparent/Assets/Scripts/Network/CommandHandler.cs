
using System.Collections.Generic;
using Lite;

public class CommandHandler
{
	public const ushort Connect = 101;
	public const ushort Exception = 102;
	public const ushort Disconnect = 103;
	public const ushort ShowWindow = 201;
	public const ushort HideWindow = 202;
	public const ushort ChangeAlpha = 203;
	public const ushort ChangeSize = 204;
	public const ushort PlayThisOne = 205;

	delegate void HandleMethod(byte[] data);

	private static Dictionary<ushort, HandleMethod> handles = new Dictionary<ushort, HandleMethod>();

	public static void Register()
	{
		handles.Add(ShowWindow, OnShowWindow);
		// ...
	}

	public static void Handle(Packet packet)
	{
		ushort id = packet.msgId;
		HandleMethod func;
		if (handles.TryGetValue(id, out func))
			func(packet.data);
	}

	static void OnShowWindow(byte[] data)
	{

	}
}
