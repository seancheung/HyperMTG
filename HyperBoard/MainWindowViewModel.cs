using System;
using System.ComponentModel;
using System.Windows.Threading;

namespace HyperBoard
{
	public class MainWindowViewModel : INotifyPropertyChanged
	{
		private readonly int[] dir;
		private DispatcherTimer timer;

		public MainWindowViewModel()
		{
			Position = new double[2];
			Size = new double[] {525, 350};
			dir = new[] {1, 1};
			timer = new DispatcherTimer {Interval = new TimeSpan(0, 0, 0, 0, 20)};
			timer.Tick += delegate { Update(); };
			timer.Start();
		}

		public double[] Position { get; set; }

		public double[] Size { get; set; }

		public event PropertyChangedEventHandler PropertyChanged;

		private void Update()
		{
			Position[0] += 2*dir[0];
			Position[1] += 1*dir[1];
			OnPropertyChanged("Position");
			if (Position[0] >= 525 - 50 || Position[0] <= 0)
			{
				dir[0] *= -1;
			}
			if (Position[1] >= 350 - 50 || Position[1] <= 0)
			{
				dir[1] *= -1;
			}
		}

		protected virtual void OnPropertyChanged(string propertyName)
		{
			PropertyChangedEventHandler handler = PropertyChanged;
			if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}