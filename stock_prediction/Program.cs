using System;
using System.Collections.Generic;
using System.Text;

namespace stock_prediction
{
	class MainClass
	{
		public static void generateCSV(string code, List<HistoricalStockNode> data){

			StringBuilder sb = new StringBuilder(); 

			sb.AppendLine(string.Format("DayInYear,Close")); 

			foreach (var d in data)
			{
				sb.AppendLine(string.Format("{0},{1}", d.DayInYear , d.Close)); 
			}
				 

			System.IO.File.WriteAllText(code + " - stock.csv", sb.ToString());


		}
		public static void Main (string[] args)
		{
			string code = "AAPL";
			int yearToStart = 2003;
			int yearToEnd = 2013;

			List<HistoricalStockNode> data = HistoricalStockDownloader.DownloadData(code, yearToStart, yearToEnd);

			generateCSV(code, data);

			Console.Read();
		}
	}
}
