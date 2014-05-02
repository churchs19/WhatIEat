using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace Shane.Church.WhatIEat.WP.Helpers
{
    public class PreviousSiblingConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is FrameworkElement)
            {
                var elem = value as FrameworkElement;
                if (elem.Parent != null)
                {
                    var childCount = VisualTreeHelper.GetChildrenCount(elem.Parent);
                    var itemIdx = -1;
                    for (var i = 0; i < childCount; i++)
                    {
                        var item = VisualTreeHelper.GetChild(elem.Parent, i);
                        if (item.Equals(elem))
                        {
                            itemIdx = i;
                        }
                    }
                    if (itemIdx > 0)
                    {
                        return VisualTreeHelper.GetChild(elem.Parent, itemIdx - 1);
                    }
                }
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
