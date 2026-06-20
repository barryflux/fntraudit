using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace FntrAudit.Helpers
{
    public class BoolToGridLengthConverter : IValueConverter
    {
        public double ExpandedWidth { get; set; } = 260;
        public double CollapsedWidth { get; set; } = 70;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var isCollapsed = value is bool b && b;
            return new GridLength(isCollapsed ? CollapsedWidth : ExpandedWidth);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is GridLength gridLength)
                return gridLength.Value <= CollapsedWidth;

            return false;
        }
    }
}
