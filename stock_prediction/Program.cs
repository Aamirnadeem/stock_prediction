using System;
using System.Collections.Generic;

namespace stock_prediction
{
	class MainClass
	{

		public static void Main (string[] args)
		{
			string code = "AAPL";
			int yearToStart = 2003;
			int yearToEnd = 2013;

			List<HistoricalStockRecord> data = HistoricalStockDownloader.DownloadData(code, yearToStart, yearToEnd);

			DataAnalysis dataAnalysis = new DataAnalysis();

            dataAnalysis.GenerateStockQuoteCSV(code, data);
            dataAnalysis.GenerateStockDerivativeCSV(code, data, 1);

			Console.Read();
		}
	}
}
