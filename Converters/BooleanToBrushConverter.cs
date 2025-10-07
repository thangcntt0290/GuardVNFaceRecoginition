using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace GuardVNFaceRecoginition.Converters
{
    /// <summary>
    /// Converts boolean to different brush colors based on selection state
    /// </summary>
    public class BooleanToBrushConverter : IValueConverter
    {
        public Brush TrueBrush { get; set; } = new SolidColorBrush(Color.FromRgb(0x42, 0x85, 0xF4));
        public Brush FalseBrush { get; set; } = Brushes.Transparent;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool boolValue && boolValue)
            {
                return TrueBrush;
            }
            return FalseBrush;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}