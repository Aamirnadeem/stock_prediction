using System;
using System.Text;
using System.Collections.Generic;

namespace stock_prediction
{
	public enum PredictionType
	{
		Sell, Buy, Hold
	}

    public class DataAnalysis
	{

		public void generateStockQuoteCSV(string code, List<HistoricalStockRecord> records){

			foreach (var record in records)
			{
				StringBuilder sb = new StringBuilder(); 

				sb.AppendLine(string.Format("{0},{1}", "DayInYear", "Close")); 

                for (int i = 0; i < record.Quotes.Length; i++)
				{
                    sb.AppendLine(string.Format("{0},{1}", i+1, record.Quotes[i])); 
				}

                System.IO.File.WriteAllText(string.Format("{0} - {1} - qutoes.csv", code, record.Year), sb.ToString());
			}
		}

		public List<HistoricalStockDerivative> generateStockDerivative(string code, List<HistoricalStockRecord> records, int daysInterval)
		{
			List<HistoricalStockDerivative> stockDerivatives = new List<HistoricalStockDerivative>();
			foreach (var record in records)
			{
				StringBuilder sb = new StringBuilder();

				sb.AppendLine(string.Format("{0},{1}", "DayInYear", "Derivative"));

				HistoricalStockDerivative stockDerivative = new HistoricalStockDerivative();
				stockDerivative.Year = record.Year;
				stockDerivative.Derivatives = new double[HistoricalStockDownloader.checkLeapYear(record.Year) ? HistoricalStockDownloader.DAYSINONEYEAR + 1 : HistoricalStockDownloader.DAYSINONEYEAR];
				stockDerivatives.Add(stockDerivative);

				for (int i = 0; i < record.Quotes.Length - 1; )
				{
					double date1 = record.Quotes[i];
					double date2 = record.Quotes[i + daysInterval];

					double derivative = Math.Derivative(0, (double)daysInterval, date1, date2);
					stockDerivative.Derivatives[i] = derivative;
					sb.AppendLine(string.Format("{0},{1}", i + 1, derivative));

					i = i + daysInterval;
				}
				System.IO.File.WriteAllText(string.Format("{0} - {1} - Derivative.csv", code, record.Year), sb.ToString());
			}
			return stockDerivatives;
		}

		public AvgHistoricalStockRecord generateStockDerivativeAvg(string code, List<HistoricalStockDerivative> derivatives)
        {
			int count = derivatives.Count;

			AvgHistoricalStockRecord avgStockDerivative = new AvgHistoricalStockRecord();
			avgStockDerivative.StartYear = derivatives[count - 1].Year;
			avgStockDerivative.EndYear = derivatives[0].Year;
			avgStockDerivative.AvgQuotes = new double[HistoricalStockDownloader.DAYSINONEYEAR];

            StringBuilder sb = new StringBuilder();
			sb.AppendLine(string.Format("{0},{1}", "DayInYear", "DerivativeAvg"));

            for (int i = 0; i < HistoricalStockDownloader.DAYSINONEYEAR; i++)
            {
                double sum = 0;
				for (int j = 0; j < count; j++)
                {
                    double derivate = derivatives[j].Derivatives[i];
                    sum += derivate;
                }

				double avg = sum / (avgStockDerivative.EndYear - avgStockDerivative.StartYear + 1);

				sb.AppendLine(string.Format("{0},{1}", i+1, avg));

				avgStockDerivative.AvgQuotes[i] = avg;
            }
			System.IO.File.WriteAllText(string.Format("{0} - {1} - {2} - DerivativeAvg.csv", code, avgStockDerivative.StartYear, avgStockDerivative.EndYear), sb.ToString());
           
			return avgStockDerivative;
		}

