using HomeMonitor.Models;
using Prometheus;
using System;

namespace HomeMonitor.Services
{
    public class PrometheusExporter : IStatsExporter
    {
        private readonly Gauge _ambientTemperatureGauge;
        private readonly Gauge _targetTemperatureGauge;
        private readonly Gauge _humidityGauge;
        private readonly Gauge _homeGauge;
        private readonly Gauge _heatingGuage;

        private readonly MetricServer _server;

        private bool _isStarted;

        public PrometheusExporter()
        {
            _ambientTemperatureGauge = Metrics.CreateGauge("AmbientTemperature", "Reports the current ambient temperature of the house");
            _targetTemperatureGauge = Metrics.CreateGauge("TargetTemperature", "Reports the current target temperature of the house");
            _humidityGauge = Metrics.CreateGauge("Humidity", "Reports the current humidity of the house");
            _homeGauge = Metrics.CreateGauge("Home", "Reports whether the thermostat is set to home mode");
            _heatingGuage = Metrics.CreateGauge("Heating", "Reports whether the heating system is currently on");

            _server = new MetricServer(1234);
        }

        public void Start()
        {
            _server.Start();
            _isStarted = true;
        }

        public void PublishStats(HomeStats metrics)
        {
            if (!_isStarted)
            {
                throw new ArgumentException("Can't report metrics if the server isn't started!");
            }

            _ambientTemperatureGauge.Set(metrics.AmbientTemperature);
            _targetTemperatureGauge.Set(metrics.TargetTemperature);
            _humidityGauge.Set(metrics.Humidity);
            _homeGauge.Set(metrics.Home ? 1 : 0);
            _heatingGuage.Set(metrics.Heating ? 1 : 0);
        }
    }
}
