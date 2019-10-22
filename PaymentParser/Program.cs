using Dapper;
using Microsoft.VisualBasic.FileIO;
using Newtonsoft.Json;
using PaymentParser.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace PaymentParser
{
    class Program
    {
        static void Main(string[] args)
        {
            var paymentsList = new List<Payment>();
            var path = Directory.GetCurrentDirectory() + "/Files/PF.csv";
            // TextFieldParser is in the Microsoft.VisualBasic.FileIO namespace.
            using (TextFieldParser parser = new TextFieldParser(path))
            {
                parser.CommentTokens = new string[] { "#" };
                parser.SetDelimiters(new string[] { ";" });
                parser.HasFieldsEnclosedInQuotes = true;

                // Skip over header line.
                parser.ReadLine();

                while (!parser.EndOfData)
                {
                    string[] fields = parser.ReadFields();
                    var accountNumber = fields[0];
                    var description = fields[1];
                    var payDate = fields[2];
                    var payAmount = fields[3];
                    var balance = fields[4];
                    var currency = fields[5];
                    var isParsedAccountNumber = long.TryParse(accountNumber, out long parsedAccountNumber);
                    var isParsedPayDate = DateTime.TryParse(payDate, out DateTime parsedPayDate);
                    var isParsedPayAmount = Decimal.TryParse(payAmount, out Decimal parsedPayAmount);
                    var isParsedBalance = Decimal.TryParse(balance, out Decimal parsedBalance);

                    if (isParsedAccountNumber && isParsedBalance && isParsedPayAmount && isParsedPayDate)
                    {
                        var payment = new Payment();
                        payment.Account = parsedAccountNumber;
                        payment.Balance = parsedBalance;
                        payment.PayAmount = parsedPayAmount;
                        payment.PayDate = parsedPayDate;
                        payment.Currency = currency;
                        payment.Description = description;
                        payment.Timestamp = DateTime.Now;
                        paymentsList.Add(payment);
                    }
                    else
                    {
                        using (var file = new StreamWriter(Directory.GetCurrentDirectory() + "/Files/BadRecords.txt", true))
                        {
                            file.WriteLine(fields[0] + ',' + fields[1] + ',' + fields[2] + ',' + fields[3] + ',' + fields[4] + ',' + fields[5]);
                        }
                    }
                }

                var currencyList = paymentsList.Select(x => x.Currency).Distinct().ToList();
                var index = currencyList.IndexOf("EUR");
                currencyList.RemoveAt(index);

                var exchangeRates = GetCurrencyExchange(currencyList);
                foreach (var payment in paymentsList)
                {
                    if (!payment.Currency.Equals("EUR"))
                    {
                        var temp = exchangeRates.rates.GetType().GetProperty(payment.Currency).GetValue(exchangeRates.rates, null);
                        var currencyValue = (decimal)temp;
                        payment.PayAmountCurrency = payment.PayAmount * currencyValue;
                        payment.BalanceCurrency = payment.Balance * currencyValue;
                    }
                    else
                    {
                        payment.PayAmountCurrency = payment.PayAmount;
                        payment.BalanceCurrency = payment.Balance;
                    }
                }
                var totalCount = 0;
                var connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\kdiak\source\repos\PaymentParser\PaymentParser\LocalDatabase\Payment.mdf;Integrated Security=True";
                using (var connection = new SqlConnection(connectionString))
                {
                    var insertQuesry = "INSERT INTO Payment VALUES (@Timestamp, @Account, @Description, @PayDate, @PayAmount, @Balance, @Currency, @BalanceCurrency, @PayAmountCurrency)";
                    connection.Execute(insertQuesry, paymentsList);
                    var countQuery = "select count(*) from Payment";
                    totalCount = connection.ExecuteScalar<int>(countQuery);
                }
                Console.WriteLine("The total rows inserted in table are: {0}", totalCount);

                foreach (var currency in currencyList)
                {
                    var sum = paymentsList.Where(a => a.Currency == currency).Sum(a => a.PayAmount);
                    Console.WriteLine("For currency {0}, the sum of payments in Euro is: {1}", currency, sum);

                }

                Console.ReadKey();

            }
        }
        private static ExchangeRate GetCurrencyExchange(List<string> currencies)
        {
            ExchangeRate exchangeRate = null;
            var baseUrl = "https://api.exchangeratesapi.io/latest?symbols=";
            var url = baseUrl + String.Join(",", currencies.ToArray());
            using (var client = new HttpClient())
            {
                var response = client.GetAsync(url).Result;
                if (response.IsSuccessStatusCode)
                {
                    exchangeRate = JsonConvert.DeserializeObject<ExchangeRate>(response.Content.ReadAsStringAsync().Result);
                }
            }
            return exchangeRate;
        }
    }
}
