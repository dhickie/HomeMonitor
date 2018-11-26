using HomeMonitor.Models;

namespace HomeMonitor.Services
{
    public interface IStatsExporter
    {
        void Start();
        void PublishStats(HomeStats stats);
    }
}
