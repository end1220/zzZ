
using System.ComponentModel;
using System.Windows.Input;

namespace Float
{
	public partial class ModelData : INotifyPropertyChanged
	{
		public long Id
		{
			get { return workshopId; }
			set
			{
				workshopId = value;
				NotifyPropertyChanged("Id");
			}
		}

		public string Name
		{
			get { return title; }
			/*set
			{
				title = value;
				NotifyPropertyChanged("Name");
			}*/
		}

		public string Title
		{
			get { return title; }
// 			set
// 			{
// 				title = value;
// 				NotifyPropertyChanged("Title");
// 			}
		}

		public string Author
		{
			get { return author; }
// 			set
// 			{
// 				author = value;
// 				NotifyPropertyChanged("Author");
// 			}
		}

		public string PreviewImage
		{
			get { return AppConst.PersistentDataPath + workshopId + "/" + preview; }
			set
			{
				//preview = value;
				NotifyPropertyChanged("PreviewImage");
			}
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
