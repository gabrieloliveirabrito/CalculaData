using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace CalculaData
{
    internal class Program
    {
        static async Task<List<DateTime>> FetchHollidays(int year)
        {
            Console.Write("Fetching hollidays for {0}...", year);
            const string url = "https://api.calendario.com.br/?json=true&token=ZmVsaXBlLmEubWF6aWVyaUBob3RtYWlsLmNvbSZoYXNoPTczMDA3OTc0&ano={0}&estado=MG&cidade=Passos";

            WebClient client = new WebClient();
            string response = await client.DownloadStringTaskAsync(string.Format(url, year));
            List<HollidayData> hollidays = JsonConvert.DeserializeObject<List<HollidayData>>(response, new IsoDateTimeConverter { DateTimeFormat = "dd/MM/yyyy" });

            Console.WriteLine("{0} hollidays has been fetch!", hollidays.Count);
            return hollidays.Select(h => h.Date).ToList();
        }

        static async Task<bool> Run(uint workHour, uint totalWork)
        {
            try
            {
                List<DateTime> hollidays = new List<DateTime>();
                DateTime startDate = new DateTime(2021, 11, 29);
                DateTime endDate = startDate;

                hollidays.Add(new DateTime(2021, 12, 20));
                hollidays.Add(new DateTime(2021, 12, 21));
                hollidays.Add(new DateTime(2021, 12, 22));
                hollidays.Add(new DateTime(2021, 12, 23));
                hollidays.Add(new DateTime(2021, 12, 24));
                hollidays.AddRange(await FetchHollidays(endDate.Year));

                ulong dayCount = 0;
                while (dayCount * workHour < totalWork)
                {
                    if (endDate.DayOfWeek == DayOfWeek.Saturday || endDate.DayOfWeek == DayOfWeek.Sunday)
                        Console.WriteLine("{0} - Weekend", endDate.ToString("dd/MM/yyyy"));
                    else if (hollidays.Contains(endDate))
                        Console.WriteLine("{0} - Holliday", endDate.ToString("dd/MM/yyyy"));
                    else
                    {
                        Console.WriteLine("{0} - Workday", endDate.ToString("dd/MM/yyyy"));
                        dayCount++;
                    }

                    endDate = endDate.AddDays(1);
                    if (endDate.Day == 1 && endDate.Month == 1)
                        hollidays.AddRange(await FetchHollidays(endDate.Year));
                }

                Console.WriteLine("Total of {0} work days, {1} work hours", dayCount, dayCount * 6);
                Console.WriteLine("Start Date = {0}", startDate.ToString("dd/MM/yyyy"));
                Console.WriteLine("End Date = {0}", endDate.ToString("dd/MM/yyyy"));

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
                return false;
            }
        }
        static void Main(string[] args)
        {
            bool result = Run(6, 500).Result;

            Console.WriteLine("Result = {0}", result ? "OK" : "FAILED");
            Console.ReadLine();
        }
    }
}
