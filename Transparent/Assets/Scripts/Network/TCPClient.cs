using UnityEngine;
using System;
using System.IO;
using System.Net.Sockets;


namespace Lite
{
	public enum DisconnectType
	{
		Exception,
		Disconnect,
		ClientClosing,
	}

	public class TCPClient : NetClient
	{
		private TcpClient tcpClient = null;
		private NetworkStream outStream = null;
		private MemoryStream memStream;
		private BinaryReader reader;

		private const int MAX_READ = 8192;
		private byte[] byteBuffer = new byte[MAX_READ];
		public static bool loggedIn = false;


		public override void Init()
		{
			memStream = new MemoryStream();
			reader = new BinaryReader(memStream);
		}

		public override void Destroy()
		{
			this.CloseClient();
			reader.Close();
			memStream.Close();
		}

		public void ConnectServer(string host, int port)
		{
			tcpClient = null;
			tcpClient = new TcpClient();
			tcpClient.SendTimeout = 1000;
			tcpClient.ReceiveTimeout = 1000;
			tcpClient.NoDelay = true;
			try
			{
				tcpClient.BeginConnect(host, port, new AsyncCallback(OnConnectServer), null);
			}
			catch (Exception e)
			{
				CloseClient();
				Debug.LogError(e.Message);
			}
		}

		void OnConnectServer(IAsyncResult asr)
		{
			outStream = tcpClient.GetStream();
			tcpClient.GetStream().BeginRead(byteBuffer, 0, MAX_READ, new AsyncCallback(OnRead), null);
			//NetworkManager.AddEvent(Protocal.Connect, new ByteBuffer());
		}

		void WriteMessage(byte[] message)
		{
			MemoryStream ms = null;
			using (ms = new MemoryStream())
			{
				ms.Position = 0;
				BinaryWriter writer = new BinaryWriter(ms);
				ushort msglen = (ushort)message.Length;
				writer.Write(msglen);
				writer.Write(message);
				writer.Flush();
				if (tcpClient != null && tcpClient.Connected)
				{
					//NetworkStream stream = client.GetStream(); 
					byte[] payload = ms.ToArray();
					outStream.BeginWrite(payload, 0, payload.Length, new AsyncCallback(OnWrite), null);
				}
				else
				{
					Debug.LogError("client.connected----->>false");
				}
			}
		}

		void OnRead(IAsyncResult asr)
		{
			int bytesRead = 0;
			try
			{
				if (tcpClient == null)
				{
					OnDisconnect(DisconnectType.ClientClosing, "");
					return;
				}
				lock (tcpClient.GetStream())
				{
					bytesRead = tcpClient.GetStream().EndRead(asr);
				}
				if (bytesRead < 1)
				{
					OnDisconnect(DisconnectType.Disconnect, "bytesRead < 1");
					return;
				}
				OnRecv(byteBuffer, bytesRead);
				lock (tcpClient.GetStream())
				{
					Array.Clear(byteBuffer, 0, byteBuffer.Length);
					tcpClient.GetStream().BeginRead(byteBuffer, 0, MAX_READ, new AsyncCallback(OnRead), null);
				}
			}
			catch (Exception ex)
			{
				//PrintBytes();
				OnDisconnect(DisconnectType.Exception, ex.Message);
			}
		}

		void OnDisconnect(DisconnectType dis, string msg)
		{
			if (dis != DisconnectType.ClientClosing)
				CloseClient();
			/*ushort protocal = dis == DisconnectType.Exception ? Protocal.Exception : Protocal.Disconnect;

			ByteBuffer buffer = new ByteBuffer();
			buffer.WriteShort((ushort)protocal);
			NetworkManager.PushPacket(protocal, buffer);*/
			Debug.Log("Connection closed. Distype: " + dis + ". Msg: " + msg);
		}

		void PrintBytes()
		{
			string returnStr = string.Empty;
			for (int i = 0; i < byteBuffer.Length; i++)
			{
				returnStr += byteBuffer[i].ToString("X2");
			}
			Debug.LogError(returnStr);
		}

		void OnWrite(IAsyncResult r)
		{
			try
			{
				outStream.EndWrite(r);
			}
			catch (Exception ex)
			{
				Debug.LogError("OnWrite--->>>" + ex.Message);
			}
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

		public void CloseClient()
		{
			if (tcpClient != null)
			{
				if (tcpClient.Connected)
					tcpClient.Close();
				tcpClient = null;
			}
			loggedIn = false;
		}

		public override void Send(byte[] buffer)
		{
			WriteMessage(buffer);
		}
	}
}