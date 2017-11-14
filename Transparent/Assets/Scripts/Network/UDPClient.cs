
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;


namespace Lite
{

	public class UDPClient : NetClient
	{
		string editString = "hello wolrd";

		Socket socket;
		EndPoint serverEnd;
		IPEndPoint ipEnd;
		string recvStr;
		string sendStr;
		byte[] recvData = new byte[1024];
		byte[] sendData = new byte[1024];
		int recvLen;
		Thread connectThread;


		public override void Init()
		{
			ipEnd = new IPEndPoint(IPAddress.Parse(AppDefine.remoteIP), AppDefine.remotePort);
			socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

			IPEndPoint sender = new IPEndPoint(IPAddress.Any, 0);
			serverEnd = (EndPoint)sender;
			Log.Error("waiting for sending UDP dgram");

			Send("hello");

			connectThread = new Thread(new ThreadStart(Receive));
			connectThread.Start();
		}

		public override void Destroy()
		{
			Quit();
		}

		void Send(string sendStr)
		{
			sendData = new byte[1024];
			sendData = Encoding.ASCII.GetBytes(sendStr);
			socket.SendTo(sendData, sendData.Length, SocketFlags.None, ipEnd);
		}

		void Receive()
		{
			while (true)
			{
				recvData = new byte[1024];
				recvLen = socket.ReceiveFrom(recvData, ref serverEnd);
				Log.Error("message from: " + serverEnd.ToString());

				recvStr = Encoding.ASCII.GetString(recvData, 0, recvLen);
				Log.Error("recv: " + recvStr);
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

	}

}