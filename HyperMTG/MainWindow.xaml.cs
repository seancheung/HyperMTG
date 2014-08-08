using HyperMTG.Langs;

namespace HyperMTG
{
	/// <summary>
	///     Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow
	{
		public MainWindow()
		{
			App.LanguageManager = new LanguageManager();
			InitializeComponent();
		}
	}
}