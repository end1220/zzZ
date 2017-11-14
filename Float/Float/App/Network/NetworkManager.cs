
using System.IO;
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
		CommandHandler.Register();
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
				CommandHandler.Handle(packet);
			}
		}
	}

	public void SendBytes(ushort msgId, byte[] bytes)
	{
		/*ByteBuffer bb = new ByteBuffer();
		bb.WriteShort((ushort)(bytes.Length + 2));
		bb.WriteShort(msgId);
		bb.WriteBytes(bytes);
		byte[] array = bb.ToBytes();
		server.Send(array);
		bb.Close();*/

		using (MemoryStream ms = new MemoryStream())
		{
			ms.Position = 0;
			BinaryWriter writer = new BinaryWriter(ms);
			int msglen = bytes.Length + sizeof(ushort);
			writer.Write((ushort)msglen);
			writer.Write(msgId);
			writer.Write(bytes);
			writer.Flush();

			byte[] array = ms.ToArray();
			server.Send(array);
		}
	}

}
