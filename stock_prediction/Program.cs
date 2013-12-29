using System;
using System.Collections.Generic;

namespace stock_prediction
{
	class MainClass
	{

		public static void Main (string[] args)
		{
			string code = "AAPL";
			int yearToStart = 2011;
			int yearToEnd = 2013;

			List<HistoricalStockRecord> data = HistoricalStockDownloader.DownloadData(code, yearToStart, yearToEnd);

			DataAnalysis dataAnalysis = new DataAnalysis();

            dataAnalysis.generateStockQuoteCSV(code, data);
            List<HistoricalStockDerivative> derivatives =dataAnalysis.generateStockDerivativeCSV(code, data, 1);
            dataAnalysis.generateStockDerivativeSumCSV(code, derivatives);

			Console.Read();
		}
	}
}
