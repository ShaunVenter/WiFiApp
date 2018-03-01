using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlwaysOn.Objects
{
    public class UserSettings
    {
        public bool Notifications { get; set; } = true;
        public bool NotificationText { get; set; } = true;
        public bool NotificationSound { get; set; } = true;
        public bool NotificationAvailable { get; set; } = true;
        public bool DisableConnectionManager { get; set; } = true;
        public DateTime LastConnectionNotification { get; set; } = DateTime.Now.AddMinutes(-60);
        public DateTime LastAvailableNotification { get; set; } = DateTime.Now.AddMinutes(-60);
    }
}
