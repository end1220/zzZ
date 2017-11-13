
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
			public UdpClient udp;
			public IPEndPoint endPoint;
			public const int BufferSize = 1024;
			public byte[] buffer = new byte[BufferSize];
			public int counter = 0;
		}

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
			IPEndPoint localPoint = new IPEndPoint(IPAddress.Any, listenPort);
			IPEndPoint remotePoint = new IPEndPoint(remoteAddr, remotePort);

			receiveState = new UdpState();
			receiveState.udp = new UdpClient(localPoint);
			receiveState.endPoint = remotePoint;

			sendState = new UdpState();
			sendState.udp = new UdpClient();
			sendState.endPoint = remotePoint;

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
					IAsyncResult iar = receiveState.udp.BeginReceive(new AsyncCallback(ReceiveCallback), receiveState);
					receiveDone.WaitOne();
					Thread.Sleep(100);
				}
			}
		}

		private void ReceiveCallback(IAsyncResult iar)
		{
			if (iar.IsCompleted)
			{
				Byte[] bytes = receiveState.udp.EndReceive(iar, ref receiveState.endPoint);
				string receiveString = System.Text.Encoding.Unicode.GetString(bytes);
				Log.Error(receiveString);
				Thread.Sleep(100);

				/*ByteBuffer buffer = new ByteBuffer(bytes);
				Packet packet = new Packet();
				packet.length = (ushort)bytes.Length;
				packet.msgId = buffer.ReadShort();
				packet.stamp = 0;
				packet.data = buffer.ReadBytes();
				NetworkManager.PushPacket(packet.msgId, packet);*/

				receiveDone.Set();
				//Send(null);
			}
		}

		public override void Send(byte[] buffer)
		{
			//udpSend.Connect(sendState.ipEndPoint);
			sendState.counter++;

			string message = string.Format("第{0}个UDP请求处理完成！", sendState.counter);
			buffer = System.Text.Encoding.Unicode.GetBytes(message);
			sendState.udp.BeginSend(buffer, buffer.Length, sendState.endPoint, new AsyncCallback(SendCallback), sendState);
			sendDone.WaitOne();
		}

		private void SendCallback(IAsyncResult iar)
		{
			UdpState udpState = iar.AsyncState as UdpState;
			Log.Error(string.Format("第{0}个请求处理完毕！", udpState.counter));
			Log.Error(string.Format("number of bytes sent: {0}", udpState.udp.EndSend(iar)));
			sendDone.Set();
		}

	}

}