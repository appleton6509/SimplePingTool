using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace ITBox.Converters
{
    /// <summary>
    /// Converts a bool value with string parameter containing a int value, to the supplied int value.
    /// </summary>
    /// <remarks>
    /// intended to be used with a OneWayToSource binding from a bool event to a view model property
    /// </remarks>
    public class BoolToIntValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new Exception();
        }

        /// <summary>
        /// Converts from bool with int parameter to int value
        /// </summary>
        /// <param name="value">true/false</param>
        /// <param name="targetType">int</param>
        /// <param name="parameter">int value</param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((bool)value)
            {
                int.TryParse(parameter.ToString(), out int newValue);
                return newValue;
            }
            else { return 0; }
            
        }
    }
}
