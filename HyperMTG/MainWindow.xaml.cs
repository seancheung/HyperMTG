using FirstFloor.ModernUI.Presentation;
using HyperMTG.Properties;

namespace HyperMTG
{
	/// <summary>
	///     Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow
	{
		public MainWindow()
		{
			//Load theme setting
			AppearanceManager.Current.AccentColor = Settings.Default.AccentColor;
			AppearanceManager.Current.FontSize = Settings.Default.FontSize;
			AppearanceManager.Current.ThemeSource = Settings.Default.Theme;

			InitializeComponent();
		}
	}
}