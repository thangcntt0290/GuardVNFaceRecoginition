using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace GuardVNFaceRecoginition.Converters
{
    /// <summary>
    /// Converts boolean to Visibility
    /// </summary>
    public class BooleanToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool boolValue = false;
            
            // Handle different input types
            if (value is bool b)
            {
                boolValue = b;
            }
            else if (value is string s)
            {
                boolValue = !string.IsNullOrEmpty(s);
            }
            else if (value != null)
            {
                boolValue = true;
            }

            bool invert = parameter?.ToString()?.ToLower() == "invert";
            return (boolValue ^ invert) ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Visibility visibility)
            {
                bool invert = parameter?.ToString()?.ToLower() == "invert";
                return (visibility == Visibility.Visible) ^ invert;
            }
            return false;
        }
    }
}