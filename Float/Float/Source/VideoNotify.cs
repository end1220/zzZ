using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Float
{
	public class VideoNotify : INotifyPropertyChanged
	{
		private string _name;
		private string _id;
		private string _upperson;
		private string _local;
		private string _uptime;
		private string _downcount;
		private string _playcount;
		private string _info;
		private string _type;
		private string _image;
		public event PropertyChangedEventHandler PropertyChanged;
		public string Name
		{
			get { return _name; }
			set
			{
				_name = value;
				NotifyPropertyChanged("Name");
			}
		}

		public string Image
		{
			get { return _image; }
			set
			{
				_image = value;
				NotifyPropertyChanged("Image");
			}
		}
		public string Playcount
		{
			get { return _playcount; }
			set
			{
				_playcount = value;
				NotifyPropertyChanged("Playcount");
			}
		}
		public string Id
		{
			get { return _id; }
			set
			{
				_id = value;
				NotifyPropertyChanged("Id");
			}
		}
		public string Upperson
		{
			get { return _upperson; }
			set { _upperson = value; NotifyPropertyChanged("Upperson"); }
		}
		public string Local
		{
			get { return _local; }
			set { _local = value; NotifyPropertyChanged("Local"); }
		}
		public string Uptime
		{
			get { return _uptime; }
			set { _uptime = value; NotifyPropertyChanged("Uptime"); }
		}
		public string DownCount
		{
			get { return _downcount; }
			set { _downcount = value; NotifyPropertyChanged("Downcount"); }
		}
		public string Info
		{
			get { return _info; }
			set { _info = value; NotifyPropertyChanged("Info"); }
		}
		public string Type
		{
			get { return _type; }
			set { _type = value; NotifyPropertyChanged("Type"); }
		}
		public void NotifyPropertyChanged(string propertyName)
		{
			if (PropertyChanged != null)
			{
				PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}

	}
}