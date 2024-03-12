using StaticFieldEpidEval.Models;
using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace StaticFieldEpidEval.Views.Converters
{
    internal class CheckResultToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            CheckResult checkResult = (CheckResult)Enum.Parse(typeof(CheckResult), value.ToString());

            switch (checkResult)
            {
                case CheckResult.Information:
                    return Brushes.Blue;
                case CheckResult.Warning:
                    return Brushes.Orange;
                case CheckResult.Error:
                    return Brushes.Red;
                default:
                    return Brushes.Transparent;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
