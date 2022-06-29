using System;
using System.Collections.Generic;
using System.Linq;

public class Assessment : IAssessment
{
    /// <summary>
    /// Returns the score with the highest value
    /// </summary>
    public Score? WithMax(IEnumerable<Score> scores)
    {
        if (scores.Count() > 0)
        {
            return scores.Max<Score>();
        }
        else
        {
            return null;
        }
    }

    /// <summary>
    /// Returns the average value of the collection. For an empty collection it returns null
    /// </summary>
    public double? GetAverageOrDefault(IEnumerable<int> items)
    {
        if (items.Count() > 0)
        {
            return items.Average();
        }
        else
        {
            return null;
        }
    }


    /// <summary>
    /// Appends the suffix value to the text if the text has value. If not, it returns empty.
    /// </summary>
    public string WithSuffix(string text, string suffixValue)
    {
        if (!String.IsNullOrEmpty(suffixValue))
        {
            if (!String.IsNullOrEmpty(text))
            {
                return text + (suffixValue.Contains(".") ? suffixValue : "." + suffixValue);
            }
            else
            {
                return String.Empty;
            }
        }
        else
        {
            return "not set suffix value";
        }

    }

    /// <summary>
    /// It fetches all the data from the source.
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    public IEnumerable<Score> GetAllScoresFrom(IDataProvider<Score> source)
    { 
        return source.GetData(nextPageToken: "all").Items.ToList();
    }

    /// <summary>
    /// Returns child's name prefixed with all its parents' names separated by the specified separator.Example : Parent/Child
    /// </summary>
    public string GetFullName(IHierarchy child, string separator = null)
    {
        separator ??= "/";
        return child.Parent.Name + separator + child.Name;
    }

    /// <summary>
    /// Refactor: Returns the value that is closest to the average value of the specified numbers.
    /// </summary>
    public int? ClosestToAverageOrDefault(IEnumerable<int> numbers)
    {
        int avgInt = (int)numbers.Average();

        List<int> listResult = numbers.Where(x => x <= avgInt).ToList();

        if (listResult.Count() == 0)
        {
            return 0;
        }
        else
        {
            return listResult.OrderBy(x=>x).Last();
        }
    }

