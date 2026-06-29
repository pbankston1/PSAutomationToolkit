using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using PSAutomationToolkit.Models;

namespace PSAutomationToolkit.ViewModels
{
    // ── Complexity → Badge Color ─────────────────────────────────────
    public class ComplexityColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is ComplexityLevel level)
            {
                return level switch
                {
                    ComplexityLevel.Entry  => new SolidColorBrush(Color.FromRgb(46,  125, 50)),   // green
                    ComplexityLevel.Mid    => new SolidColorBrush(Color.FromRgb(230, 81,  0)),    // orange
                    ComplexityLevel.Senior => new SolidColorBrush(Color.FromRgb(106, 27, 154)),   // purple
                    _                     => new SolidColorBrush(Colors.Gray)
                };
            }
            return new SolidColorBrush(Colors.Gray);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }

    // ── bool → Visibility ───────────────────────────────────────────
    public class BoolToVisibilityConverter : IValueConverter
    {
        public static readonly BoolToVisibilityConverter Instance = new();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            => value is true ? Visibility.Visible : Visibility.Collapsed;

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => value is Visibility.Visible;
    }

    // ── bool → Visibility (inverted) ────────────────────────────────
    public class BoolToVisibilityInverseConverter : IValueConverter
    {
        public static readonly BoolToVisibilityInverseConverter Instance = new();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            => value is true ? Visibility.Collapsed : Visibility.Visible;

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => value is not Visibility.Visible;
    }

    // ── null → Visibility ────────────────────────────────────────────
    public class NullToVisibilityConverter : IValueConverter
    {
        public static readonly NullToVisibilityConverter NullVisible    = new() { ShowWhenNull = true };
        public static readonly NullToVisibilityConverter NotNullVisible = new() { ShowWhenNull = false };

        public bool ShowWhenNull { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool isNull = value == null;
            return (ShowWhenNull ? isNull : !isNull) ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }

    // ── non-empty string → Visibility ───────────────────────────────
    public class StringToVisibilityConverter : IValueConverter
    {
        public static readonly StringToVisibilityConverter Instance = new();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            => string.IsNullOrWhiteSpace(value as string) ? Visibility.Collapsed : Visibility.Visible;

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}
