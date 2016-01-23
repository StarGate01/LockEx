using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Net.Http;

namespace LockEx.OWM
{

#region Data contracts

    [DataContract]
    public class MultipleDaysForecast
    {

        [DataMember(Name = "cod")]
        public int Code { get; set; }

        [DataMember(Name = "message")]
        public string Message { get; set; }

        [DataMember(Name = "list")]
        public List<Forecast> Forecasts { get; set; }

    }

    [DataContract]
    public class Forecast
    {

        [DataMember(Name = "dt")]
        public int Timestamp { get; set; }

        [DataMember(Name = "temp")]
        public Temperature Temperature { get; set; }

        [DataMember(Name = "pressure")]
        public double Pressure { get; set; }

        [DataMember(Name = "humidity")]
        public double Humidity { get; set; }

        [DataMember(Name = "weather")]
        public List<Weather> Weathers { get; set; }

        [DataMember(Name = "speed")]
        public double WindSpeed { get; set; }

        [DataMember(Name = "deg")]
        public double WindDirection { get; set; }

    }

    [DataContract]
    public class Temperature
    {

        [DataMember(Name = "day")]
        public double Day { get; set; }

        [DataMember(Name = "min")]
        public double Min { get; set; }

        [DataMember(Name = "max")]
        public double Max { get; set; }

        [DataMember(Name = "night")]
        public double Night { get; set; }

        [DataMember(Name = "eve")]
        public double Evening { get; set; }

        [DataMember(Name = "morn")]
        public double Morning { get; set; }

    }

    [DataContract]
    public class Weather
    {

        [DataMember(Name = "id")]
        public int Id { get; set; }

        [DataMember(Name = "main")]
        public string Main { get; set; }

        [DataMember(Name = "description")]
        public string Description { get; set; }

        [DataMember(Name = "icon")]
        public string Icon { get; set; }

    }

#endregion

    class OWMClient
    {

        private string ApiKey;

        public OWMClient(string apiKey)
        {
            ApiKey = apiKey;
        }

        public async Task<MultipleDaysForecast> GetDaysForcast(string city, string language, int numDays)
        {
            try
            {
                HttpClient client = new HttpClient();
                String resString = await client.GetStringAsync("http://api.openweathermap.org/data/2.5/forecast/daily?q=" + city + "&appid=" + ApiKey + "&lang=" + language + "&cnt=" + numDays + "&units=metric");
                DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(MultipleDaysForecast));
                using (var ms = new MemoryStream(Encoding.UTF8.GetBytes(resString)))
                {
                    MultipleDaysForecast forecast = (MultipleDaysForecast)serializer.ReadObject(ms);
                    if (forecast.Code != 200) return null;
                    return forecast;
                }
            }
            catch
            {
                return null;
            }

        }

    }

}
