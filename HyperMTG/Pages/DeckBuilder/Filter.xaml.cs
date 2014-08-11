using HyperMTG.ViewModel;

namespace HyperMTG.Pages
{
	/// <summary>
	///     Interaction logic for Filter.xaml
	/// </summary>
	public partial class Filter
	{
		public Filter()
		{
			DataContext = FilterViewModel.Instance;

			InitializeComponent();
		}
	}
}