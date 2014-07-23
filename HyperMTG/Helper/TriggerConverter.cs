using System;
using System.Globalization;
using System.Windows.Data;

namespace HyperMTG.Helper
{
	public class TriggerConverter : IMultiValueConverter
	{
		#region IMultiValueConverter Members

		public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
		{
			// First value is target value.
			// All others are update triggers only.
			if (values.Length < 1) return Binding.DoNothing;
			return values[0];
		}

		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}

		#endregion
	}
}