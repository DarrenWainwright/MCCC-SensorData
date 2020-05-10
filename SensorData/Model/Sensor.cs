using System;

namespace SensorData.Model
{
    public class Sensor
    {
        public Sensor(string id, string name, string type, int hearbeatInterval, DateTime lastHeartbeat)
        {
            Id = id;
            Name = name;
            Type = type;
            HeartbeatInterval = hearbeatInterval;
            LastHeartbeat = lastHeartbeat;
        }

        public string Id { get; }
        public string Name { get; }
        public string Type { get; }
        public int HeartbeatInterval { get; }
        public DateTime LastHeartbeat { get; }
    }
}