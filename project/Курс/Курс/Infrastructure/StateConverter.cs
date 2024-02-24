using System;
using System.Linq;
using System.Windows.Data;
using Курс.Models;

namespace Курс.Infrastructure
{
    [ValueConversion(typeof(int), typeof(string))]
    public class StateConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            using (ComputerRepairShopContext db = new ComputerRepairShopContext())
            {
                return db.States.Where(s => s.StatusId == (int)value).FirstOrDefault().StatusName;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return null;
        }
    }
}
