using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using ServiceMonitor.Shared;

namespace ServiceMonitor.Client
{
    public class MonitoringClient
    {
        private readonly HttpClient _httpClient;

        public MonitoringClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task SubscribeToNotification(NotificationSubscription subscription)
        {
            var response = await _httpClient.PutAsJsonAsync("notifications/subscription", subscription);
            response.EnsureSuccessStatusCode();
        }
    }
}
