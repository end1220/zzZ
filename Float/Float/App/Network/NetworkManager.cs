
using System.Collections.Generic;
using Lite;


using PacketPair = System.Collections.Generic.KeyValuePair<ushort, Lite.Packet>;

public class NetworkManager
{
	private UDPServer server;
	static readonly object lockObject = new object();
	static Queue<PacketPair> messageQueue = new Queue<PacketPair>();

	public void Init()
	{
		server = new UDPServer();
		server.Init();
		//udpClient.Run(AppDefine.remoteIP, AppDefine.remotePort, AppDefine.listenPort);
	}

	public void Destroy()
	{
		server.Destroy();
	}

	public static void PushPacket(ushort msgId, Packet packet)
	{
		lock (lockObject)
		{
			messageQueue.Enqueue(new PacketPair(msgId, packet));
		}
	}

	public void Tick()
	{
		if (messageQueue.Count > 0)
		{
			while (messageQueue.Count > 0)
			{
				PacketPair pair = messageQueue.Dequeue();
				Packet packet = pair.Value;
			}
		}
	}

	public void SendBytes(ushort msgId, byte[] buffer)
	{
		ByteBuffer bb = new ByteBuffer();
		bb.WriteShort(msgId);
		bb.WriteBytes(buffer);
		server.Send(bb.ToBytes());
		bb.Close();
	}

}
