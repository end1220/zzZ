using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Forms; // NotifyIcon control
using System.Drawing; // Icon


namespace Float
{
	/// <summary>
	/// MainWindow.xaml 的交互逻辑
	/// </summary>
	public partial class MainWindow : Window
	{
		WindowState lastState;
		NotifyIcon notifyIcon;


		public MainWindow()
		{
			this.SourceInitialized += OnSourceInitialized;
			this.Activated += OnActivated;
			this.Deactivated += OnDeactivated;
			this.Loaded += OnLoaded;
			this.Unloaded += OnUnloaded;
			this.Closing += OnClosing;
			this.Closed += OnClosed;
			this.StateChanged += OnStateChanged;

			InitializeComponent();

			CreateNotifyIcon();
			lastState = WindowState;
			this.Visibility = Visibility.Hidden;
		}

		void OnActivated(object sender, EventArgs e)
		{
			//Console.WriteLine("2---Activated！");
		}

		void OnDeactivated(object sender, EventArgs e)
		{
			//Console.WriteLine("Deactivated！");
		}

		void OnSourceInitialized(object sender, EventArgs e)
		{
			//Console.WriteLine("1---SourceInitialized！");
		}

		void OnUnloaded(object sender, RoutedEventArgs e)
		{
			//Console.WriteLine("Unloaded！");
		}

		void OnClosed(object sender, EventArgs e)
		{
			//Console.WriteLine("_Closed！");
		}

		void OnLoaded(object sender, RoutedEventArgs e)
		{
			//Console.WriteLine("3---Loaded！");
		}

		void OnClosing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			e.Cancel = true;
			this.WindowState = WindowState.Minimized;
		}

		private void OnStateChanged(object sender, EventArgs e)
		{
			if (WindowState == WindowState.Minimized)
			{
				this.Hide();
				this.notifyIcon.Visible = true;
				this.notifyIcon.ShowBalloonTip(5, "Note", "Show Main Window from here", ToolTipIcon.Info);
			}
		}

		private void OnOKButtonClick(object sender, RoutedEventArgs e)
		{
			Console.WriteLine(this.CurrentModelImage.Name);
		}

		private void OnCalcelButtonClick(object sender, RoutedEventArgs e)
		{

		}

		#region notify icon

		private void CreateNotifyIcon()
		{
			this.notifyIcon = new NotifyIcon();
			this.notifyIcon.BalloonTipText = "Floating fun";
			this.notifyIcon.Text = "Floating icon";
			this.notifyIcon.Icon = new Icon("float.ico");
			this.notifyIcon.Visible = true;
			notifyIcon.MouseDoubleClick += OnNotifyIconDoubleClick;
			this.notifyIcon.ShowBalloonTip(1000);

			System.Windows.Forms.MenuItem m1 = new System.Windows.Forms.MenuItem("Show Main Window");
			m1.Click += OnClickNotifyOpenMainWindow;
			System.Windows.Forms.MenuItem m2 = new System.Windows.Forms.MenuItem("Exit");
			m2.Click += OnClickNotifyExit;
			System.Windows.Forms.MenuItem m3 = new System.Windows.Forms.MenuItem("Send");
			m3.Click += OnClickSend;
			System.Windows.Forms.MenuItem[] menu = new System.Windows.Forms.MenuItem[] { m1, m2, m3 };
			this.notifyIcon.ContextMenu = new System.Windows.Forms.ContextMenu(menu);
		}

		private void OnNotifyIconDoubleClick(object sender, EventArgs e)
		{
			this.Show();
			WindowState = lastState;
		}

		void OnClickNotifyExit(object sender, EventArgs e)
		{
			/*if (System.Windows.MessageBox.Show("sure to exit?", "application",
												MessageBoxButton.YesNo,
												MessageBoxImage.Question,
												MessageBoxResult.No) == MessageBoxResult.Yes)
			{
				System.Windows.Application.Current.Shutdown();
			}*/
			this.notifyIcon.Visible = false;
			System.Windows.Application.Current.Shutdown();
		}

		void OnClickNotifyOpenMainWindow(object sender, EventArgs e)
		{
			this.Show();
			WindowState = lastState;
		}

		void OnClickSend(object sender, EventArgs e)
		{
			Command cmd = new Command();
			cmd.cmdType = (ushort)CommandId.ShowWindow;
			cmd.number1 = 1;
			cmd.number2 = 22;
			cmd.number3 = 33;
			cmd.string1 = "s1111";
			cmd.string2 = "s2222";
			cmd.string3 = "s3333";
			App.Instance.SendCommand(CommandId.ShowWindow, cmd);
		}


		#endregion

	}

}
