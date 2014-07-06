using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace Shane.Church.WhatIEat.WP.Helpers
{
	public class NegativeNumberConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			double val = 0;
			if (isNumeric(value))
			{
				return -System.Convert.ToDouble(value);
			}
			else if (Double.TryParse(value.ToString(), out val))
			{
				return -val;
			}
			throw new ArgumentException("Value is not numeric", "value");
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotSupportedException();
		}

		private bool isNumeric(object value)
		{
			return value is sbyte
					|| value is byte
					|| value is short
					|| value is ushort
					|| value is int
					|| value is uint
					|| value is long
					|| value is ulong
					|| value is float
					|| value is double
					|| value is decimal;
		}
	}
}
