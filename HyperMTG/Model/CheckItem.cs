using HyperMTG.Helper;

namespace HyperMTG.Model
{
	public class CheckItem<T> : ObservableObject
	{
		private bool _isChecked;

		/// <summary>
		///     Initializes a new instance of the CheckItem class.
		/// </summary>
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
				RaisePropertyChanged("IsChecked");
			}
		}
	}
}