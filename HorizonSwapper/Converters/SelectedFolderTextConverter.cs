using System.Globalization;
using System.Windows.Data;

namespace HorizonSwapper.Converters;

public class SelectedFolderTextConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        var path = value as string;
        return string.IsNullOrEmpty(path) ? "Selected game directory: None" : $"Selected game directory: {path}";
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
