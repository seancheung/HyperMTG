using System.Windows;

//http://blog.csdn.net/heredes/article/details/6277009

namespace HyperMTG.Helper
{
	public class DataContextProxy : DependencyObject
	{
		public static readonly DependencyProperty DataContextProperty =
			DependencyProperty.Register(
				"DataContext",
				typeof (object),
				typeof (DataContextProxy),
				new PropertyMetadata(null));

		public object DataContext
		{
			get { return GetValue(DataContextProperty); }
			set { SetValue(DataContextProperty, value); }
		}
	}
}