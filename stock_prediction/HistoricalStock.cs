using System;
using System.Collections.Generic;

namespace stock_prediction
{
	public class HistoricalStock
	{
			public DateTime Date { get; set; }
			public double Open { get; set; }
			public double High { get; set; }
			public double Low { get; set; }
			public double Close { get; set; }
			public double Volume { get; set; }
			public double AdjClose { get; set; }
	}

	public class HistoricalStockRecord
	{
		public int Year { get; set;}
        public double[] Quotes { get; set; }
	}

    public class HistoricalStockDerivative
    {
        public int Year { get; set; }
        public double[] Derivatives { get; set; }
    }
}

