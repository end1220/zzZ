using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Lite;

namespace Float
{
	/// <summary>
	/// App.xaml 的交互逻辑
	/// </summary>
	public partial class App : Application
	{
		private NetworkManager network = new NetworkManager();

		public static App Instance
		{
			get
			{
				return Current as App;
			}
		}

		private void AppStartup(object sender, StartupEventArgs e)
		{
			network.Init();
		}

		private void AppExit(object sender, ExitEventArgs e)
		{

		}

		private void AppActivated(object sender, EventArgs e)
		{

		}

		private void AppDeactivated(object sender, EventArgs e)
		{

		}

		public void SendCommand(CommandId id, Command cmd)
		{
			cmd.cmdType = (ushort)id;
			byte[] buffer = ProtobufUtil.Serialize<Command>(cmd);
			var bb = new ByteBuffer();
			bb.WriteShort(cmd.cmdType);
			bb.WriteBytes(buffer);
			byte[] bytes = bb.ToBytes();
			network.SendBytes((ushort)id, bytes);
		}
	}
}
