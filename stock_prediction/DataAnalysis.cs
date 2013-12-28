using System;
using System.Text;
using System.Collections.Generic;

namespace stock_prediction
{
    public class DataAnalysis
	{

		public void generateCSV(string code, List<HistoricalStockRecord> records){

			foreach (var record in records)
			{
				StringBuilder sb = new StringBuilder(); 

				sb.AppendLine(string.Format("{0},{1}", "DayInYear", "Close")); 

				foreach (var node in record.nodeList)
				{
					sb.AppendLine(string.Format("{0},{1}", node.DayInYear , node.Close)); 
				}

				System.IO.File.WriteAllText(string.Format("{0} - {1}.csv", code, record.Year), sb.ToString());
			}


		}
    }
}

