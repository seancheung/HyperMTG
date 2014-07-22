using System;
using System.Globalization;
using System.Windows.Data;

namespace HyperMTG.Helper
{
	public class TooltipConverter : IValueConverter
	{
		#region Implementation of IValueConverter

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return ((DateTime) value).Year == 1 ? "Last Update Time: Never" : string.Format("Last Update Time: {0}", value);
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return null;
		}

		#endregion
	}
}