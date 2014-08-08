using HyperMTG.Helper;

namespace HyperMTG.Model
{
	public class CheckItem : ObservableObject
	{
		private bool? _isChecked;

		/// <summary>
		///     Initializes a new instance of the CheckItem class.
		/// </summary>
		public CheckItem(string content, bool? isChecked)
		{
			Content = content;
			IsChecked = isChecked;
		}

		public string Content { get; set; }

		public bool? IsChecked
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