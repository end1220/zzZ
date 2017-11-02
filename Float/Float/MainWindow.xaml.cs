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
	public partial class MainWindow : Window
	{
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
		}

		void OnSourceInitialized(object sender, EventArgs e)
		{
			Console.WriteLine("1---SourceInitialized！");
		}

		void OnUnloaded(object sender, RoutedEventArgs e)
		{
			Console.WriteLine("Unloaded！");
		}

		void OnClosed(object sender, EventArgs e)
		{
			Console.WriteLine("_Closed！");
		}

		void OnLoaded(object sender, RoutedEventArgs e)
		{
			Console.WriteLine("3---Loaded！");
		}

		void OnDeactivated(object sender, EventArgs e)
		{
			Console.WriteLine("Deactivated！");
		}

		void OnContentRendered(object sender, EventArgs e)
		{
			Console.WriteLine("ContentRendered！");
		}

		void OnClosing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			Console.WriteLine("---Closing！");
		}

		void OnActivated(object sender, EventArgs e)
		{
			Console.WriteLine("2---Activated！");
		}

	}
}
