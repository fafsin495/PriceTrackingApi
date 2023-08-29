using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PriceTracking.Core.DTOs
{
    public class ProductDto:BaseDto
    {
        public string ProductTitle { get; set; }
        public string ProductCategory { get; set; }
        public decimal ProductPrice { get; set; }
        public DateTime ProductDate { get; set; }
        public string ProductId { get; set; }
        public string ProductUrl { get; set; }
    }
}