		public double[] generatePrediction(string code, int daysInterval, AvgHistoricalStockRecord avgStockDerivative){

			int length = avgStockDerivative.AvgQuotes.Length - daysInterval;

			double[] predictionResults = new double[length];

			StringBuilder sb = new StringBuilder();
			sb.AppendLine(string.Format("{0},{1}", "DayInYear", "Prediction"));

			for (int i = 0; i < length; i++)
			{
				double spanSum = 0.0, spanAvg = 0.0;

				for (int j = 0; j < daysInterval; j++)
				{
					spanSum += avgStockDerivative.AvgQuotes[i + j];
				}

				spanAvg = spanSum / daysInterval;
				predictionResults[i] = spanAvg;

				sb.AppendLine(string.Format("{0},{1}", i + 1, spanAvg));
			}

			System.IO.File.WriteAllText(string.Format("{0} - {1} - {2} - prediction.csv", code, avgStockDerivative.StartYear, avgStockDerivative.EndYear), sb.ToString());

			return predictionResults;
		}

		//generate prediction report to CSV file
		public void generateReport(string code, int yearToStart, int yearToEnd, int yearToPredict, int daysOfAvgDerivativeInterval, int daysThreshold, 
			double valueThreshold, double[] predictionResults, double[] evaluatedData)
		{
			StringBuilder sb = new StringBuilder();
			sb.AppendLine(string.Format("{0},{1},{2},{3}", "Start Year", "End Year", "Prediction Year","Days Interval"));

			PredictionResult result = new PredictionResult();
			result.StartYear = yearToStart;
			result.EndYear = yearToEnd;
			result.PredictedYear = yearToPredict;
			result.DaysInterval = daysOfAvgDerivativeInterval;

			double totalScore = 0;
			PredictionType[] types = new PredictionType[predictionResults.Length];

			sb.AppendLine(string.Format("{0},{1},{2},{3}", result.StartYear, result.EndYear, result.PredictedYear, result.DaysInterval));
			sb.AppendLine(string.Format("{0},{1},{2}", "Day", "Prediction Type", "Evalution Result"));

			for (int i = 0; i < predictionResults.Length; i++)
			{
				PredictionType type = PredictionType.Hold;
				for (int j = 0; j < daysThreshold && i + j < predictionResults.Length; j++)
				{
					double value = predictionResults[i + j];
					PredictionType currentType;

					if (value > valueThreshold)
					{
						currentType = PredictionType.Buy;
					}
					else if (value < valueThreshold)
					{
						currentType = PredictionType.Sell;
					}
					else
					{
						currentType = PredictionType.Hold;
					}


					if (j > 0 && type != currentType)
					{
						type = PredictionType.Hold;
						break;
					}
					else
					{
						type = currentType;
					}

				}

				types[i] = type;
				bool evaluationResult = getEvalutionResult(evaluatedData[i], evaluatedData[i + daysOfAvgDerivativeInterval], type);

				if (evaluationResult)
				{
					totalScore++;
				}

				sb.AppendLine(string.Format("{0},{1},{2}", i + 1, getTypeString(type), evaluationResult));
			}

			sb.AppendLine(string.Format("{0}", totalScore/predictionResults.Length));

			System.IO.File.WriteAllText(string.Format("{0} - {1} - {2} - prediction report.csv", code, yearToStart, yearToEnd), sb.ToString());

		}

		// Convert type to corresponding string
		private string getTypeString(PredictionType type)
		{
			string typeString = "";

			switch (type)
			{
				case PredictionType.Buy:
					typeString = "Buy";
					break;
				case PredictionType.Sell:
					typeString = "Sell";
					break;
				case PredictionType.Hold:
				default:
					typeString = "Hold";
					break;
			}

			return typeString;
		}

		private bool getEvalutionResult(double startDayQuote, double endDayQuote, PredictionType type)
		{
			if (endDayQuote - startDayQuote > 0)
			{
				return type == PredictionType.Buy;
			}
			else if (endDayQuote - startDayQuote < 0)
			{
				return type == PredictionType.Sell;
			}
			else
			{
				return type == PredictionType.Hold;
			}
		}


    }
}

