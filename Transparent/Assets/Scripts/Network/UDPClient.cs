
using System.IO;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;


namespace Float
{

	public class UDPClient : NetClient
	{
		Socket socket;
		EndPoint serverEnd;
		IPEndPoint ipEnd;
		byte[] recvData = new byte[1024];
		int recvLen;
		Thread connectThread;
		private MemoryStream memStream;
		private BinaryReader reader;

		public override void Init()
		{
			ipEnd = new IPEndPoint(IPAddress.Parse(AppConst.remoteIP), AppConst.remotePort);
			socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

			IPEndPoint sender = new IPEndPoint(IPAddress.Any, 0);
			serverEnd = (EndPoint)sender;
			//Log.Error("waiting for sending UDP dgram");

			SendConnect();

			connectThread = new Thread(new ThreadStart(Receive));
			connectThread.Start();

			memStream = new MemoryStream();
			reader = new BinaryReader(memStream);
		}

		public override void Destroy()
		{
			Quit();
			reader.Close();
			memStream.Close();
		}

		public override void Send(byte[] bytes)
		{
			MemoryStream ms = null;
			using (ms = new MemoryStream())
			{
				ms.Position = 0;
				BinaryWriter writer = new BinaryWriter(ms);
				ushort msglen = (ushort)bytes.Length;
				writer.Write(msglen);
				writer.Write(bytes);
				writer.Flush();
				byte[] payload = ms.ToArray();
				socket.SendTo(payload, payload.Length, SocketFlags.None, ipEnd);
			}
		}

		void SendConnect()
		{
			ushort msgId = (ushort)CommandId.Connect;
			Command cmd = new Command();
			byte[] bytes = ProtobufUtil.Serialize<Command>(cmd);
			ByteBuffer bb = new ByteBuffer();
			bb.WriteShort(msgId);
			bb.WriteBytes(bytes);
			byte[] array = bb.ToBytes();
			Send(array);
			bb.Close();
			//socket.SendTo(bytes, bytes.Length, SocketFlags.None, ipEnd);
		}

		void Receive()
		{
			while (true)
			{
				System.Array.Clear(recvData, 0, recvData.Length);
				recvLen = socket.ReceiveFrom(recvData, ref serverEnd);
				OnRecv(recvData, recvLen);
				//Log.Error("message from: " + serverEnd.ToString());
				//recvStr = Encoding.ASCII.GetString(recvData, 0, recvLen);
				//Log.Error("recv: " + recvStr);
			}
		}

		void Quit()
		{
			if (connectThread != null)
			{
				connectThread.Interrupt();
				connectThread.Abort();
			}
			if (socket != null)
				socket.Close();
		}

		/*void OnGUI()
		{
			editString = GUI.TextField(new Rect(10, 10, 100, 20), editString);
			if (GUI.Button(new Rect(10, 30, 60, 20), "send"))
				Send(editString);
		}*/

		void OnRecv(byte[] bytes, int length)
		{
			memStream.Seek(0, SeekOrigin.End);
			memStream.Write(bytes, 0, length);
			//Reset to beginning
			memStream.Seek(0, SeekOrigin.Begin);
			while (GetRemainingBytesCount() > 2)
			{
				ushort messageLen = reader.ReadUInt16();
				if (GetRemainingBytesCount() >= messageLen)
				{
					MemoryStream ms = new MemoryStream();
					BinaryWriter writer = new BinaryWriter(ms);
					writer.Write(reader.ReadBytes(messageLen));
					ms.Seek(0, SeekOrigin.Begin);
					HandleReceivedMessage(ms);
				}
				else
				{
					//Back up the position two bytes
					memStream.Position = memStream.Position - 2;
					break;
				}
			}
			//Create a new stream with any leftover bytes
			byte[] leftover = reader.ReadBytes((int)GetRemainingBytesCount());
			memStream.SetLength(0);     //Clear
			memStream.Write(leftover, 0, leftover.Length);
		}

		private long GetRemainingBytesCount()
		{
			return memStream.Length - memStream.Position;
		}

		private void HandleReceivedMessage(MemoryStream ms)
		{
			BinaryReader r = new BinaryReader(ms);
			byte[] message = r.ReadBytes((int)(ms.Length - ms.Position));

			ByteBuffer buffer = new ByteBuffer(message);
			Packet packet = new Packet();
			packet.length = (ushort)message.Length;
			packet.msgId = buffer.ReadShort();
			packet.stamp = 0;
			packet.data = buffer.ReadBytes();
			NetworkManager.PushPacket(packet.msgId, packet);
		}

	}

}