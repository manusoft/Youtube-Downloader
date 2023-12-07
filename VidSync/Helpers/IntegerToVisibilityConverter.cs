namespace VidSync.Helpers;

public class IntegerToVisibilityConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (value is int intValue)
        {
            // Convert 0 to false, and any other integer to true
            return intValue == 0 ? Visibility.Visible : Visibility.Collapsed;
        }

        // Return false for non-integer values
        return Visibility.Collapsed;
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        throw new NotImplementedException();
    }
}
