using System;
using Newtonsoft.Json;

namespace SensorData.Model
{
    public class WeatherDocument
    {
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("location")]
        public string Location { get; set; }
        [JsonProperty("celcius")]
        public double Celcius { get; set; }
        [JsonProperty("fahrenheit")]
        public double Fahrenheit { get; set; }
        [JsonProperty("humidity")]
        public double Humidity { get; set; }
        [JsonProperty("dateCreated")]
        public DateTime DateCreated { get; set; }

        public WeatherDocument(string location, double celcius, double humidity)
        {
            Id = Guid.NewGuid().ToString();
            Location = location;
            Celcius = celcius;
            Fahrenheit = ((9.0 / 5.0) * Celcius) + 32;
            Humidity = humidity;
            DateCreated = DateTime.UtcNow;
        }



    }



}
