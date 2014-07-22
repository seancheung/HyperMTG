using System;
using System.Globalization;
using System.Resources;
using System.Windows.Data;
using System.Windows.Media;

namespace HyperMTG.Helper
{
	public class MutiColorConverter : IValueConverter
	{
		#region Implementation of IValueConverter

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return (bool) value
				? (System.Convert.ToInt32(parameter) == 0
					? new SolidColorBrush(Color.FromRgb(237, 106, 62))
					: new SolidColorBrush(Color.FromRgb(39, 142, 206)))
				: (System.Convert.ToInt32(parameter) == 0
					? new SolidColorBrush(Color.FromRgb(39, 142, 206))
					: new SolidColorBrush(Color.FromRgb(237, 106, 62)));
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return null;
		}

		#endregion
	}
}