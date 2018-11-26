using Newtonsoft.Json;
using System.Collections.Generic;

namespace HomeMonitor.Models
{
    public class NestResponse
    {
        [JsonProperty(PropertyName = "devices")]
        public Devices Devices { get; set; }
        [JsonProperty(PropertyName = "structures")]
        public Dictionary<string, Structure> Structures { get; set; }
}

    public class Devices
    {
        [JsonProperty(PropertyName = "thermostats")]
        public Dictionary<string, Thermostat> Thermostats { get; set; }
    }

    public class Thermostat
    {
        [JsonProperty(PropertyName = "humidity")]
        public int Humidity { get; set; }
        [JsonProperty(PropertyName = "target_temperature_c")]
        public double TargetTemperatureC { get; set; }
        [JsonProperty(PropertyName = "ambient_temperature_c")]
        public double AmbientTemperatureC { get; set; }
        [JsonProperty(PropertyName = "hvac_state")]
        public string HvacState { get; set; }
    }

    public class Structure
    {
        [JsonProperty(PropertyName = "away")]
        public string Away { get; set; }
    }
}
