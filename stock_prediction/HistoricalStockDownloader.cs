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

				data =  data.Replace("r","");

				string[] rows = data.Split(new string[] {"\n", "\r\n"}, StringSplitOptions.RemoveEmptyEntries);

				int tempYear = 0;

				//First row is headers so Ignore it
				for (int i = 1; i < rows.Length; i++)
				{
					if (rows[i].Replace("n","").Trim() == "") continue;

					string[] cols = rows[i].Split(',');


					HistoricalStock hs = new HistoricalStock();

					hs.Date = Convert.ToDateTime(cols[0]);

					int currentYear = hs.Date.Year;

					HistoricalStockRecord record;


					if (currentYear >= yearToStartFrom && currentYear <= yearToEnd)
					{

					   HistoricalStockNode hsn = new HistoricalStockNode();

                        //create new record when a new year data is coming
						if (tempYear != currentYear)
						{
							tempYear = currentYear;
							record = new HistoricalStockRecord();
							records.Add(record);
							record.nodeList = new List<HistoricalStockNode>();
							record.Year = hs.Date.Year;
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

						hs.AdjClose = Convert.ToDouble(cols[6]);


						hsn.DayInYear = hs.Date.DayOfYear;

                        //fill empty row with last close value
                        if (DAYSINONEYEAR - hsn.DayInYear > record.nodeList.Count)
                        {
                            double lastClose = 0.0;

                            if (record.nodeList.Count == 0)
                            {
                                if (records.Count >= 2)
                                {
                                    HistoricalStockRecord lastRecord = records[records.Count - 2];
                                    lastClose = lastRecord.nodeList[lastRecord.nodeList.Count - 1].Close;
                                }
                                else
                                {
                                    lastClose = 0.0;
                                }
                            }
                            else
                            {
                                lastClose = record.nodeList[record.nodeList.Count - 1].Close;
                            }


                            for(int p = record.nodeList.Count; p < DAYSINONEYEAR - hsn.DayInYear; p++)
                            {
                                HistoricalStockNode hsnFill = new HistoricalStockNode();
                                hsnFill.DayInYear = DAYSINONEYEAR - p;
                                hsnFill.Close = lastClose;

                                record.nodeList.Add(hsnFill);
                            }  
                        }

						hsn.Close = hs.AdjClose;
						record.nodeList.Add(hsn);
					}


				}

				return records;
			}
		}
	}
}

