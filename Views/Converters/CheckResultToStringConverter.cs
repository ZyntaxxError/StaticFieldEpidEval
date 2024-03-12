using StaticFieldEpidEval.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace StaticFieldEpidEval.Views.Converters
{
    internal class CheckResultToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            CheckResult checkResult = (CheckResult)Enum.Parse(typeof(CheckResult), value.ToString());

            string returnValue = string.Empty;
            switch (checkResult)
            {
                case CheckResult.Error:
                    {
                        returnValue = "r";
                        break;
                    }
                case CheckResult.Information:
                    {
                        returnValue = "i";
                        break;
                    }
                case CheckResult.Warning:
                    {
                        returnValue = "L";
                        break;
                    }
            }

            return returnValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
