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
using Steamworks;

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

			FloatApp.MsgSystem.Register(AppConst.MSG_ITEM_DOWNLOADED, OnItemDownloaded);
			FloatApp.MsgSystem.Register(AppConst.MSG_ITEM_INSTALLED, OnItemInstalled);
			FloatApp.MsgSystem.Register(AppConst.MSG_MODEL_LIST_READY, OnModelListReady);
			FloatApp.MsgSystem.Register(AppConst.MSG_MODEL_LIST_UPDATE, OnModelListUpdate);
		}

		private void OnItemDownloaded(object[] args)
		{
			PublishedFileId_t fileId = (PublishedFileId_t)args[0];
			Log.Info("Home.OnItemDownloaded fileID " + fileId);
		}

		private void OnItemInstalled(object[] args)
		{
			PublishedFileId_t fileId = (PublishedFileId_t)args[0];
			Log.Info("Home.OnItemInstalled fileID " + fileId);
		}

		private void OnModelListReady(object[] args)
		{
			RefreshListbox();
		}

		private void OnModelListUpdate(object[] args)
		{
			RefreshListbox();
		}

		public void RefreshListbox()
		{
			List<ModelData> videos = FloatApp.DataManager.ModelList;
			listBoxModels.ItemsSource = videos;
			Log.Info("RefreshListbox");
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
