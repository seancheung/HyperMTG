using System;
using HyperKore.Common;
using HyperMTGMain.Helper;

namespace HyperMTGMain.Model
{
	public class ProgressCheck : ObservableClass
	{
		private bool _isChecked;
		private bool _isProcessing;
		private int _max;
		private int _prog;

		public ProgressCheck(bool isChecked, Set content)
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
				OnPropertyChanged("IsProcessing");
			}
		}

		public int Prog
		{
			get { return _prog; }
			set
			{
				_prog = value;
				OnPropertyChanged("Prog");
			}
		}

		public int Max
		{
			get { return _max; }
			set
			{
				_max = value;
				OnPropertyChanged("Max");
			}
		}

		public Set Content { get; private set; }

		public string Group
		{
			get { return Content.Group; }
		}

		public bool IsLocal
		{
			get { return Content.Local; }
			set
			{
				Content.Local = value;
				Content.LastUpdate = DateTime.Now;
				OnPropertyChanged("IsLocal");
			}
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