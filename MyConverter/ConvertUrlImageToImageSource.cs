using CashY.Core;
using System.Globalization;

namespace CashY.MyConverter
{
    public class ConvertUrlImageToImageSource : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (string.IsNullOrEmpty(value?.ToString())) return null;

            string url = string.Format(FORMATS.DOWNLOAD_IMAGE, value);

            // Create an ImageSource from the URL
            Uri imageUrl = new Uri(url);
            ImageSource imageSource = ImageSource.FromUri(imageUrl);

            return imageSource;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
