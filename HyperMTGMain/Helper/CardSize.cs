namespace HyperMTGMain.Helper
{
	public class CardSize : ObservableClass
	{
		private readonly double _height;
		private readonly double _width;
		private double _ratio;

		public CardSize()
		{
			_width = 312;
			_height = 445;
		}

		public double Width
		{
			get { return _width*Ratio; }
		}

		public double Height
		{
			get { return _height*Ratio; }
		}

		public double Ratio
		{
			get { return _ratio; }
			set
			{
				if (value > 1 || value < 0)
				{
					return;
				}
				_ratio = value;
				OnPropertyChanged("Width");
				OnPropertyChanged("Height");
			}
		}
	}
}