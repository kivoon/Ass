using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace testApp
{
    class Program
    {
        static void Main(string[] args)
        {
            Assessment assm = new Assessment();

            var data = File.ReadAllText("json2.json");
            var listData = JsonConvert.DeserializeObject<IEnumerable<Booking>>(data);
            IEnumerable<BookingGrouping> Group = assm.Group(listData);
            System.Console.WriteLine("check Function Group : ");
            foreach (var item in Group)
            {
                System.Console.WriteLine("from : " + item.From + " / to : " + item.To);
                System.Console.WriteLine("  items : ");
                foreach (var item2 in item.Items)
                {
                    System.Console.WriteLine("      ---> " + item2.Project + " /// " + item2.Allocation);
                }
            }


            System.Console.WriteLine("check Function Merge : ");
            IEnumerable<int> first = new int[] { 1, 4, 6 };
            IEnumerable<int> second = new int[] { 5, 2, 3, 10 };
            IEnumerable<int> merge = assm.Merge(first, second);
            foreach (var item in merge)
            {
                System.Console.WriteLine(item.ToString());
            }


            System.Console.WriteLine("function WithSuffix : " + assm.WithSuffix("project", "pdf"));



            IEnumerable<int> value = new int[] { 1, 4, 6, 5, 3, 5, 6, 8, 10, 10, 10, 10, 10, 10, 10, 18, 15, 2, 4, 3, 2 };
            System.Console.WriteLine("function ClosestToAverageOrDefault : " + assm.ClosestToAverageOrDefault(value).Value.ToString());
            System.Console.WriteLine("function GetAverageOrDefault : " + assm.GetAverageOrDefault(value).Value.ToString());


            IEnumerable<Score> scores = new Score[]
            {
                new Score() { Value = 5 },
                new Score() { Value = 10 },
                new Score() { Value = 3 },
                new Score() { Value = 6 },
                new Score() { Value = 13 },
                new Score() { Value = 7 },
                new Score() { Value = 9 },

            };
            System.Console.WriteLine("function WithMax : " + assm.WithMax(scores).Value.ToString());
        }
    }
}

