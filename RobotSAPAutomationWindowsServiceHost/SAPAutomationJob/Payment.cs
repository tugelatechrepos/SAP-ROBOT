using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAPAutomationJob
{
    public class Payment
    {
        public string AccountNumber { get; set; }
        public string ServiceId { get; set; }
        public DateTime PaymentDate { get; set; }
        public string PaymentType { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; }
        public string Description { get; set; }
    }
}
