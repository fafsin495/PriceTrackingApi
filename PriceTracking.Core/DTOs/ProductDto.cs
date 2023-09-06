using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PriceTracking.Core.DTOs
{
    public class ProductDto:ProductBaseDto
    {
        public decimal ProductPrice { get; set; }
        public DateTime ProductDate { get; set; }
        public string ProductUrl { get; set; }
    }
}
