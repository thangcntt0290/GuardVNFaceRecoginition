using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace GuardVNFaceRecoginition.Converters
{
    /// <summary>
    /// Converts boolean to foreground color for navigation buttons
    /// </summary>
    public class BooleanToForegroundConverter : IValueConverter
    {
        public Brush SelectedBrush { get; set; } = Brushes.White;
        public Brush UnselectedBrush { get; set; } = new SolidColorBrush(Color.FromRgb(0x66, 0x66, 0x66));

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool boolValue && boolValue)
            {
                return SelectedBrush;
            }
            return UnselectedBrush;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}