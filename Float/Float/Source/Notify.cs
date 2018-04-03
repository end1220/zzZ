using System;
using System.Windows;
using System.Windows.Forms; // NotifyIcon control
using System.Drawing; // Icon


namespace Float
{
	public class Notify
	{
		NotifyIcon notifyIcon;
		MainWindow window;

		public void Create(MainWindow window)
		{
			this.window = window;
			notifyIcon = new NotifyIcon();
			notifyIcon.BalloonTipText = "Floating fun";
			notifyIcon.Text = "Floating";
			notifyIcon.Icon = new Icon("float.ico");
			notifyIcon.Visible = true;

			MenuItem m1 = new MenuItem("Show Main Window");
			m1.Click += OnOpenMainWindow;
			MenuItem m2 = new MenuItem("Hide Main Window");
			m2.Click += OnHideMainWindow;
			MenuItem m3 = new MenuItem("Play This");
			m3.Click += OnPlayThis;
			MenuItem m6 = new MenuItem("Exit");
			m6.Click += OnExit;
			MenuItem m10 = new MenuItem("Send");
			m10.Click += OnSend;
			MenuItem m11 = new MenuItem("Rebuild");
			m10.Click += OnRebuildModelList;
			MenuItem m100 = new MenuItem("Test");
			m100.Click += OnTest;
			MenuItem[] menu = new MenuItem[] { m1, m2, m3, m6, m10, m11, m100};
			notifyIcon.ContextMenu = new ContextMenu(menu);
		}

		public void Destroy()
		{
			notifyIcon.Visible = false;
			notifyIcon.Dispose();
		}

		void OnExit(object sender, EventArgs e)
		{
			Destroy();
			App.Instance.Quit();
		}

		void OnOpenMainWindow(object sender, EventArgs e)
		{
			window.ShowWindow();
		}

		void OnHideMainWindow(object sender, EventArgs e)
		{
			window.HideWindow();
		}

		void OnPlayThis(object sender, EventArgs e)
		{
			
		}

		void OnSend(object sender, EventArgs e)
		{
			Command cmd = new Command();
			NetworkManager.Instance.SendCommand(CommandId.ShowWindow, cmd);
		}

		void OnRebuildModelList(object sender, EventArgs e)
		{
			//string modelPath = CustomSettings.Current.ModelPath;
			//ModelDataManager.RebuildModelListAsync(modelPath);
		}

		void OnTest(object sender, EventArgs e)
		{
			//string path = FloatApp.Instance.GetManager<SteamManager>().GetRootPath();
			//Log.Error(path);
		}
	}
}