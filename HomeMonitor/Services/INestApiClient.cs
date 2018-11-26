using HomeMonitor.Models;
using System.Threading.Tasks;

namespace HomeMonitor.Services
{
    public interface INestApiClient
    {
        Task<NestResponse> GetNestState();
    }
}
