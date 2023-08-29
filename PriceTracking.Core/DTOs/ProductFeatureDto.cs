using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PriceTracking.Core.DTOs
{
    public class ProductFeatureDto
    {
        public string ProductId { get; set; }
        public string ProductTitle { get; set; }
        public string ProductUrl { get; set; }
        public int CategoryId { get; set; }
    }
}
