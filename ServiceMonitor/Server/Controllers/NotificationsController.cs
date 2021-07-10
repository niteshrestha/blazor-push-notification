using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ServiceMonitor.Server.Helpers;
using ServiceMonitor.Shared;

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

            var notificationHelper = new NotificationHelper();
            foreach (var subscription in subscriptions)
            {
                try
                {
                    await notificationHelper.SendNotification(subscription);
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
