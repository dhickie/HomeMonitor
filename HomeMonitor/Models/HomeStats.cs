namespace HomeMonitor.Models
{
    public class HomeStats
    {
        public double TargetTemperature { get; set; }
        public double AmbientTemperature { get; set; }
        public int Humidity { get; set; }
        public bool Home { get; set; }
        public bool Heating { get; set; }
    }
}
