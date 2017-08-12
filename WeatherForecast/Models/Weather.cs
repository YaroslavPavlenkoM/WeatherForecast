using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WeatherForecast.Models
{
    public class Weather
    {
        public string Sunset { get; set; }
        public string Sunrise { get; set; }
        public double Temperature { get; set; }
        public int Humidity { get; set; }
        public double Pressure { get; set; }
        public double SpeedWind { get; set; }
        public int Visibility { get; set; }
        public int Cloudiness { get; set; }
        public double Precipitation { get; set; }
        public string UrlIcon { get; set; }
        public string StateWeather { get; set; }
    }

    public class WeatherForPeriodOfTime : Weather
    {
        public string StartTime { get; set; }
        public string EndTime { get; set; }
    }
}