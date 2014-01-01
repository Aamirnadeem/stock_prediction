using System;
using System.Collections.Generic;

namespace stock_prediction
{
	class MainClass
	{

		public static void Main (string[] args)
		{
			string code = "AAPL";
			int yearToStart = 2002;
			int yearToEnd = 2012;
			int yearToPredict = yearToEnd + 1;

            int daysOfDerivativeInterval = 1;
            int daysOfAvgDerivativeInterval = 5;
            int daysOfPredictionThreshold = 5;
            double valueOfPredictionThreshold = 0.0;

			List<HistoricalStockRecord> data = HistoricalStockDownloader.DownloadData(code, yearToStart, yearToPredict);
			List<HistoricalStockRecord> watchedData = data.GetRange(0, data.Count - 1);
			List<HistoricalStockRecord> evaluatedData = data.GetRange(data.Count - 1, 1);

			DataAnalysis dataAnalysis = new DataAnalysis();

			dataAnalysis.generateStockQuoteCSV(code, watchedData);
			List<HistoricalStockDerivative> derivatives = dataAnalysis.generateStockDerivative(code, watchedData, daysOfDerivativeInterval);
            AvgHistoricalStockRecord avgDerivatives= dataAnalysis.generateStockDerivativeAvg(code, derivatives);
            double[] predictionResults = dataAnalysis.generatePrediction(code, daysOfAvgDerivativeInterval, avgDerivatives);
			dataAnalysis.generateReport(code, yearToStart, yearToEnd, yearToPredict, 
				daysOfAvgDerivativeInterval, daysOfPredictionThreshold, 
				valueOfPredictionThreshold, predictionResults, evaluatedData[0].Quotes);

			Console.Read();
		}
	}
}
