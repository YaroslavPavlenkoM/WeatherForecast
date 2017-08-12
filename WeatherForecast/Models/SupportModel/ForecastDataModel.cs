using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WeatherForecast.Models.SupportModel
{
    public class ForecastDataModel
    {
        public Weather CurrentWeather { get; set; }
        public List<WeatherForecast> Forecast { get; set; }
    }
}