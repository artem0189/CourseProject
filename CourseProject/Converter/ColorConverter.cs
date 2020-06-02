using System;
using System.Globalization;
using System.Windows.Data;

namespace CourseProject.Converter
{
    class ColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
            {
                string result = "";
                if (value.Equals(true))
                {
                    result = "Green";
                }
                else
                {
                    result = "Red";
                }
                return result;
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }
}
