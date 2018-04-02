
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Float
{
	public class UDPServer
	{
		Socket socket;
		EndPoint clientEnd;
		IPEndPoint ipEnd;
		string recvStr;
		string sendStr;
		byte[] recvData = new byte[1024];
		byte[] sendData = new byte[1024];
		int recvLen;
		Thread connectThread;
		private MemoryStream memStream;
		private BinaryReader reader;

		public void Init()
		{
			ipEnd = new IPEndPoint(IPAddress.Any, AppConst.listenPort);
			socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
			socket.Bind(ipEnd);

			IPEndPoint sender = new IPEndPoint(IPAddress.Any, 0);
			clientEnd = (EndPoint)sender;
			Log.Error("waiting for UDP dgram");

			connectThread = new Thread(new ThreadStart(Receive));
			connectThread.Start();
			memStream = new MemoryStream();
			reader = new BinaryReader(memStream);
		}

		public void Destroy()
		{
			Quit();
			reader.Close();
			memStream.Close();
		}

		void Receive()
		{
			while (true)
			{
				System.Array.Clear(recvData, 0, recvData.Length);
				recvLen = socket.ReceiveFrom(recvData, ref clientEnd);
				OnRecv(recvData, recvLen);
				//Log.Error("message from: " + clientEnd.ToString());
				//recvStr = Encoding.ASCII.GetString(recvData, 0, recvLen);
				//Log.Error("我是服务器，接收到客户端的数据" + recvStr);
				//sendStr = "From Server: " + recvStr;
				//Send(sendStr);
			}
		}

		public void Send(byte[] bytes)
		{
			//string sendStr = "#333666#";
			//bytes = Encoding.ASCII.GetBytes(sendStr);
			socket.SendTo(bytes, bytes.Length, SocketFlags.None, clientEnd);
		}

		public void Send(string sendStr)
		{
			sendData = new byte[1024];
			sendData = Encoding.ASCII.GetBytes(sendStr);
			socket.SendTo(sendData, sendData.Length, SocketFlags.None, clientEnd);
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
			Log.Error("disconnect");
		}

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