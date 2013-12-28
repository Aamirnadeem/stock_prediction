using System;
using System.Collections.Generic;
using System.Net;

namespace stock_prediction
{
	public class HistoricalStockDownloader
	{
		public static List<HistoricalStockRecord> DownloadData(string ticker, int yearToStartFrom, int yearToEnd)
		{
			//List<HistoricalStock> retval = new List<HistoricalStock>();
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
						hsn.Close = hs.AdjClose;

						//retval.Add(hs);
						record.nodeList.Add(hsn);
					}


				}

				return records;
			}
		}
	}
}

