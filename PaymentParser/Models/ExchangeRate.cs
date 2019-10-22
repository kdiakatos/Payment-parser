using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaymentParser.Models
{
    public class ExchangeRate
    {
        public Rates rates { get; set; }
        public string @base { get; set; }
        public string date { get; set; }
    }
    public class Rates
    {
        public decimal CAD { get; set; }
        public decimal HKD { get; set; }
        public decimal ISK { get; set; }
        public decimal PHP { get; set; }
        public decimal DKK { get; set; }
        public decimal HUF { get; set; }
        public decimal CZK { get; set; }
        public decimal AUD { get; set; }
        public decimal RON { get; set; }
        public decimal SEK { get; set; }
        public decimal IDR { get; set; }
        public decimal INR { get; set; }
        public decimal BRL { get; set; }
        public decimal RUB { get; set; }
        public decimal HRK { get; set; }
        public decimal JPY { get; set; }
        public decimal THB { get; set; }
        public decimal CHF { get; set; }
        public decimal SGD { get; set; }
        public decimal PLN { get; set; }
        public decimal BGN { get; set; }
        public decimal TRY { get; set; }
        public decimal CNY { get; set; }
        public decimal NOK { get; set; }
        public decimal NZD { get; set; }
        public decimal ZAR { get; set; }
        public decimal USD { get; set; }
        public decimal MXN { get; set; }
        public decimal ILS { get; set; }
        public decimal GBP { get; set; }
        public decimal KRW { get; set; }
        public decimal MYR { get; set; }
    }
}
