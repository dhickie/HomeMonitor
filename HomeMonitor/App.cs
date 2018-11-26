using HomeMonitor.Models;
using HomeMonitor.Options;
using HomeMonitor.Services;
using Microsoft.Extensions.Options;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace HomeMonitor
{
    public class App : IApp
    {
        private const string HOME_STATUS = "home";
        private const string HEATING_STATUS = "heating";

        private INestApiClient _client;
        private IStatsExporter _statsExporter;
        private readonly int _pollingPeriodSeconds;
        private DateTime _nextPoll;

        public App(INestApiClient client, IStatsExporter statsExporter, IOptions<NestOptions> options)
        {
            _client = client;
            _statsExporter = statsExporter;

            _pollingPeriodSeconds = options.Value.PollingPeriodSeconds;
        }

        public async Task Run()
        {
            // Starts the stats server
            _statsExporter.Start();

            _nextPoll = DateTime.UtcNow;
            NestResponse nestState = null;
            for (; ;)
            {
                var error = false;
                try
                {
                    nestState = await _client.GetNestState();
                }
                catch (Exception e)
                {
                    error = true;
                    Console.WriteLine($"Uhoh, something went wrong: {e}");
                }

                if (!error)
                {
                    ReportStats(nestState);
                }
                
                await WaitToNextPoll();
            }
        }

        private void ReportStats(NestResponse nestResponse)
        {
            var thermostat = nestResponse.Devices.Thermostats.First().Value;
            var awayString = nestResponse.Structures.First().Value.Away;

            var stats = new HomeStats();
            stats.AmbientTemperature = thermostat.AmbientTemperatureC;
            stats.TargetTemperature = thermostat.TargetTemperatureC;
            stats.Humidity = thermostat.Humidity;
            stats.Home = awayString == HOME_STATUS;
            stats.Heating = thermostat.HvacState == HEATING_STATUS;

            _statsExporter.PublishStats(stats);
        }

        private async Task WaitToNextPoll()
        {
            _nextPoll = _nextPoll.AddSeconds(_pollingPeriodSeconds);
            while (_nextPoll < DateTime.UtcNow)
            {
                _nextPoll = _nextPoll.AddSeconds(_pollingPeriodSeconds);
            }

            var waitTime = (_nextPoll - DateTime.UtcNow).TotalMilliseconds;
            await Task.Delay((int)waitTime);
        }
    }
}
