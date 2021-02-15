using System;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ServiceMonitor.Shared;
using WebPush;

namespace ServiceMonitor.Server.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class NotificationsController : ControllerBase
    {
        private readonly ServiceMonitorContext _db;

        public NotificationsController(ServiceMonitorContext db)
        {
            _db = db;
        }

        [HttpPut("subscription")]
        public async Task<NotificationSubscription> Subscribe(NotificationSubscription subscription)
        {
            subscription.Group = "NotificationTest";
            _db.Add(subscription);
            await _db.SaveChangesAsync();
            return subscription;
        }

        [HttpPost("send")]
        public async Task<IActionResult> SendNotificationAsync()
        {
            var subscriptions = _db.NotificationSubscriptions
                .OrderByDescending(n => n.NotificationSubscriptionId);

            foreach (var subscription in subscriptions)
            {
                var publicKey = "BLC8GOevpcpjQiLkO7JmVClQjycvTCYWm6Cq_a7wJZlstGTVZvwGFFHMYfXt6Njyvgx_GlXJeo5cSiZ1y4JOx1o";
                var privateKey = "OrubzSz3yWACscZXjFQrrtDwCKg-TGFuWhluQ2wLXDo";

                var pushSubscription = new PushSubscription(subscription.Url, subscription.P256dh, subscription.Auth);
                var vapidDetails = new VapidDetails("mailto:hello@niteshrestha.com.np", publicKey, privateKey);
                var webPushClient = new WebPushClient();
                try
                {
                    string message = "Scrapper has stopped working. Please check the server";
                    var payload = JsonSerializer.Serialize(new
                    {
                        message,
                        url = "",
                    });
                    await webPushClient.SendNotificationAsync(pushSubscription, payload, vapidDetails);
                    Console.WriteLine($"Notification sent to subscription with Id {subscription.NotificationSubscriptionId}");
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine($"Error sending push notification to {subscription.NotificationSubscriptionId}: {ex.Message}\nRemoving subscription from database.");
                    _db.Remove(subscription);
                }
            }
            await _db.SaveChangesAsync();

            return Ok();
        }

    }
}
