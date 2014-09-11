using HyperMTG.Helper;

namespace HyperMTG.Model
{
	public class PageSize : ObservableObject
	{
		private readonly double originalItemHeight = 445;
		private readonly double originalItemWidth = 312;
		private readonly double originalPageHeight = 1335;
		private readonly double originalPageWidth = 936;
		private double _itemHeight;
		private double _itemWidth;
		private double _pageHeight;
		private double _pageWidth;

		public PageSize()
		{
			ItemWidth = 312;
			ItemHeight = 445;
			PageWidth = 936;
			PageHeight = 1335;
		}

		public double ItemWidth
		{
			get { return _itemWidth; }
			set
			{
				_itemWidth = value;
				RaisePropertyChanged("ItemWidth");
			}
		}

		public double ItemHeight
		{
			get { return _itemHeight; }
			set
			{
				_itemHeight = value;
				RaisePropertyChanged("ItemHeight");
			}
		}

		public double PageWidth
		{
			get { return _pageWidth; }
			set
			{
				_pageWidth = value;
				RaisePropertyChanged("PageWidth");
			}
		}

		public double PageHeight
		{
			get { return _pageHeight; }
			set
			{
				_pageHeight = value;
				RaisePropertyChanged("PageHeight");
			}
		}

		/// <summary>
		///     Set display ratio (0.0 ~ 1.0)
		/// </summary>
		/// <param flavor="ratio"></param>
		public void SetRatio(double ratio)
		{
			if (ratio < 0 || ratio > 1)
			{
				return;
			}

			ItemHeight = ratio*originalItemHeight;
			ItemWidth = ratio*originalItemWidth;
			PageHeight = ratio*originalPageHeight;
			PageWidth = ratio*originalPageWidth;
		}
	}
}