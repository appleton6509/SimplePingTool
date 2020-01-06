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
    /// A converter class for converting byte values into KB,MB or GB
    /// </summary>
    public class MemorySizeByteConverter : IValueConverter
    {
        /// <summary>
        /// Convert a BYTE value into KB,MB, or GB
        /// </summary>
        /// <param name="value">a double byte value</param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        /// <remarks>
        /// 
        /// Conversion table from Bytes to KiloBytes,MegaBytes,GigaBytes.
        ///     1024 BYTES        == 1 KB
        ///     1024 KILOBYTES    == 1 MB
        ///     1024 MEGABYTES    == 1 GB
        /// 
        /// </remarks>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double convertedValue;
            double inputValue = (double)value;
            string label;


            // Less than 1 Megabyte, show Kilobyte 
            if (inputValue < Math.Pow(10, 6)) 
            {
                convertedValue = Math.Round(inputValue / 1024);
                label = " KB's";

            }
            // Less than 1 Gigabyte, show Megabyte 
            else if (inputValue < Math.Pow(10, 9))
            {
                convertedValue = Math.Round((inputValue / 1024) / 1024);
                label = " MB's";
            }

            //All larger values show in Gigabyte
            else
            {
                convertedValue = Math.Round(((inputValue / 1024) / 1024) / 1024, 2);
                label = " GB's";
            }



            return convertedValue + label;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
