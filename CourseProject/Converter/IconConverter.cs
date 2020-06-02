using System;
using System.IO;
using System.Globalization;
using System.Windows.Data;

namespace CourseProject.Converter
{
    class IconConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string currentDirectory = Directory.GetCurrentDirectory() + "\\Images\\";
            return currentDirectory + parameter.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }
}
