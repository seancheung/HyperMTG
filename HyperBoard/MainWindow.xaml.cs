using System;
using HyperKore.Game;

namespace HyperBoard
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow
	{
		private MainGame game = new MainGame();
		Subject subject = new Subject();

		public MainWindow()
		{
			InitializeComponent();
			subject.Attach(ShowA);
			subject.Attach(ShowA);
			subject.Attach(ShowA);
			subject.Attach(ShowB);
			subject.Notify();
		}

		private void ShowA()
		{
			Console.WriteLine("A");
		}

		private void ShowB()
		{
			Console.WriteLine("B");
		}
	}
}