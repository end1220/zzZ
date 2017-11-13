
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;


namespace Lite
{

	public class UDPClient : NetClient
	{
		public class UdpState
		{
			public UdpClient udpClient;
			public IPEndPoint ipEndPoint;
			public const int BufferSize = 1024;
			public byte[] buffer = new byte[BufferSize];
			public int counter = 0;
		}

		private IPEndPoint localPoint = null;
		private IPEndPoint remotePoint = null;
		private UdpClient udpReceive = null;
		private UdpClient udpSend = null;
		
		UdpState receiveState = null;
		UdpState sendState = null;
		
		private ManualResetEvent sendDone = new ManualResetEvent(false);
		private ManualResetEvent receiveDone = new ManualResetEvent(false);


		public override void Init()
		{
			
		}

		public override void Destroy()
		{

		}

		public void Run(string remoteIP, int remotePort, int listenPort)
		{
			IPAddress remoteAddr = IPAddress.Parse(remoteIP);

			localPoint = new IPEndPoint(IPAddress.Any, listenPort);
			remotePoint = new IPEndPoint(remoteAddr, remotePort);
			udpReceive = new UdpClient(localPoint);
			udpSend = new UdpClient();

			receiveState = new UdpState();
			receiveState.udpClient = udpReceive;
			receiveState.ipEndPoint = localPoint;

			sendState = new UdpState();
			sendState.udpClient = udpSend;
			sendState.ipEndPoint = remotePoint;

			Thread t = new Thread(new ThreadStart(Receive));
			t.Start();
		}

		private void Receive()
		{
			Log.Error("listening for messages");
			while (true)
			{
				lock (this)
				{
					IAsyncResult iar = udpReceive.BeginReceive(new AsyncCallback(ReceiveCallback), receiveState);
					receiveDone.WaitOne();
					Thread.Sleep(100);
				}
			}
		}
		
		private void ReceiveCallback(IAsyncResult iar)
		{
			UdpState udpReceiveState = iar.AsyncState as UdpState;
			if (iar.IsCompleted)
			{
				Byte[] bytes = udpReceiveState.udpClient.EndReceive(iar, ref udpReceiveState.ipEndPoint);
				//string receiveString = Encoding.ASCII.GetString(receiveBytes);
				//Console.WriteLine("Received: {0}", receiveString);
				//Thread.Sleep(100);

				ByteBuffer buffer = new ByteBuffer(bytes);
				Packet packet = new Packet();
				packet.length = (ushort)bytes.Length;
				packet.msgId = buffer.ReadShort();
				packet.stamp = 0;
				packet.data = buffer.ReadBytes();
				NetworkManager.PushPacket(packet.msgId, packet);

				receiveDone.Set();
				//SendMsg();
			}
		}
		
		public override void Send(byte[] buffer)
		{
			udpSend.Connect(sendState.ipEndPoint);
			sendState.udpClient = udpSend;
			sendState.counter++;

			//string message = string.Format("第{0}个UDP请求处理完成！", sendState.counter);
			//Byte[] sendBytes = Encoding.Unicode.GetBytes(message);
			udpSend.BeginSend(buffer, buffer.Length, new AsyncCallback(SendCallback), sendState);
			sendDone.WaitOne();
		}
		
		private void SendCallback(IAsyncResult iar)
		{
			UdpState udpState = iar.AsyncState as UdpState;
			Log.Error(string.Format("第{0}个请求处理完毕！", udpState.counter));
			Log.Error(string.Format("number of bytes sent: {0}", udpState.udpClient.EndSend(iar)));
			sendDone.Set();
		}
		
	}

}