using System;
using Newtonsoft.Json;

namespace SensorData.Model
{

    public interface IDHT22 { }

    public class TemperatureSensorEvent : IDHT22
    {
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("sensorId")]
        public string SensorId { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("celcius")]
        public double Celcius { get; set; }
        [JsonProperty("fahrenheit")]
        public double Fahrenheit { get; set; }
        [JsonProperty("dateCreated")]
        public DateTime DateCreated { get; set; }

        public TemperatureSensorEvent(string sensorId, string name, double celcius, double fahrenheit)
        {
            Id = Guid.NewGuid().ToString();
            SensorId = sensorId;
            Name = name;
            Celcius = celcius;
            Fahrenheit = fahrenheit;
            DateCreated = DateTime.UtcNow;
        }

    }

    public class HumiditySensorEvent : IDHT22
    {
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("sensorId")]
        public string SensorId { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("humidity")]
        public double Humidity { get; set; }
        [JsonProperty("dateCreated")]
        public DateTime DateCreated { get; set; }

        public HumiditySensorEvent(string sensorId, string name, double humidity)
        {
            Id = Guid.NewGuid().ToString();
            SensorId = sensorId;
            Name = name;
            Humidity = humidity;
            DateCreated = DateTime.UtcNow;
        }

    }
}
