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
			MenuItem[] menu = new MenuItem[] { m1, m2, m3, m6, m10 };
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
			App.Instance.Destory();
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
			cmd.number1 = 1;
			cmd.number2 = 22;
			cmd.number3 = 33;
			cmd.string1 = "s1111";
			cmd.string2 = "s2222";
			cmd.string3 = "s3333";
			App.Instance.SendCommand(CommandId.ShowWindow, cmd);
		}

	}
}