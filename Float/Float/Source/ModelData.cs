
using System.ComponentModel;
using System.Windows.Input;

namespace Float
{
	public partial class ModelData : INotifyPropertyChanged
	{
		public long Id
		{
			get { return workshopId; }
		}

		public string Name
		{
			get { return title; }
		}

		public string Title
		{
			get { return title; }
		}

		public string Author
		{
			get { return author; }
		}

		public string PreviewImage
		{
			get { return AppConst.PersistentDataPath + workshopId + "/" + preview; }
		}

		public event PropertyChangedEventHandler PropertyChanged;

		public void NotifyPropertyChanged(string propertyName)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}


		private void Image_MouseLeftButtonUp(object sender, MouseButtonEventArgs args)
		{
			Log.Error("buton up");
		}

	}
}