    public IEnumerable<Score> GetAllScoresFrom()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Groups the specified bookings based on their consecutive dates then by their projects and finally the booking allocation. Read the example carefully.
    /// Example : [{Project:HR, Date: 01/02/2020 , Allocation: 10},
    ///            {Project:CRM, Date: 01/02/2020 , Allocation: 15},
    ///            {Project:HR, Date: 02/02/2020 , Allocation: 10},
    ///            {Project:CRM, Date: 02/02/2020 , Allocation: 15},
    ///            {Project:HR, Date: 03/02/2020 , Allocation: 15},
    ///            {Project:CRM, Date: 03/02/2020 , Allocation: 15},
    ///            {Project:HR, Date: 04/02/2020 , Allocation: 15},
    ///            {Project:CRM, Date: 04/02/2020 , Allocation: 15},
    ///            {Project:HR, Date: 05/02/2020 , Allocation: 15},
    ///            {Project:CRM, Date: 05/02/2020 , Allocation: 15},
    ///            {Project:ECom, Date: 05/02/2020 , Allocation: 15},
    ///            {Project:ECom, Date: 06/02/2020 , Allocation: 10},
    ///            {Project:CRM, Date: 06/02/2020 , Allocation: 15}
    ///            {Project:ECom, Date: 07/02/2020 , Allocation: 10},
    ///            {Project:CRM, Date: 07/02/2020 , Allocation: 15}]    
    /// Returns : 
    ///          [
    ///            { From:01/02/2020 , To:02/02/2020 , [{ Project:CRM , Allocation:15 },{ Project:HR , Allocation:10 }]  },
    ///            { From:03/02/2020 , To:04/02/2020 , [{ Project:CRM , Allocation:15 },{ Project:HR , Allocation:15 }]  },
    ///            { From:05/02/2020 , To:05/02/2020 , [{ Project:CRM , Allocation:15 },{ Project:HR , Allocation:15 },{ Project:ECom , Allocation:15 }]  },
    ///            { From:06/02/2020 , To:07/02/2020 , [{ Project:CRM , Allocation:15 },{ Project:ECom , Allocation:10 }]  }
    ///          ]
    /// </summary>
    public IEnumerable<BookingGrouping> Group(IEnumerable<Booking> dates)
    {
        List<BookingGrouping> result = new List<BookingGrouping>(); // for return

        List<BookingGroupingItem> stack = new List<BookingGroupingItem>(); // for Item BookingGrouping

        
        //List<Booking> dataItem = new List<Booking>();
        //dataItem.AddRange(dates.ToArray());
        dates.OrderByDescending(x => x.Date);

        // find start date
        DateTime sDate = new DateTime();
        // for find start date
        bool setSDate = false;

        // find end date
        DateTime eDate = new DateTime();

        int checkStartDate = 0;

        // For Find duplicates and keep the one with a more recent date
        List<DateTime> disList = dates.Select(x => x.Date).Distinct().ToList();

        foreach (var item in disList)
        {

            List<Booking> srch = dates.Where(x => x.Date == item.Date).ToList();


            if (stack.Count() != 0)
            {
                if (stack.Count() != srch.Count())
                {
                    result.Add(new BookingGrouping()
                    {
                        From = sDate,
                        To = eDate,
                        Items = stack
                    });
                    setSDate = false;
                    checkStartDate = 0;
                    sDate = new DateTime();
                    eDate = new DateTime();
                    stack = new List<BookingGroupingItem>();
                }
            }

            foreach (var item3 in srch)
            {

                BookingGroupingItem itBookingGrouping = new BookingGroupingItem()
                {
                    Project = item3.Project,
                    Allocation = item3.Allocation
                };

                if (checkStartDate == 0)
                {
                    if (setSDate == false)
                    {
                        sDate = item3.Date;
                        stack.Add(itBookingGrouping);
                        setSDate = true;
                    }
                    else
                    {
                        eDate = item3.Date;
                        if (sDate == item3.Date)
                        {
                            var wh = stack.Where(x => x.Project == itBookingGrouping.Project && x.Allocation == itBookingGrouping.Allocation);
                            if (wh.Count() == 0)
                            {
                                stack.Add(itBookingGrouping);
                            }
                        }
                    }
                }
                else
                {
                    var wh = stack.Where(x => x.Project == itBookingGrouping.Project && x.Allocation == itBookingGrouping.Allocation);
                    if (wh.Count() == 0)
                    {
                        result.Add(new BookingGrouping()
                        {
                            From = sDate,
                            To = eDate,
                            Items = stack
                        });


                        setSDate = false;
                        checkStartDate = 0;
                        sDate = item3.Date;
                        eDate = new DateTime();
                        stack = new List<BookingGroupingItem>();
                        stack.Add(itBookingGrouping);

                    }
                    else
                    {
                        eDate = item3.Date;
                    }
                }
            }
            checkStartDate++; // next day
        }

        if (setSDate == true)
        {
            result.Add(new BookingGrouping()
            {
                From = sDate,
                To = eDate,
                Items = stack
            });
            setSDate = false;
            checkStartDate = 0;
            sDate = new DateTime();
            eDate = new DateTime();
            stack = new List<BookingGroupingItem>();
        }

        return result;
    }

    /// <summary>
    /// Merges the specified collections so that the n-th element of the second list should appear after the n-th element of the first collection. 
    /// Example : first : 1,3,5 second : 2,4,6 -> result : 1,2,3,4,5,6
    /// </summary>
    public IEnumerable<int> Merge(IEnumerable<int> first, IEnumerable<int> second)
    {
        List<int> mrg = new List<int>();
        mrg.AddRange(first);
        mrg.AddRange(second);
        mrg.Sort();
        return mrg;
    }
}
