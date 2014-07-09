using System;
using System.Globalization;
using System.Windows.Data;

namespace HyperMTG.Helper
{
	public class SliderConverter : IMultiValueConverter
	{
		public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
		{
			var min = System.Convert.ToDouble(values[0]);
			var max = System.Convert.ToDouble(values[1]);

			return Math.Min(min, max);
		}

		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
		{
			return null;
		}
	}
}