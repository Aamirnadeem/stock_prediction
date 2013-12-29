using System;
using System.Text;
using System.Collections.Generic;

namespace stock_prediction
{
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

        public List<HistoricalStockDerivative> generateStockDerivativeCSV(string code, List<HistoricalStockRecord> records, int daysInterval)
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

        public void generateStockDerivativeSumCSV(string code, List<HistoricalStockDerivative> derivatives)
        {
            int count = derivatives.Count;

            int endYear = derivatives[0].Year;
            int startYear= derivatives[count - 1].Year;

            StringBuilder sb = new StringBuilder();
            sb.AppendLine(string.Format("{0},{1}", "DayInYear", "DerivativeSum"));

            for (int i = 0; i < HistoricalStockDownloader.DAYSINONEYEAR; i++)
            {
                double sum = 0;
                for (int j = 0; j < count; j++)
                {
                    double derivate = derivatives[j].Derivatives[i];
                    sum += derivate;
                }
                sb.AppendLine(string.Format("{0},{1}", i+1, sum));
            }
            System.IO.File.WriteAllText(string.Format("{0} - {1} - {2} - DerivativeSum.csv", code, startYear, endYear), sb.ToString());
        }
    }
}

