using System;
using HyperKore.Common;
using HyperMTG.Helper;

namespace HyperMTG.Model
{
	public class CheckSetItem : ObservableObject
	{
		private bool _isChecked;
		private bool _isProcessing;
		private int _max;
		private int _prog;

		public CheckSetItem(bool isChecked, Set content)
		{
			IsChecked = isChecked;
			Content = content;
		}

		public bool IsProcessing
		{
			get { return _isProcessing; }
			set
			{
				_isProcessing = value;
				RaisePropertyChanged("IsProcessing");
			}
		}

		public int Prog
		{
			get { return _prog; }
			set
			{
				_prog = value;
				RaisePropertyChanged("Prog");
			}
		}

		public int Max
		{
			get { return _max; }
			set
			{
				_max = value;
				RaisePropertyChanged("Max");
			}
		}

		public Set Content { get; set; }

		public bool IsLocal
		{
			get { return Content.Local; }
			set
			{
				Content.Local = value;
				Content.LastUpdate = DateTime.Now;
				RaisePropertyChanged("IsLocal");
			}
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