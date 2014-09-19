using HyperKore.Common;
using HyperMTGMain.Helper;

namespace HyperMTGMain.Model
{
	public class CheckItem<T> : ObservableClass
	{
		private bool _isChecked;

		public CheckItem(T content, bool isChecked)
		{
			Content = content;
			IsChecked = isChecked;
		}

		public T Content { get; set; }

		public string Name
		{
			get { return Content.ToString(); }
		}

		public bool IsChecked
		{
			get { return _isChecked; }
			set
			{
				_isChecked = value;
				OnPropertyChanged("IsChecked");
			}
		}
	}
}