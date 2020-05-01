using System;
using Newtonsoft.Json;

namespace SensorData.Model
{
    public class Heartbeat
    {
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("type")]
        public string Type { get; set; }
        [JsonProperty("heartbeatInterval")]
        public int HeartbeatInterval { get; set; }
        [JsonProperty("lastConnected")]
        public DateTime LastConnected { get; set; }

        public Heartbeat(string name, string type, int heartbeatInterval)
        {
            Id = Guid.NewGuid().ToString();
            Name = name;
            Type = type;
            HeartbeatInterval = heartbeatInterval;
            LastConnected = DateTime.UtcNow;
        }
    }
}
