using UnityEngine;
using System.Collections.Generic;
using Lite;



using PacketPair = System.Collections.Generic.KeyValuePair<ushort, Lite.Packet>;

public class NetworkManager : MonoBehaviour, IManager
{
	private SocketClient socketClient;
	static readonly object mLockObject = new object();
	static Queue<PacketPair> mMessageQueue = new Queue<PacketPair>();

	public void Init()
	{
		socketClient = new SocketClient();
		socketClient.Init();
	}


	public void Destroy()
	{
		socketClient.Destroy();
	}

	public static void PushPacket(ushort msgId, Packet packet)
	{
		lock (mLockObject)
		{
			mMessageQueue.Enqueue(new PacketPair(msgId, packet));
		}
	}

	public void Tick()
	{
		if (mMessageQueue.Count > 0)
		{
			while (mMessageQueue.Count > 0)
			{
				PacketPair pair = mMessageQueue.Dequeue();
				Packet packet = pair.Value;
			}
		}
	}

	public void SendConnect()
	{
		socketClient.SendConnect();
	}

	public void SendBytes(ushort msgId, byte[] buffer)
	{
		var bb = new ByteBuffer();
		bb.WriteShort(msgId);
		bb.WriteBytes(buffer);
		socketClient.SendMessage(bb);
	}

	public void SendString(ushort msgId, string str)
	{
		var bb = new ByteBuffer();
		bb.WriteShort(msgId);
		bb.WriteString(str);
		socketClient.SendMessage(bb);
	}

}
