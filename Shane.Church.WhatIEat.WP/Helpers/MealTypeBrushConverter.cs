using Shane.Church.WhatIEat.Core.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows.Media;

namespace Shane.Church.WhatIEat.WP.Helpers
{
	public class MealTypeBrushConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			if (value != null && value is MealType)
			{
				switch ((MealType)value)
				{
					case MealType.Breakfast:
						return new SolidColorBrush(Color.FromArgb(255, 143, 111, 173));
					case MealType.Lunch:
						return new SolidColorBrush(Color.FromArgb(255, 75, 52, 96));
					case MealType.Dinner:
						return new SolidColorBrush(Color.FromArgb(255, 66, 122, 85));
					case MealType.Snack:
						return new SolidColorBrush(Color.FromArgb(255, 148, 249, 183));
					case MealType.Undefined:
					default:
						return new SolidColorBrush(Color.FromArgb(255, 186, 167, 138));
				}
			}
			else
			{
				return new SolidColorBrush(Color.FromArgb(255, 186, 167, 138));
			}
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotSupportedException();
		}
	}
}
