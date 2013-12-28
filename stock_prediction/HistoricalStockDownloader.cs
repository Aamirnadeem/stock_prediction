using System;
using System.Collections.Generic;
using System.Net;

namespace stock_prediction
{
	public class HistoricalStockDownloader
	{
		public static List<HistoricalStockNode> DownloadData(string ticker, int yearToStartFrom, int yearToEnd)
		{
			//List<HistoricalStock> retval = new List<HistoricalStock>();
			List<HistoricalStockNode> retvalMetrix = new List<HistoricalStockNode>();

			using (WebClient web = new WebClient())
			{
				string data = web.DownloadString(string.Format("http://ichart.finance.yahoo.com/table.csv?s={0}&c={1}", ticker, yearToStartFrom));

				data =  data.Replace("r","");

				string[] rows = data.Split(new string[] {"\n", "\r\n"}, StringSplitOptions.RemoveEmptyEntries);

				//First row is headers so Ignore it
				for (int i = 1; i < rows.Length; i++)
				{
					if (rows[i].Replace("n","").Trim() == "") continue;

					string[] cols = rows[i].Split(',');

					HistoricalStock hs = new HistoricalStock();
					HistoricalStockNode hsn = new HistoricalStockNode();


					hs.Date = Convert.ToDateTime(cols[0]);

					if (hs.Date.Year >= yearToStartFrom && yearToStartFrom <= yearToEnd)
					{
						hs.Open = Convert.ToDouble(cols[1]);
						hs.High = Convert.ToDouble(cols[2]);
						hs.Low = Convert.ToDouble(cols[3]);
						hs.Close = Convert.ToDouble(cols[4]);
						hs.Volume = Convert.ToDouble(cols[5]);
						hs.AdjClose = Convert.ToDouble(cols[6]);

						hsn.DayInYear = hs.Date.DayOfYear;
						hsn.Close = hs.AdjClose;

						//retval.Add(hs);
						retvalMetrix.Add(hsn);
					}


				}

				return retvalMetrix;
			}
		}
	}
}

