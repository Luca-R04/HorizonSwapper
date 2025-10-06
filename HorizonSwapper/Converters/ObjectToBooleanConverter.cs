using System.Globalization;
using System.Windows.Data;

namespace HorizonSwapper.Converters;

public class ObjectToBooleanConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        // value = SelectedCharacter
        // parameter = current RadioButton's DataContext (current Character)
        return value == parameter;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return (bool)value ? parameter : Binding.DoNothing;
    }
}
