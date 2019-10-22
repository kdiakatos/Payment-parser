using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaymentParser.Models
{
    public class Payment
    {
        public int CaseId { get; set; }
        public DateTime Timestamp { get; set; }
        public long Account { get; set; }
        public string Description { get; set; }
        public DateTime PayDate { get; set; }
        public decimal PayAmount { get; set; }
        public decimal Balance { get; set; }
        public string Currency { get; set; }
        public decimal BalanceCurrency { get; set; }
        public decimal PayAmountCurrency { get; set; }
    }
}
