using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Net;
using System.IO;
using System.Xml;
using WeatherForecast.Models.SupportModel;
using WeatherForecast.Models;
using System.Globalization;


namespace WeatherForecast.Controllers
{
    public class HomeController : Controller
    {
        public void SetCurrentWeather(ForecastDataModel dataModel, string nameOfCity)
        {
            // Url of request on OpenWeatherMap API.
            string url = String.Format("http://api.openweathermap.org/data/2.5/weather?q={0}&mode=xml&units=metric&APPID=7d84c8e4c9f0d51eb938228e50a49b4a", nameOfCity);
            HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create(url);
            HttpWebResponse response = (HttpWebResponse)req.GetResponse();


            Stream stream = response.GetResponseStream(); // The stream of data from the Internet resource.
            XmlDocument doc = new XmlDocument();


            doc.Load(stream); // Loads the XML document from the specified stream.

            // Work with xml document.
            dataModel.CurrentWeather.Sunrise = doc.SelectSingleNode("current/city/sun").Attributes["rise"].Value.Substring(11);  // извлечения атрибута по имени
            dataModel.CurrentWeather.Sunset = doc.SelectSingleNode("current/city/sun").Attributes["set"].InnerText.Substring(11);

            dataModel.CurrentWeather.Temperature = Double.Parse(doc.SelectSingleNode("current/temperature").Attributes["value"].Value.Replace('.', ','));
            dataModel.CurrentWeather.Humidity = Int32.Parse(doc.SelectSingleNode("current/humidity").Attributes["value"].Value);
            dataModel.CurrentWeather.Pressure = Double.Parse(doc.SelectSingleNode("current/pressure").Attributes["value"].Value.Replace('.', ','));
            dataModel.CurrentWeather.SpeedWind = Double.Parse(doc.SelectSingleNode("current/wind/speed").Attributes["value"].Value.Replace('.', ','));
            dataModel.CurrentWeather.Cloudiness = Int32.Parse(doc.SelectSingleNode("current/clouds").Attributes["value"].Value);
            if (doc.SelectSingleNode("current/precipitation").Attributes["mode"].Value != "no")
            {
                dataModel.CurrentWeather.Precipitation = Double.Parse(doc.SelectSingleNode("current/precipitation").Attributes["value"].Value.Replace('.', ','));
            }
            else
            {
                dataModel.CurrentWeather.Precipitation = 0;
            }

            dataModel.CurrentWeather.Visibility = Int32.Parse(doc.SelectSingleNode("current/visibility").Attributes["value"].Value);
            dataModel.CurrentWeather.UrlIcon = "http://openweathermap.org/img/w/" + doc.SelectSingleNode("current/weather").Attributes["icon"].Value + ".png";
            dataModel.CurrentWeather.StateWeather = doc.SelectSingleNode("current/weather").Attributes["value"].Value;
        }

