using System;
using System.Text.Json;
using System.Threading.Tasks;
using ServiceMonitor.Shared;
using WebPush;

namespace ServiceMonitor.Server.Helpers
{
    public class NotificationHelper
    {
        public async Task SendNotification(NotificationSubscription subscription)
        {
            var publicKey = "BLC8GOevpcpjQiLkO7JmVClQjycvTCYWm6Cq_a7wJZlstGTVZvwGFFHMYfXt6Njyvgx_GlXJeo5cSiZ1y4JOx1o";
            var privateKey = "OrubzSz3yWACscZXjFQrrtDwCKg-TGFuWhluQ2wLXDo";

            var pushSubscription = new PushSubscription(subscription.Url, subscription.P256dh, subscription.Auth);
            var vapidDetails = new VapidDetails("mailto:hello@niteshrestha.com.np", publicKey, privateKey);
            var webPushClient = new WebPushClient();
            string message = "Scrapper has stopped working. Please check the server";
            var payload = JsonSerializer.Serialize(new
            {
                message,
                url = "counter",
            });
            await webPushClient.SendNotificationAsync(pushSubscription, payload, vapidDetails);
            Console.WriteLine($"Notification sent to subscription with Id {subscription.NotificationSubscriptionId}");
        }
    }
}
