using Avalonia.Data.Converters;
using AvaloniaApplication1.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AvaloniaApplication1.Helper
{
    public class IndexToItemConverter: IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int index && parameter is IEnumerable<FileTree> children)
            {
                var item = children.ElementAtOrDefault(index);
                return item;
            }

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
