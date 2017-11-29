
using System.ComponentModel;


public partial class ModelData : INotifyPropertyChanged
{
	public long Id
	{
		get { return id; }
		set
		{
			id = value;
			NotifyPropertyChanged("Id");
		}
	}

	public string Name
	{
		get { return name; }
		set
		{
			name = value;
			NotifyPropertyChanged("Name");
		}
	}

	public string Title
	{
		get { return title; }
		set
		{
			title = value;
			NotifyPropertyChanged("Title");
		}
	}

	public string Author
	{
		get { return author; }
		set
		{
			author = value;
			NotifyPropertyChanged("Author");
		}
	}

	public event PropertyChangedEventHandler PropertyChanged;

	public void NotifyPropertyChanged(string propertyName)
	{
		PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
	}

}
