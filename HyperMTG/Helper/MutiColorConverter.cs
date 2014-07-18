using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace HyperMTG.Helper
{
	public class MutiColorConverter : IValueConverter
	{
		#region Implementation of IValueConverter

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			bool flag = (bool) value;
			return flag
				? Color.FromRgb(237, 106, 62)
				: Color.FromRgb(93, 93, 93);
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return null;
		}

		#endregion
	}
}