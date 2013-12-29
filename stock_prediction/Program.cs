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

            int daysOfDerivativeInterval = 1;
            int daysOfAvgDerivativeInterval = 5;
            int daysOfPredictionThreshold = 5;
            double valueOfPredictionThreshold = 0.0;

			List<HistoricalStockRecord> data = HistoricalStockDownloader.DownloadData(code, yearToStart, yearToEnd);

			DataAnalysis dataAnalysis = new DataAnalysis();

            dataAnalysis.generateStockQuoteCSV(code, data);
            List<HistoricalStockDerivative> derivatives = dataAnalysis.generateStockDerivative(code, data, daysOfDerivativeInterval);
            AvgHistoricalStockRecord avgDerivatives= dataAnalysis.generateStockDerivativeAvg(code, derivatives);
            double[] predictionResults = dataAnalysis.generatePrediction(code, daysOfAvgDerivativeInterval, avgDerivatives);
            dataAnalysis.generateReport(code, yearToStart, yearToEnd, daysOfPredictionThreshold, valueOfPredictionThreshold, predictionResults);

			Console.Read();
		}
	}
}
