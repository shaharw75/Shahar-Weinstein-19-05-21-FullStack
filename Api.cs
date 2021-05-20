using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using mainModel;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RealCommerce.DTO;

namespace RealCommerce
{
    public static class Api
    {
        /// <summary>
        /// Get city name from autoComplete field, data is being fetched from Accuweather
        /// </summary>
        /// <param name="q"></param>
        /// <returns></returns>
        public static List<DTO.Search> GetQuery(string q)
        {
            var result = new ConcurrentBag<DTO.Search>();
            using (var client = new HttpClient())
            {
                var apiKey = Utils.GetConfigByName<string>("ApiKey");
                var jsonResult = client.GetStringAsync(new Uri(
                    $"http://dataservice.accuweather.com/locations/v1/cities/autocomplete?apikey={apiKey}&q={q}&language=he-IL")).Result;

                var citiesResult = (JArray)JsonConvert.DeserializeObject(jsonResult);

                if (citiesResult.Count > 0)
                {
                    Parallel.ForEach(citiesResult, city =>
                    {
                        result.Add(new Search()
                        {
                            Key = int.Parse(city["Key"].ToString()),
                            CityName = city["LocalizedName"].ToString()

                        });
                    });
                }
            }
            
            
            return result.ToList();
        }

        /// <summary>
        /// Get specific weather information from Accuweather
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static WeatherResult GetWeather(string key)
        {
            var result = new DTO.WeatherResult();
            
            using (var context = new DB.mainModel())
            {
                var city = context.Cities.FirstOrDefault(x => x.CityKey == key);
                if (city != null && city.WeatherDate == DateTime.Today.Date &&
                    city.WeatherTime >= DateTime.Now.TimeOfDay.Add(new TimeSpan(0, -1, 0, 0, 0)))
                {
                    result = new WeatherResult()
                    {
                        Weather = int.Parse(city.Weather.ToString()),
                        WeatherText = city.WeatherText
                        
                    };
                }
                else
                {
                    using (var client = new HttpClient())
                    {
                        var apiKey = Utils.GetConfigByName<string>("ApiKey");
                        
                        
                        
                        var jsonResult = client.GetStringAsync(new Uri(
                            $"http://dataservice.accuweather.com/currentconditions/v1/{key}?apikey={apiKey}&language=he-IL")).Result;

                        var weatherResult = (JArray)JsonConvert.DeserializeObject(jsonResult);

                        if (weatherResult.Count > 0)
                        {
                            var wText = weatherResult[0]["WeatherText"].ToString();
                            int wTemp = int.Parse(Math.Round(decimal.Parse(weatherResult[0]["Temperature"]["Metric"]["Value"]
                                .ToString())).ToString());

                            result.Weather = wTemp;
                            result.WeatherText = wText;

                            var city2 = context.Cities.FirstOrDefault(x => x.CityKey == key);
                            if (city2 == null)
                            {
                                var jsonResultCity = client.GetStringAsync(new Uri(
                                    $"http://dataservice.accuweather.com/locations/v1/{key}?apikey={apiKey}&language=he-IL")).Result;

                                var cityResult = (JObject)JsonConvert.DeserializeObject(jsonResultCity);
                                var cityName = cityResult["LocalizedName"].ToString();
                                
                                context.Cities.Add(new City()
                                {
                                    Weather = wTemp,
                                    CityKey = key,
                                    CityName = cityName,
                                    WeatherDate = DateTime.Now.Date,
                                    WeatherText = wText,
                                    WeatherTime = DateTime.Now.TimeOfDay
                                });
                                context.SaveChanges();
                            }
                            else
                            {
                                city2.Weather = wTemp;
                                city2.WeatherText = wText;
                                context.SaveChanges();
                            }

                        }
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Add city to favorites list
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static int Add(string key)
        {
            
            var result = 0;
            
            using (var context = new DB.mainModel())
            {
                
                var city = context.Cities.FirstOrDefault(x => x.CityKey == key);
                if (city != null)
                {
                    var fav = context.Favorites.FirstOrDefault(x => x.CityId == city.Id);
                    if (fav == null)
                    {
                        context.Favorites.Add(new Favorite()
                        {
                            CityId = city.Id
                        });
                        context.SaveChanges();
                    }
                    else
                    {
                        result = 1;
                    }
                }
                else
                {
                    result = 1;
                }
                
            }

            return result;
        }

        /// <summary>
        /// remove city from favorites list
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static int Delete(string key)
        {
            var result = 0;
            
            using (var context = new DB.mainModel())
            {
                
                var city = context.Cities.FirstOrDefault(x => x.CityKey == key);
                if (city != null)
                {
                    var fav = context.Favorites.FirstOrDefault(x => x.CityId == city.Id);
                    if (fav == null)
                    {
                        result = 1;

                    }
                    else
                    {
                        context.Favorites.Remove(fav);
                        context.SaveChanges();                        
                    }
                }
                else
                {
                    result = 1;
                }
                
            }

            return result;
        }
    }
}