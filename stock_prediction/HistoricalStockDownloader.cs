using System;
using System.Collections.Generic;
using System.Net;

namespace stock_prediction
{
	public class HistoricalStockDownloader
	{
        const int DAYSINONEYEAR = 365;
		public static List<HistoricalStockRecord> DownloadData(string ticker, int yearToStartFrom, int yearToEnd)
		{
			List<HistoricalStockRecord> records = new List<HistoricalStockRecord>();

			using (WebClient web = new WebClient())
			{
				string data = web.DownloadString(string.Format("http://ichart.finance.yahoo.com/table.csv?s={0}&c={1}", ticker, yearToStartFrom));

                //System.IO.File.WriteAllText(string.Format("raw data.csv"), data);

				data =  data.Replace("r","");

				string[] rows = data.Split(new string[] {"\n", "\r\n"}, StringSplitOptions.RemoveEmptyEntries);

				int tempYear = 0;

				//First row is headers so Ignore it
				for (int i = 1; i < rows.Length; i++)
				{
					if (rows[i].Replace("n","").Trim() == "") continue;

					string[] cols = rows[i].Split(',');

                    DateTime currentDate = Convert.ToDateTime(cols[0]);

                    int currentYear = currentDate.Year;

					HistoricalStockRecord record;

					if (currentYear >= yearToStartFrom && currentYear <= yearToEnd)
					{

                        //create new record when a new year data is coming
						if (tempYear != currentYear)
						{
							tempYear = currentYear;
							record = new HistoricalStockRecord();
							records.Add(record);
                            record.Quotes = new double[checkLeapYear(currentYear) ? DAYSINONEYEAR + 1 : DAYSINONEYEAR];
                            record.Year = currentYear;
						}
						else
						{
							record = records[records.Count - 1];
						}


//						hs.Open = Convert.ToDouble(cols[1]);
//						hs.High = Convert.ToDouble(cols[2]);
//						hs.Low = Convert.ToDouble(cols[3]);
//						hs.Close = Convert.ToDouble(cols[4]);
//						hs.Volume = Convert.ToDouble(cols[5]);
//                      hs.AdjClose = Convert.ToDouble(cols[6]);

                        double value = Convert.ToDouble(cols[6]);
                        record.Quotes[currentDate.DayOfYear-1] = value;

                        /*fill empty row with close quote data from last day */
                        for (int p = 0; p < 3 && currentDate.DayOfYear+p < (checkLeapYear(currentYear) ? DAYSINONEYEAR + 1 : DAYSINONEYEAR) 
                            && record.Quotes[currentDate.DayOfYear+p] == 0; p++)
                        {
                            record.Quotes[currentDate.DayOfYear+p] = value;
                        }

					}


				}

				return records;
			}
		}

        /*Check if a year is leap year*/
        private static bool checkLeapYear(int year)
        {
            return (year%4 == 0 && year%100 != 0) || (year%400 == 0);
        }
	}
}

