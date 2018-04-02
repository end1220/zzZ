
using FirstFloor.ModernUI.Windows.Controls;
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


namespace Float
{
	/// <summary>
	/// MainWindow.xaml 的交互逻辑
	/// </summary>
	public partial class MainWindow : ModernWindow
	{
		Notify notify = new Notify();

		public MainWindow()
		{
			this.Activated += OnActivated;
			this.Deactivated += OnDeactivated;
			this.Closing += OnClosing;
			this.Closed += OnClosed;
			this.StateChanged += OnStateChanged;

			InitializeComponent();

			notify.Create(this);
			this.Visibility = Visibility.Visible;
			this.Visibility = Visibility.Hidden;
		}

		void OnActivated(object sender, EventArgs e)
		{

		}

		void OnDeactivated(object sender, EventArgs e)
		{

		}

		void OnClosing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			e.Cancel = true;
			this.WindowState = WindowState.Minimized;
		}

		void OnClosed(object sender, EventArgs e)
		{
			notify.Destroy();
		}

		private void OnStateChanged(object sender, EventArgs e)
		{
			if (WindowState == WindowState.Minimized)
			{
				this.Hide();
				//notifyIcon.ShowBalloonTip(3, "Note", "Show Main Window from here", ToolTipIcon.Info);
			}
		}

		public void ShowWindow()
		{
			Show();
		}

		public void HideWindow()
		{
			Hide();
		}

	}

}
