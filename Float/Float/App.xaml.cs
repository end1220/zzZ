using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using Lite;

namespace Float
{
	/// <summary>
	/// App.xaml 的交互逻辑
	/// </summary>
	public partial class App : Application
	{
		public static App Instance { get { return Current as App; } }

		private DispatcherTimer dispatcherTimer = new DispatcherTimer();
		private NetworkManager network = new NetworkManager();

		private void Init()
		{
			network.Init();
			dispatcherTimer.Tick += new EventHandler(Tick);
			dispatcherTimer.Interval = TimeSpan.FromMilliseconds(500);
			dispatcherTimer.Start();
		}

		private void Tick(object sender, EventArgs e)
		{
			network.Tick();
		}

		public void Destory()
		{
			network.Destroy();
			dispatcherTimer.Stop();
			Shutdown();
			//Environment.Exit(0);
		}

		public void SendCommand(CommandId id, Command cmd)
		{
			byte[] bytes = ProtobufUtil.Serialize<Command>(cmd);
			network.SendBytes((ushort)id, bytes);
		}

		private void AppStartup(object sender, StartupEventArgs e)
		{
			Init();
		}

		private void AppExit(object sender, ExitEventArgs e) { }
		private void AppActivated(object sender, EventArgs e) { }
		private void AppDeactivated(object sender, EventArgs e) { }
	}
}
