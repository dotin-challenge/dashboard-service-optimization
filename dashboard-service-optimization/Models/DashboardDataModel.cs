using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dashboard_service_optimization.Models
{
    public class DashboardDataModel
    {
        public byte Month { get; set; }
        public double Value { get; set; }
        public string MonthTitle => Month switch
        {
            1 => "January",
            2 => "February",
            3 => "March",
            4 => "April",
            5 => "May",
            6 => "June",
            7 => "July",
            8 => "August",
            9 => "September",
            10 => "October",
            11 => "November",
            12 => "December",
            _ => string.Empty
        };

    }
}