        public void SetForecastWeather(ForecastDataModel dataModel, string nameOfCity)
        {
            // Url of request on OpenWeatherMap API.
            string url = String.Format("http://api.openweathermap.org/data/2.5/forecast?q={0},us&mode=xml&units=metric&APPID=7d84c8e4c9f0d51eb938228e50a49b4a", nameOfCity);

            HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create(url);
            HttpWebResponse response = (HttpWebResponse)req.GetResponse();

            
            Stream stream = response.GetResponseStream(); // The stream of data from the Internet resource. 
            XmlDocument doc = new XmlDocument();


            doc.Load(stream); // Loads the XML document from the specified stream.

            XmlNodeList timeList = doc.SelectNodes("weatherdata/forecast/time");
            WeatherForPeriodOfTime weatherForPeriodOfTime;
            WeatherForecast.Models.WeatherForecast weatherForecast = new WeatherForecast.Models.WeatherForecast();
            weatherForecast.WeatherDyringDay = new List<WeatherForPeriodOfTime>();

            // Set first period of time.
            weatherForPeriodOfTime = GetWeatherForPeriodOfTime(timeList[0]);
            weatherForecast.WeatherDyringDay.Add(weatherForPeriodOfTime);

            // Set another period of time (except first).
            string day;
            for (int i = 1; i < timeList.Count; i++)
            {
                day = timeList[i].Attributes["from"].Value.Substring(8, 2);
                if (day == weatherForPeriodOfTime.StartTime.Substring(8, 2))
                {
                    weatherForPeriodOfTime = GetWeatherForPeriodOfTime(timeList[i]);
                    weatherForecast.WeatherDyringDay.Add(weatherForPeriodOfTime);
                }
                else
                {
                    weatherForecast.Date = new DateTime(Int32.Parse(weatherForPeriodOfTime.StartTime.Substring(0, 4)),
                                                        Int32.Parse(weatherForPeriodOfTime.StartTime.Substring(5, 2)),
                                                        Int32.Parse(weatherForPeriodOfTime.StartTime.Substring(8, 2)));
                    dataModel.Forecast.Add(weatherForecast);

                    weatherForecast = new WeatherForecast.Models.WeatherForecast();
                    weatherForecast.WeatherDyringDay = new List<WeatherForPeriodOfTime>();

                    weatherForPeriodOfTime = GetWeatherForPeriodOfTime(timeList[i]);
                    weatherForecast.WeatherDyringDay.Add(weatherForPeriodOfTime);
                }
            }
        }

        // This method get weather forecast(three-hours period) from xml node
        public WeatherForPeriodOfTime GetWeatherForPeriodOfTime( XmlNode node)
        {
            WeatherForPeriodOfTime weatherForPeriodOfTime = new WeatherForPeriodOfTime();
            weatherForPeriodOfTime.StartTime = node.Attributes["from"].Value;
            weatherForPeriodOfTime.EndTime = node.Attributes["to"].Value;
            weatherForPeriodOfTime.SpeedWind = Double.Parse(node.SelectSingleNode("windSpeed").Attributes["mps"].Value.Replace('.', ','));
            weatherForPeriodOfTime.Temperature = Double.Parse(node.SelectSingleNode("temperature").Attributes["value"].Value.Replace('.', ','));
            weatherForPeriodOfTime.Pressure = Double.Parse(node.SelectSingleNode("pressure").Attributes["value"].Value.Replace('.', ','));
            weatherForPeriodOfTime.Humidity = Int32.Parse(node.SelectSingleNode("humidity").Attributes["value"].Value);
            weatherForPeriodOfTime.Cloudiness = Int32.Parse(node.SelectSingleNode("clouds").Attributes["all"].Value);
            weatherForPeriodOfTime.StateWeather = node.SelectSingleNode("symbol").Attributes["name"].Value;
            weatherForPeriodOfTime.UrlIcon = "http://openweathermap.org/img/w/" + node.SelectSingleNode("symbol").Attributes["var"].Value + ".png";
            if (node.SelectSingleNode("precipitation").Attributes["value"] != null)
            {
                weatherForPeriodOfTime.Precipitation = Double.Parse(node.SelectSingleNode("precipitation").Attributes["value"].Value.Replace('.', ','));
            }
            else
            {
                weatherForPeriodOfTime.Precipitation = 0;
            }

            return weatherForPeriodOfTime;
        }
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult GetCurrentWeather(string nameOfCity, string nameOfCountry)
        {

            ViewData["CityName"] = nameOfCity; //Set name city.
            ForecastDataModel dataModel = new ForecastDataModel();
            dataModel.CurrentWeather = new Weather();
            dataModel.Forecast = new List<WeatherForecast.Models.WeatherForecast>();

            try
            {
                SetCurrentWeather(dataModel, nameOfCity);
                SetForecastWeather(dataModel, nameOfCity);
            }
            catch (WebException)
            {
                ViewData["TextError"] = "There is no such city";  // Set error message.
                return View("Index");
            }
            catch(Exception e)
            {
                ViewData["TextError"] = e.Message; // Set error message.
                return View("Index");
            }

            

            return View("Index", dataModel);
        }

    }
}