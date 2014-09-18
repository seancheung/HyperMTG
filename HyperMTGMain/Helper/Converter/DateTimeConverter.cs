using System;
using System.Globalization;
using System.Windows.Data;

namespace HyperMTGMain.Helper.Converter
{
	public class DateTimeConverter : IValueConverter
	{
		#region Implementation of IValueConverter

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return ((DateTime) value).Year == 1 ? "Never" : value.ToString();
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return null;
		}

		#endregion
	}
}