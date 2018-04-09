using UnityEngine;
using System.Collections.Generic;
using Float;



using PacketPair = System.Collections.Generic.KeyValuePair<ushort, Float.Packet>;

public class NetworkManager : IManager
{
	public static NetworkManager Instance { get; private set; }
	static bool UseTcp = false;
	private NetClient client;
	static readonly object lockObject = new object();
	static Queue<PacketPair> messageQueue = new Queue<PacketPair>();

	public override void Init()
	{
		Instance = this;
		CommandHandler.Register();
		if (UseTcp)
		{
			TCPClient tcpClient = new TCPClient();
			tcpClient.Init();
			tcpClient.ConnectServer(AppConst.remoteIP, AppConst.remotePort);
			client = tcpClient;
		}
		else
		{
			UDPClient udpClient = new UDPClient();
			udpClient.Init();
			client = udpClient;
		}
	}


	public override void Destroy()
	{
		client.Destroy();
	}

	public static void PushPacket(ushort msgId, Packet packet)
	{
		lock (lockObject)
		{
			messageQueue.Enqueue(new PacketPair(msgId, packet));
		}
	}

	public override void Tick()
	{
		if (messageQueue.Count > 0)
		{
			while (messageQueue.Count > 0)
			{
				PacketPair pair = messageQueue.Dequeue();
				Packet packet = pair.Value;

				CommandHandler.Handle(packet);
			}
		}
	}

	public void SendBytes(ushort msgId, byte[] buffer)
	{
		ByteBuffer bb = new ByteBuffer();
		bb.WriteShort(msgId);
		bb.WriteBytes(buffer);
		if (AppConst.LocalMode)
		{
			Packet packet = new Packet();
			packet.msgId = msgId;
			packet.data = buffer;
			CommandHandler.Handle(packet);
		}
		else
		{
			client.Send(bb.ToBytes());
		}
		bb.Close();
	}

}
