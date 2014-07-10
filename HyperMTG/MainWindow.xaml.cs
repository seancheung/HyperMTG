using FirstFloor.ModernUI.Windows.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace HyperMTG
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : ModernWindow
	{
		public MainWindow()
		{
			InitializeComponent();
			//var plugins = HyperKore.IO.IOHandler.Instance.GetPlugins<HyperKore.IO.ICompressor>();
			//foreach (var plugin in plugins)
			//{
			//	MessageBox.Show(plugin.Name + plugin.Description);
			//}
		}
	}
}
