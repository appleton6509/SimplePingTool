using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace SimplePingTool.Converters
{
    /// <summary>
    /// Converts a bool value with string parameter containing a int value, to the supplied int value.
    /// </summary>
    /// <remarks>
    /// intended to be used with a OneWayToSource binding from a bool event to a view model property
    /// </remarks>
    public class TextToIntConverter : IValueConverter
    {
        /// <summary>
        /// converts from a int to a string
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!Object.Equals(value,null))
            {
                return value.ToString();
            }
            else return null;
        }

        /// <summary>
        /// Converts from string to an int
        /// </summary>
        /// <param name="value">true/false</param>
        /// <param name="targetType">int</param>
        /// <param name="parameter">int value</param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (int.TryParse(value.ToString(), out int interval))
            {
                return interval;
            }
            else return 0;
        }
    }
}
