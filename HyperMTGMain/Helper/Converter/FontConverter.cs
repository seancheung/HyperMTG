﻿using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace HyperMTGMain.Helper.Converter
{
	public class FontConverter : IValueConverter
	{
		#region Implementation of IValueConverter

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return (bool) value
				? FontWeights.Bold
				: FontWeights.Normal;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return null;
		}

		#endregion
	}
}