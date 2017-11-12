using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace Float
{
	/// <summary>
	/// App.xaml 的交互逻辑
	/// </summary>
	public partial class App : Application
	{
		Send send = new Send();

		public static App Instance
		{
			get
			{
				return Current as App;
			}
		}

		private void AppActivated(object sender, EventArgs e)
		{

		}

		private void AppDeactivated(object sender, EventArgs e)
		{

		}

		private void AppExit(object sender, ExitEventArgs e)
		{

		}

		private void AppStartup(object sender, StartupEventArgs e)
		{

		}
	}
}
