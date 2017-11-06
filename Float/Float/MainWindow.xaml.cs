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
		WindowState ws;
		WindowState wsl;
		NotifyIcon notifyIcon;


		public MainWindow()
		{
			this.Activated += OnActivated;
			this.Closing += OnClosing;
			this.ContentRendered += OnContentRendered;
			this.Deactivated += OnDeactivated;
			this.Loaded += OnLoaded;
			this.Closed += OnClosed;
			this.Unloaded += OnUnloaded;
			this.SourceInitialized += OnSourceInitialized;

			InitializeComponent();

			//显示托盘。
			icon();
			//保证窗体显示在上方。
			wsl = WindowState;
		}

		private void icon()
		{
			this.notifyIcon = new NotifyIcon();
			this.notifyIcon.BalloonTipText = "Hello, 文件监视器"; //设置程序启动时显示的文本
			this.notifyIcon.Text = "文件监视器";//最小化到托盘时，鼠标点击时显示的文本
			this.notifyIcon.Icon = new System.Drawing.Icon("float.ico");//程序图标
			this.notifyIcon.Visible = true;
			notifyIcon.MouseDoubleClick += OnNotifyIconDoubleClick;
			this.notifyIcon.ShowBalloonTip(1000);

			System.Windows.Forms.MenuItem m1 = new System.Windows.Forms.MenuItem("open");
			m1.Click += m1_Click;
			System.Windows.Forms.MenuItem m2 = new System.Windows.Forms.MenuItem("close");
			m2.Click += m2_Click;
			System.Windows.Forms.MenuItem[] m = new System.Windows.Forms.MenuItem[] { m1, m2 };
			this.notifyIcon.ContextMenu = new System.Windows.Forms.ContextMenu(m);
		}

		private void OnNotifyIconDoubleClick(object sender, EventArgs e)
		{
			this.Show();
			WindowState = wsl;
		}

		private void Window_StateChanged(object sender, EventArgs e)
		{
			ws = WindowState;
			if (ws == WindowState.Minimized)
			{
				this.Hide();
			}
		}

		void m2_Click(object sender, EventArgs e)
		{
			if (System.Windows.MessageBox.Show("sure to exit?",
											   "application",
												MessageBoxButton.YesNo,
												MessageBoxImage.Question,
												MessageBoxResult.No) == MessageBoxResult.Yes)
			{
				System.Windows.Application.Current.Shutdown();
			}
		}

		void m1_Click(object sender, EventArgs e)
		{
			this.Show();
			this.Activate();
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

		void OnDeactivated(object sender, EventArgs e)
		{
			//Console.WriteLine("Deactivated！");
		}

		void OnContentRendered(object sender, EventArgs e)
		{
			//Console.WriteLine("ContentRendered！");
		}

		void OnClosing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			//Console.WriteLine("---Closing！");
			e.Cancel = true;
			this.WindowState = WindowState.Minimized;
			ws = WindowState.Minimized;
			this.notifyIcon.Visible = true;
			this.notifyIcon.ShowBalloonTip(30, "注意", "大家好，这是一个事例", ToolTipIcon.Info);
		}

		void OnActivated(object sender, EventArgs e)
		{
			//Console.WriteLine("2---Activated！");
		}

		private void OnOKButtonClick(object sender, RoutedEventArgs e)
		{
			Console.WriteLine(this.CurrentModelImage.Name);
		}

		private void OnCalcelButtonClick(object sender, RoutedEventArgs e)
		{

		}
	}

}
