using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PriceTracking.Core.DTOs
{
    public class InflationDto:ProductBaseDto
    {
        public List<ProductDto> Products { get; set; }
        public decimal StartingPrice { get; set; }
        public decimal EndingPrice { get; set; }
        public decimal InflationDifference { get; set; }
        public decimal InflationPercentageDifference { get; set; }

        public int DurationDate { get; set; }

    }
}
