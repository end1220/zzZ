
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;


namespace Lite
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

		public void Init()
		{
			ipEnd = new IPEndPoint(IPAddress.Any, AppDefine.listenPort);
			socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
			socket.Bind(ipEnd);

			IPEndPoint sender = new IPEndPoint(IPAddress.Any, 0);
			clientEnd = (EndPoint)sender;
			Log.Error("waiting for UDP dgram");

			connectThread = new Thread(new ThreadStart(Receive));
			connectThread.Start();
		}

		public void Destroy()
		{
			Quit();
		}

		void Receive()
		{
			while (true)
			{
				recvData = new byte[1024];
				recvLen = socket.ReceiveFrom(recvData, ref clientEnd);
				Log.Error("message from: " + clientEnd.ToString());
				recvStr = Encoding.ASCII.GetString(recvData, 0, recvLen);
				Log.Error("我是服务器，接收到客户端的数据" + recvStr);

				sendStr = "From Server: " + recvStr;
				Send(sendStr);
			}
		}

		public void Send(byte[] bytes)
		{
			string sendStr = "#333666#";
			bytes = Encoding.ASCII.GetBytes(sendStr);
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

	}

}