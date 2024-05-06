using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace swas.DAL.Models
{
    public class Notification
    {
        public int NotificationId { get; set; }
        public int ProjId { get; set; }
        public int NotificationFrom { get; set; }
        public int NotificationTo { get; set; }
        public bool IsRead { get; set; }
        public DateTime ReadDateTime { get; set; }

    }
}
