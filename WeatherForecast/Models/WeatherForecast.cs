using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Globalization;

namespace WeatherForecast.Models
{
    public class WeatherForecast
    {
        public DateTime Date { get; set; }
        public List<WeatherForPeriodOfTime> WeatherDyringDay { get; set; }

        public string ToMonthName()
        {
            return CultureInfo.CreateSpecificCulture("en-US").DateTimeFormat.GetMonthName(this.Date.Month);
        }
    }
}