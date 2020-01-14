using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace ITBox.Converters
{
    public class MemoryToWidthConverter : IMultiValueConverter
    {

        public object Convert(object[] value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value[0] is double && value[1] is double && value[2] is ListBox)
            {
                var decimalPercentage = ConvertToPercentage((double)value[0], (double)value[1]);
                var containerWidth = ((ListBox)value[2]).ActualWidth;

                return decimalPercentage * containerWidth;
            }

            //values are not correct types, return a width of 0
            else
                return (double)0;
        }

        public object[] ConvertBack(object value, Type[] targetType, object parameter, CultureInfo culture)
        {
            throw new Exception("not defined");
        }

        private double ConvertToPercentage(double valueToConvert, double total)
        {
            if (valueToConvert != 0 && total != 0)
                return (valueToConvert / total);

            //values are zero, return 0 percent
            else
                return (double)0;
        }
    }
}
