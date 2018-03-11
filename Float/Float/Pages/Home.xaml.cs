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

namespace Float.Pages
{
	/// <summary>
	/// Interaction logic for Home.xaml
	/// </summary>
	public partial class Home : UserControl
	{
		public ModelData currentModelData;

		public Home()
		{
			InitializeComponent();

			List<ModelData> videos = DataManager.Instance.ModelList;
			listBoxModels.ItemsSource = videos;
		}

		public void SetCurrentModelData(ModelData data)
		{
			currentModelData = data;
			Title2.Content = data.Title;
			Author2.Content = data.Author;
			thumb.Source = new BitmapImage(new Uri(data.PreviewImage, UriKind.Absolute));;
		}


		private void LBMouseDoubleClick(object sender, MouseButtonEventArgs e)
		{
			Log.Error("double");
		}

		private void LBMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			Log.Error("left up");
		}

		private void LBMouseRightButtonUp(object sender, MouseButtonEventArgs e)
		{
			Log.Error("right up");
		}

		private void LBSelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			Log.Error("selection changed");
			ModelData data = (ModelData)listBoxModels.SelectedItem;
			if (data == null)
				return;
			//MessageBox.Show(o.ToString());
			SetCurrentModelData(data);
		}
	}
}
