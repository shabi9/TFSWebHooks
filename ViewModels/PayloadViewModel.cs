using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebHooksDevOps.ViewModels
{
    public class PayloadViewModel
    {
        public int id { get; set; }
        public string eventType { get; set; }
        public string SubscriptionId { get; set; }
        public string createdBy { get; set; }
        public int rev { get; set; }
        public string teamProject { get; set; }
        public string url { get; set; }
        public string assignedTo { get; set; }
        public string pat { get; set; }
    }

    public class PayloadViewModel1
    {
        
        public string SubscriptionId { get; set; }
    }
}
