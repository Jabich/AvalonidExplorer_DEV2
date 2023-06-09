﻿using Avalonia.Data.Converters;
using Avalonia.Media.Imaging;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace AvaloniaApplication1.Helper
{
    public class IconConverter : IMultiValueConverter
    {
        private readonly Bitmap _file;
        private readonly Bitmap _folderExpanded;
        private readonly Bitmap _folderCollapsed;

        public IconConverter(Bitmap file, Bitmap folderExpanded, Bitmap folderCollapsed)
        {
            _file = file;
            _folderExpanded = folderExpanded;
            _folderCollapsed = folderCollapsed;
        }

        public object? Convert(IList<object?> values, Type targetType, object? parameter, CultureInfo culture)
        {
            return values.Count == 2 && values[0] is bool isDirectory && values[1] is bool isExpanded
                    ? !isDirectory ? _file : isExpanded ? _folderExpanded : _folderCollapsed
                    : null;
        }
    }
}