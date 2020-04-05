using System;
using System.Collections.Generic;
using System.Text;

namespace MCCC.Sensors.Data.Models
{
    public interface ISensor
    {
        string Id { get; set; }
        string Name { get; set; }
        DateTime DateEntered { get; set; }
    }

    public class HumiditySensor : ISensor
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public double Humidity { get; set; }
        public DateTime DateEntered { get { return DateTime.UtcNow; } set { } }

        public HumiditySensor(string id, string name, double humidity)
        {
            Id = id;
            Name = name;
            Humidity = humidity;
        }
    }

    public class TemperatureSensor : ISensor
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public double Celsius { get; set; }
        public double Fahrenheit { get; set; }
        public DateTime DateEntered { get { return DateTime.UtcNow; } set { } }

        public TemperatureSensor(string id, string name, double c, double f)
        {
            Id = id;
            Name = name;
            Celsius = c;
            Fahrenheit = f;
        }
    }

}
